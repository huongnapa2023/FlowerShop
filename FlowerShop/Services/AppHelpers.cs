using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using FlowerShop.Models;

namespace FlowerShop.Services
{
    public class ChamPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public string Role { get; private set; }
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public bool IsVip { get; private set; }
        public bool IsSuperAdmin { get; private set; }
        public string BranchName { get; private set; }
        public HashSet<string> Permissions { get; private set; }
        public string AllowedPermissionsRaw { get; private set; }

        public ChamPrincipal(IIdentity identity, string role, int userId, string email, bool isVip, bool isSuperAdmin = false, string allowedPermissions = null, string branchName = null)
        {
            Identity = identity;
            Role = role ?? "Customer";
            UserId = userId;
            Email = email ?? string.Empty;
            FullName = identity != null ? identity.Name : string.Empty;
            IsVip = isVip;
            IsSuperAdmin = isSuperAdmin;
            BranchName = !string.IsNullOrWhiteSpace(branchName) ? branchName : (isSuperAdmin ? "Trụ sở Tổng (Toàn hệ thống)" : "Chi nhánh CHAM Flowers");
            AllowedPermissionsRaw = allowedPermissions ?? string.Empty;
            Permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(allowedPermissions))
            {
                string[] parts = allowedPermissions.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                        Permissions.Add(p.Trim().ToUpperInvariant());
                }
            }
        }

        public bool IsInRole(string role)
        {
            return string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
        }

        public bool HasPermission(string permissionCode)
        {
            if (!string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return false;

            if (IsSuperAdmin)
                return true;

            if (string.IsNullOrWhiteSpace(permissionCode))
                return true;

            if (Permissions == null)
                return false;

            string code = permissionCode.Trim().ToUpperInvariant();

            // Nếu kiểm tra quyền chi tiết cấp hành động (VD: ORDER_CREATE, ORDER_EDIT, ORDER_DELETE)
            if (code.EndsWith("_CREATE", StringComparison.Ordinal) ||
                code.EndsWith("_EDIT", StringComparison.Ordinal) ||
                code.EndsWith("_DELETE", StringComparison.Ordinal))
            {
                int lastUnderscore = code.LastIndexOf('_');
                string baseModule = code.Substring(0, lastUnderscore);

                // Phải có quyền truy cập phân hệ gốc trước
                if (!Permissions.Contains(baseModule))
                    return false;

                // Nếu trong tập quyền đã có ít nhất 1 cấu hình chi tiết của phân hệ này hoặc cờ GRANULAR
                bool hasExplicitCrudConfig =
                    Permissions.Contains(baseModule + "_CREATE") ||
                    Permissions.Contains(baseModule + "_EDIT") ||
                    Permissions.Contains(baseModule + "_DELETE") ||
                    Permissions.Contains("CRUD_GRANULAR");

                if (hasExplicitCrudConfig)
                {
                    return Permissions.Contains(code);
                }

                // Tương thích ngược nếu dữ liệu cũ chỉ lưu tên phân hệ: cho phép Tạo/Sửa nhưng khóa Xóa nếu chưa cấp rõ
                return !code.EndsWith("_DELETE", StringComparison.Ordinal);
            }

            return Permissions.Contains(code);
        }
    }

    public static class AppHelpers
    {
        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

        public static int? GetCurrentUserId(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            if (cp != null && cp.UserId > 0)
                return cp.UserId;
            return null;
        }

        public static string GetCurrentEmail(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            return cp != null ? cp.Email : string.Empty;
        }

        public static string GetCurrentFullName(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            if (cp != null && !string.IsNullOrWhiteSpace(cp.FullName))
                return cp.FullName;
            return user != null && user.Identity != null && !string.IsNullOrWhiteSpace(user.Identity.Name)
                ? user.Identity.Name
                : string.Empty;
        }

        public static bool GetCurrentIsVip(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            return cp != null && cp.IsVip;
        }

        public static bool CurrentAdminHasPermission(string permissionCode)
        {
            var user = HttpContext.Current != null ? HttpContext.Current.User : null;
            var cp = user as ChamPrincipal;
            if (cp == null)
                return false;
            return cp.HasPermission(permissionCode);
        }

        public static bool CurrentUserIsSuperAdmin()
        {
            var user = HttpContext.Current != null ? HttpContext.Current.User : null;
            var cp = user as ChamPrincipal;
            return cp != null && cp.IsSuperAdmin;
        }

        public static string GetCurrentAdminBranch()
        {
            var user = HttpContext.Current != null ? HttpContext.Current.User : null;
            var cp = user as ChamPrincipal;
            if (cp == null)
                return "Chi nhánh CHAM Flowers";
            return !string.IsNullOrWhiteSpace(cp.BranchName)
                ? cp.BranchName
                : (cp.IsSuperAdmin ? "Trụ sở Tổng (Toàn hệ thống)" : "Chi nhánh CHAM Flowers");
        }

        public static bool CanCreate(string moduleCode)
        {
            return CurrentAdminHasPermission(Models.AdminPermissions.CreateOf(moduleCode));
        }

        public static bool CanEdit(string moduleCode)
        {
            return CurrentAdminHasPermission(Models.AdminPermissions.EditOf(moduleCode));
        }

        public static bool CanDelete(string moduleCode)
        {
            return CurrentAdminHasPermission(Models.AdminPermissions.DeleteOf(moduleCode));
        }

        public static string GetFirstAllowedUrl(IPrincipal user, System.Web.Mvc.UrlHelper url)
        {
            var cp = user as ChamPrincipal;
            if (cp == null || url == null)
                return "/";

            if (cp.IsSuperAdmin || cp.HasPermission(Models.AdminPermissions.Report))
                return url.Action("Index", "Report");

            if (cp.HasPermission(Models.AdminPermissions.Order))
                return url.Action("AdminIndex", "Order");

            if (cp.HasPermission(Models.AdminPermissions.Product))
                return url.Action("AdminIndex", "Product");

            if (cp.HasPermission(Models.AdminPermissions.Category))
                return url.Action("Categories", "Product");

            if (cp.HasPermission(Models.AdminPermissions.HotDeal))
                return url.Action("Manage", "HotDeal");

            if (cp.HasPermission(Models.AdminPermissions.Customer))
                return url.Action("Index", "Customer");

            if (cp.HasPermission(Models.AdminPermissions.Review))
                return url.Action("Index", "Review");

            if (cp.HasPermission(Models.AdminPermissions.Chat))
                return url.Action("Index", "Chat");

            if (cp.HasPermission(Models.AdminPermissions.Banner))
                return url.Action("Index", "Banner");

            return url.Action("AccessDenied", "Account");
        }

        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "KH";

            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string first = parts[parts.Length - 2];
                string second = parts[parts.Length - 1];
                return (first.Substring(0, 1) + second.Substring(0, 1)).ToUpperInvariant();
            }
            if (parts.Length == 1 && parts[0].Length > 0)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }
            return "KH";
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }

            return string.Format("PBKDF2${0}${1}", Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            if (!storedHash.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                return storedHash == password;
            }

            string[] parts = storedHash.Split('$');
            if (parts.Length != 3)
                return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            byte[] actualHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                actualHash = pbkdf2.GetBytes(32);
            }

            return FixedTimeEquals(actualHash, expectedHash);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        // Kiểm tra định dạng file ảnh theo đúng Mục 3.1.2 & kịch bản kiểm thử SP04
        public static bool IsImageValid(HttpPostedFileBase imageFile)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
                return false;

            string ext = Path.GetExtension(imageFile.FileName);
            if (string.IsNullOrEmpty(ext))
                return false;

            ext = ext.ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                return false;

            if (imageFile.ContentLength > 10 * 1024 * 1024) // 10 MB
                return false;

            return true;
        }

        public static string SaveUploadedImage(HttpPostedFileBase file, string webRootPath, string subFolder = "products")
        {
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string uploadsDir = Path.Combine(webRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsDir);

            string fileName = string.Format("{0:yyyyMMddHHmmss}_{1:N}{2}", DateTime.Now, Guid.NewGuid(), ext);
            string fullPath = Path.Combine(uploadsDir, fileName);

            file.SaveAs(fullPath);

            return string.Format("/uploads/{0}/{1}", subFolder, fileName);
        }

        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "san-pham-" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string normalized = input.Trim().ToLowerInvariant()
                .Replace('đ', 'd')
                .Replace('Đ', 'd');

            string formD = normalized.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char ch in formD)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            string clean = sb.ToString().Normalize(NormalizationForm.FormC);
            clean = Regex.Replace(clean, @"[^a-z0-9\s-]", "");
            clean = Regex.Replace(clean, @"\s+", "-").Trim('-');
            clean = Regex.Replace(clean, @"-+", "-");

            return string.IsNullOrEmpty(clean) ? "san-pham" : clean;
        }

        public static string FormatVnd(decimal amount)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N0} ₫", amount).Replace(",", ".");
        }

        private static bool _schemaEnsured = false;

        /// <summary>
        /// Lấy thông tin Chiến dịch / Mùa lễ hội đang có hiệu lực trong thời gian hiện tại
        /// </summary>
        public static Campaign GetActiveCampaign()
        {
            try
            {
                using (var db = new FlowerShopDbContext())
                {
                    var now = DateTime.Now;
                    return db.Campaigns.AsNoTracking()
                        .Where(c => c.IsActive && now >= c.StartDate && now <= c.EndDate)
                        .OrderByDescending(c => c.StartDate)
                        .FirstOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy cấu hình Tuỳ biến Header & Footer toàn hệ thống (SiteSetting)
        /// </summary>
        public static SiteSetting GetSiteSetting()
        {
            try
            {
                using (var db = new FlowerShopDbContext())
                {
                    if (!_schemaEnsured)
                    {
                        DbInitializer.EnsurePopupBuilderAndSiteSettings(db);
                        _schemaEnsured = true;
                    }

                    var setting = db.SiteSettings.AsNoTracking().FirstOrDefault();
                    return setting ?? new SiteSetting();
                }
            }
            catch
            {
                return new SiteSetting();
            }
        }
    }
}
