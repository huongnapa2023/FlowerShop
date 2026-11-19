using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email);
            if (user == null && email.EndsWith("@ntflowers.vn", StringComparison.OrdinalIgnoreCase))
            {
                string migratedEmail = email.Replace("@ntflowers.vn", "@chamflowers.vn");
                user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == migratedEmail);
            }

            if (user == null || !AppHelpers.VerifyPassword(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
                return View(model);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã tạm khóa. Vui lòng liên hệ CSKH CHAM Flowers để được hỗ trợ.");
                return View(model);
            }

            SignInUser(user, model.RememberMe);
            MergeSessionCartToUser(user.Id);

            TempData["Success"] = string.Format("Xin chào {0}! Chào mừng bạn quay lại CHAM Flowers.", user.FullName);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (user.Role == "Admin")
            {
                if (user.IsSuperAdmin || (!string.IsNullOrEmpty(user.AllowedPermissions) && user.AllowedPermissions.IndexOf(AdminPermissions.Report, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return RedirectToAction("Index", "Report");
                }

                // Tự động chuyển hướng Admin con về chức năng đầu tiên mà họ được Admin Tổng phân quyền
                string perms = (user.AllowedPermissions ?? "").ToUpperInvariant();
                if (perms.Contains(AdminPermissions.Order))
                    return RedirectToAction("AdminIndex", "Order");
                if (perms.Contains(AdminPermissions.Product))
                    return RedirectToAction("AdminIndex", "Product");
                if (perms.Contains(AdminPermissions.Category))
                    return RedirectToAction("Categories", "Product");
                if (perms.Contains(AdminPermissions.HotDeal))
                    return RedirectToAction("Manage", "HotDeal");
                if (perms.Contains(AdminPermissions.Customer))
                    return RedirectToAction("Index", "Customer");
                if (perms.Contains(AdminPermissions.Review))
                    return RedirectToAction("Index", "Review");
                if (perms.Contains(AdminPermissions.Chat))
                    return RedirectToAction("Index", "Chat");
                if (perms.Contains(AdminPermissions.Banner))
                    return RedirectToAction("Index", "Banner");

                return RedirectToAction("AccessDenied", "Account");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email.Trim().ToLowerInvariant();
            if (_context.Users.Any(u => u.Email.ToLower() == email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng đăng nhập hoặc dùng email khác.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Full_name = model.FullName.Trim(),
                Email = email,
                Phone = model.Phone.Trim(),
                Address = model.Address.Trim(),
                BirthDate = model.BirthDate,
                Password = AppHelpers.HashPassword(model.Password),
                Role = "Customer",
                IsActive = true,
                IsVip = false,
                TotalSpent = 0,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            SignInUser(user, true);
            MergeSessionCartToUser(user.Id);

            TempData["Success"] = "Đăng ký tài khoản thành công! Tặng bạn mã CHAOHE10 giảm 10% cho đơn đầu tiên.";
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            TempData["Success"] = "Bạn đã đăng xuất khỏi hệ thống.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string email, string phone, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email, Số điện thoại đăng ký và mật khẩu mới (tối thiểu 6 ký tự).";
                return View();
            }

            string cleanEmail = email.Trim().ToLowerInvariant();
            string cleanPhone = phone.Trim();

            var user = _context.Users.FirstOrDefault(u =>
                u.Email.ToLower() == cleanEmail &&
                u.Phone == cleanPhone);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản khớp với Email và Số điện thoại đã cung cấp.";
                return View();
            }

            user.Password = AppHelpers.HashPassword(newPassword);
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.";
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public new ActionResult Profile()
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Logout");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("Logout");

            // Nếu tài khoản là Admin / Nhân viên cơ sở -> Điều hướng sang Hồ sơ Nhân sự chuyên nghiệp
            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("StaffProfile");
            }

            var vm = new ProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone ?? string.Empty,
                Address = user.Address,
                BirthDate = user.BirthDate,
                TotalSpent = user.TotalSpent,
                IsVip = user.IsVip
            };

            ViewBag.RecentOrders = _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToList();

            return View(vm);
        }

        // GET: Account/StaffProfile
        // Hồ sơ cá nhân chuyên biệt dành cho Nhân sự / Quản trị viên
        [Authorize]
        [HttpGet]
        public ActionResult StaffProfile()
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Logout");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("Logout");

            if (!string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Profile");
            }

            ViewBag.AllPermissions = AdminPermissions.GetAll();
            ViewBag.PermissionDescriptions = AdminPermissions.Descriptions;
            ViewBag.PermissionBadges = AdminPermissions.Badges;

            return View(user);
        }

        // POST: Account/UpdateMyStaffProfile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateMyStaffProfile(string fullName, string phone)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Logout");

            var user = _context.Users.Find(userId.Value);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Logout");

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                user.FullName = fullName.Trim();
                user.Full_name = fullName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(phone))
            {
                user.Phone = phone.Trim();
            }
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            SignInUser(user, true);

            TempData["Success"] = "Đã cập nhật thông tin hồ sơ nhân sự của bạn thành công!";
            return RedirectToAction("StaffProfile");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public new ActionResult Profile(ProfileViewModel model)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Logout");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("Logout");

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                model.TotalSpent = user.TotalSpent;
                model.IsVip = user.IsVip;
                ViewBag.RecentOrders = _context.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList();
                return View(model);
            }

            user.FullName = model.FullName.Trim();
            user.Full_name = model.FullName.Trim();
            user.Phone = model.Phone.Trim();
            user.Address = model.Address.Trim();
            user.BirthDate = model.BirthDate;
            user.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            SignInUser(user, true);

            TempData["Success"] = "Cập nhật thông tin tài khoản thành công!";
            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Logout");

            var user = _context.Users.Find(userId.Value);
            if (user == null)
                return RedirectToAction("Logout");

            string redirectAction = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase) ? "StaffProfile" : "Profile";

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin đổi mật khẩu chưa hợp lệ (mật khẩu mới tối thiểu 6 ký tự và phải khớp xác nhận).";
                return RedirectToAction(redirectAction);
            }

            if (!AppHelpers.VerifyPassword(model.CurrentPassword, user.Password))
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác.";
                return RedirectToAction(redirectAction);
            }

            user.Password = AppHelpers.HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(redirectAction);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        private void SignInUser(User user, bool isPersistent)
        {
            // UserData format: "UserId|Role|FullName|IsVip"
            string cleanName = (user.FullName ?? string.Empty).Replace("|", " ");
            string userData = string.Format("{0}|{1}|{2}|{3}",
                user.Id,
                user.Role,
                cleanName,
                user.IsVip ? "true" : "false");

            DateTime expiration = isPersistent ? DateTime.Now.AddDays(7) : DateTime.Now.AddHours(8);
            var ticket = new FormsAuthenticationTicket(
                1,
                user.Email,
                DateTime.Now,
                expiration,
                isPersistent,
                userData,
                FormsAuthentication.FormsCookiePath);

            string encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Path = FormsAuthentication.FormsCookiePath
            };
            if (isPersistent)
            {
                cookie.Expires = expiration;
            }

            Response.Cookies.Add(cookie);
        }

        private void MergeSessionCartToUser(int userId)
        {
            string sessionId = Session["CartSessionId"] as string;
            if (string.IsNullOrEmpty(sessionId))
                return;

            var guestCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.SessionId == sessionId && c.UserId == null);

            if (guestCart == null || !guestCart.CartItems.Any())
                return;

            var userCart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            if (userCart == null)
            {
                guestCart.UserId = userId;
                guestCart.UpdatedAt = DateTime.Now;
            }
            else
            {
                foreach (var item in guestCart.CartItems.ToList())
                {
                    var existing = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);
                    if (existing != null)
                    {
                        existing.Quantity += item.Quantity;
                    }
                    else
                    {
                        _context.CartItems.Add(new CartItem
                        {
                            CartId = userCart.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        });
                    }
                }
                userCart.UpdatedAt = DateTime.Now;
                _context.Carts.Remove(guestCart);
            }

            _context.SaveChanges();
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
