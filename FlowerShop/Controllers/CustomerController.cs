using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Customer)]
    public class CustomerController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index(string search, string filter, int page = 1)
        {
            DbInitializer.EnsureSeedCustomers(_context);

            // BẮT BUỘC: Chỉ lấy tài khoản Khách hàng (Role == "Customer")
            // Tuyệt đối tách biệt, không để lẫn Nhân viên & Quản trị viên vào danh sách Khách hàng
            var query = _context.Users.Where(u => u.Role == "Customer").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(kw) ||
                    u.Email.ToLower().Contains(kw) ||
                    (u.Phone != null && u.Phone.Contains(kw)));
            }

            switch (filter)
            {
                case "vip":
                    query = query.Where(u => u.IsVip);
                    break;
                case "regular":
                    query = query.Where(u => !u.IsVip);
                    break;
                case "locked":
                    query = query.Where(u => !u.IsActive);
                    break;
                case "active":
                    query = query.Where(u => u.IsActive);
                    break;
            }

            ViewBag.Search = search;
            ViewBag.Filter = filter;

            int totalCustomers = _context.Users.Count(u => u.Role == "Customer");
            int vipCustomers = _context.Users.Count(u => u.Role == "Customer" && u.IsVip);
            int lockedCustomers = _context.Users.Count(u => u.Role == "Customer" && !u.IsActive);
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.VipCustomers = vipCustomers;
            ViewBag.LockedCustomers = lockedCustomers;

            var paged = query.OrderByDescending(u => u.CreatedAt).ToPagedList(page, 10);
            return View(paged);
        }

        public ActionResult Details(int id)
        {
            var user = _context.Users
                .Include("Orders.OrderItems")
                .Include("Reviews.Product")
                .FirstOrDefault(u => u.Id == id && u.Role == "Customer");

            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ khách hàng hợp lệ.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new User { Role = "Customer", IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User model)
        {
            if (string.IsNullOrWhiteSpace(model.Phone))
            {
                ModelState.AddModelError("Phone", "Vui lòng nhập số điện thoại khách hàng.");
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                string cleanEmail = model.Email.Trim().ToLowerInvariant();
                bool exists = _context.Users.Any(u => u.Email.ToLower() == cleanEmail);
                if (exists)
                {
                    ModelState.AddModelError("Email", "Email này đã tồn tại trong hệ thống.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.FullName = model.FullName.Trim();
            model.Full_name = model.FullName;
            model.Email = model.Email.Trim().ToLowerInvariant();
            model.Phone = model.Phone.Trim();
            model.Address = model.Address.Trim();
            model.Password = AppHelpers.HashPassword(model.Password);
            model.CreatedAt = DateTime.Now;

            _context.Users.Add(model);
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã thêm tài khoản \"{0}\" thành công!", model.FullName);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Customer");
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng cần chỉnh sửa hoặc tài khoản không phải là khách hàng.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string fullName, string phone, string address, bool isVip = false, bool isActive = true, string newPassword = null)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Customer");
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ khách hàng.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError("", "Họ tên, số điện thoại và địa chỉ của khách hàng không được để trống.");
                return View(user);
            }

            user.FullName = fullName.Trim();
            user.Full_name = fullName.Trim();
            user.Phone = phone.Trim();
            user.Address = address.Trim();
            user.Role = "Customer"; // Cố định luôn là Customer
            user.IsVip = isVip;
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length >= 6)
            {
                user.Password = AppHelpers.HashPassword(newPassword);
            }

            _context.SaveChanges();
            TempData["Success"] = string.Format("Cập nhật thông tin khách hàng \"{0}\" thành công!", user.FullName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleLock(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Customer");
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy khách hàng hoặc tài khoản này là nhân sự nội bộ (vui lòng quản lý tại mục Phân quyền).";
                return RedirectToAction("Index");
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = user.IsActive
                ? string.Format("Đã mở khóa tài khoản khách hàng \"{0}\".", user.FullName)
                : string.Format("Đã khóa tài khoản khách hàng \"{0}\".", user.FullName);

            return RedirectToAction("Index");
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
