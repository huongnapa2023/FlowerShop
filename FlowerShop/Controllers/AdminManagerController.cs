using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    /// <summary>
    /// Trung tâm Quản lý & Phân quyền Admin con và Hồ sơ Nhân sự Chi nhánh.
    /// </summary>
    [AdminPermissionAuthorize]
    public class AdminManagerController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // GET: AdminManager/Index
        [AdminSuperAuthorize]
        public ActionResult Index()
        {
            DbInitializer.EnsureUserRbacColumnsAndSeedSubAdmins(_context);

            var adminList = _context.Users
                .Where(u => u.Role == "Admin")
                .OrderByDescending(u => u.IsSuperAdmin)
                .ThenBy(u => u.CreatedAt)
                .ToList();

            ViewBag.AllPermissions = AdminPermissions.GetAll();
            ViewBag.PermissionDescriptions = AdminPermissions.Descriptions;
            ViewBag.PermissionBadges = AdminPermissions.Badges;

            return View(adminList);
        }

        // GET: AdminManager/StaffDetails/5
        // Cho phép xem toàn bộ hồ sơ chi tiết của nhân viên/quản trị viên
        public ActionResult StaffDetails(int id)
        {
            DbInitializer.EnsureUserRbacColumnsAndSeedSubAdmins(_context);

            var staff = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Admin");
            if (staff == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ nhân viên trong hệ thống.";
                return RedirectToAction("Index");
            }

            ViewBag.AllPermissions = AdminPermissions.GetAll();
            ViewBag.PermissionDescriptions = AdminPermissions.Descriptions;
            ViewBag.PermissionBadges = AdminPermissions.Badges;

            return View(staff);
        }

        // POST: AdminManager/UpdateStaffInfo
        [AdminSuperAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStaffInfo(int userId, string fullName, string phone, string address)
        {
            var adminUser = _context.Users.Find(userId);
            if (adminUser == null || adminUser.Role != "Admin")
            {
                TempData["Error"] = "Không tìm thấy tài khoản nhân viên cần cập nhật.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                adminUser.FullName = fullName.Trim();
                adminUser.Full_name = fullName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                adminUser.Phone = phone.Trim();
            }
            if (!string.IsNullOrWhiteSpace(address))
            {
                adminUser.Address = address.Trim();
            }

            adminUser.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã cập nhật thông tin hồ sơ cho nhân sự \"{0}\" ({1}) thành công!", adminUser.FullName, adminUser.Email);
            return RedirectToAction("StaffDetails", new { id = userId });
        }

        // POST: AdminManager/CreateAdmin
        [AdminSuperAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(string email, string fullName, string password, string phone, string address, string[] permissions)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ Email, Họ tên và Mật khẩu cho tài khoản Admin con.";
                return RedirectToAction("Index");
            }

            email = email.Trim().ToLowerInvariant();
            if (_context.Users.Any(u => u.Email.ToLower() == email))
            {
                TempData["Error"] = string.Format("Email \"{0}\" đã tồn tại trong hệ thống. Vui lòng chọn email khác.", email);
                return RedirectToAction("Index");
            }

            if (password.Length < 6)
            {
                TempData["Error"] = "Mật khẩu Admin phải có độ dài tối thiểu 6 ký tự.";
                return RedirectToAction("Index");
            }

            var permSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (permissions != null)
            {
                foreach (var p in permissions.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    permSet.Add(p.Trim().ToUpperInvariant());
                }
            }
            permSet.Add("CRUD_GRANULAR");
            string permsString = string.Join(",", permSet);

            var newAdmin = new User
            {
                Email = email,
                FullName = fullName.Trim(),
                Full_name = fullName.Trim(),
                Password = AppHelpers.HashPassword(password),
                Phone = !string.IsNullOrWhiteSpace(phone) ? phone.Trim() : "1900 6868",
                Address = !string.IsNullOrWhiteSpace(address) ? address.Trim() : "Cơ sở Hà Nội",
                Role = "Admin",
                IsSuperAdmin = false,
                AllowedPermissions = permsString,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(newAdmin);
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã tạo mới tài khoản Admin con \"{0}\" ({1} - {2}) thành công!",
                newAdmin.FullName, newAdmin.Address, newAdmin.Email);

            return RedirectToAction("Index");
        }

        // POST: AdminManager/SavePermissions
        [AdminSuperAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePermissions(int userId, string branchName, string[] permissions)
        {
            var adminUser = _context.Users.Find(userId);
            if (adminUser == null || adminUser.Role != "Admin")
            {
                TempData["Error"] = "Không tìm thấy tài khoản quản trị viên cần phân quyền.";
                return RedirectToAction("Index");
            }

            if (adminUser.IsSuperAdmin)
            {
                TempData["Error"] = "Tài khoản Admin Tổng luôn có toàn quyền tối cao, không thể giới hạn quyền!";
                return RedirectToAction("Index");
            }

            var permSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (permissions != null)
            {
                foreach (var p in permissions.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    permSet.Add(p.Trim().ToUpperInvariant());
                }
            }
            permSet.Add("CRUD_GRANULAR");
            string permsString = string.Join(",", permSet);

            if (!string.IsNullOrWhiteSpace(branchName))
            {
                adminUser.Address = branchName.Trim();
            }

            adminUser.AllowedPermissions = permsString;
            adminUser.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã lưu phân quyền chi tiết (Xem/Thêm/Sửa/Xóa) cho \"{0}\" ({1}) thành công! Hiệu lực ngay lập tức.",
                adminUser.FullName, adminUser.Address);

            return RedirectToAction("Index");
        }

        // POST: AdminManager/ToggleActive
        [AdminSuperAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int userId, string returnUrl = null)
        {
            var adminUser = _context.Users.Find(userId);
            if (adminUser == null || adminUser.Role != "Admin")
            {
                TempData["Error"] = "Không tìm thấy tài khoản quản trị viên.";
                return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
            }

            if (adminUser.IsSuperAdmin)
            {
                TempData["Error"] = "Không được phép khóa tài khoản Admin Tổng tối cao!";
                return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
            }

            adminUser.IsActive = !adminUser.IsActive;
            adminUser.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã {0} tài khoản \"{1}\" thành công!",
                adminUser.IsActive ? "kích hoạt mở lại" : "tạm khóa", adminUser.Email);

            return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
        }

        // POST: AdminManager/ResetPassword
        [AdminSuperAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(int userId, string newPassword, string returnUrl = null)
        {
            var adminUser = _context.Users.Find(userId);
            if (adminUser == null || adminUser.Role != "Admin")
            {
                TempData["Error"] = "Không tìm thấy tài khoản quản trị viên.";
                return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có tối thiểu 6 ký tự.";
                return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
            }

            adminUser.Password = AppHelpers.HashPassword(newPassword);
            adminUser.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã đặt lại mật khẩu cho tài khoản \"{0}\" thành công!", adminUser.Email);
            return !string.IsNullOrEmpty(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
