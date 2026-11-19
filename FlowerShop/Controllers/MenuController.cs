using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Banner)]
    public class MenuController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // GET: Menu (Admin Dashboard danh sách Menu)
        public ActionResult Index()
        {
            var items = _context.MenuItems
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Id)
                .ToList();

            return View(items);
        }

        // GET: Menu/Create
        public ActionResult Create()
        {
            // Tự tính thứ tự tiếp theo
            int maxOrder = _context.MenuItems.Any() ? _context.MenuItems.Max(m => m.DisplayOrder) : 0;
            var model = new MenuItem
            {
                DisplayOrder = maxOrder + 1,
                IsActive = true,
                OpenNewTab = false,
                BadgeColor = "danger"
            };

            return View(model);
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuItem model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên hiển thị của menu.");
            }

            if (string.IsNullOrWhiteSpace(model.Url))
            {
                ModelState.AddModelError("Url", "Vui lòng nhập đường dẫn URL.");
            }

            if (ModelState.IsValid)
            {
                model.Name = model.Name.Trim();
                model.Url = model.Url.Trim();
                if (!string.IsNullOrEmpty(model.Icon)) model.Icon = model.Icon.Trim();
                if (!string.IsNullOrEmpty(model.BadgeText)) model.BadgeText = model.BadgeText.Trim();
                model.CreatedAt = DateTime.Now;

                _context.MenuItems.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Đã thêm mục menu mới thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Menu/Edit/5
        public ActionResult Edit(int id)
        {
            var item = _context.MenuItems.Find(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy mục menu.";
                return RedirectToAction("Index");
            }

            return View(item);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MenuItem model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên hiển thị của menu.");
            }

            if (string.IsNullOrWhiteSpace(model.Url))
            {
                ModelState.AddModelError("Url", "Vui lòng nhập đường dẫn URL.");
            }

            if (ModelState.IsValid)
            {
                var existing = _context.MenuItems.Find(model.Id);
                if (existing == null)
                {
                    TempData["Error"] = "Không tìm thấy mục menu cần cập nhật.";
                    return RedirectToAction("Index");
                }

                existing.Name = model.Name.Trim();
                existing.Url = model.Url.Trim();
                existing.Icon = string.IsNullOrEmpty(model.Icon) ? null : model.Icon.Trim();
                existing.BadgeText = string.IsNullOrEmpty(model.BadgeText) ? null : model.BadgeText.Trim();
                existing.BadgeColor = string.IsNullOrEmpty(model.BadgeColor) ? "danger" : model.BadgeColor.Trim();
                existing.DisplayOrder = model.DisplayOrder;
                existing.OpenNewTab = model.OpenNewTab;
                existing.IsActive = model.IsActive;

                _context.SaveChanges();

                TempData["Success"] = "Đã cập nhật mục menu thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var item = _context.MenuItems.Find(id);
            if (item != null)
            {
                _context.MenuItems.Remove(item);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa mục menu thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy mục menu cần xóa.";
            }

            return RedirectToAction("Index");
        }

        // POST: Menu/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            var item = _context.MenuItems.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _context.SaveChanges();
                TempData["Success"] = item.IsActive
                    ? $"Đã bật hiển thị menu '{item.Name}'."
                    : $"Đã ẩn menu '{item.Name}'.";
            }

            return RedirectToAction("Index");
        }

        // POST: Menu/Move
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Move(int id, string direction)
        {
            var items = _context.MenuItems.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Id).ToList();
            int index = items.FindIndex(m => m.Id == id);

            if (index >= 0)
            {
                if (direction == "up" && index > 0)
                {
                    int temp = items[index].DisplayOrder;
                    items[index].DisplayOrder = items[index - 1].DisplayOrder;
                    items[index - 1].DisplayOrder = temp;

                    // Nếu bằng nhau thì đảm bảo chênh lệch
                    if (items[index].DisplayOrder == items[index - 1].DisplayOrder)
                    {
                        items[index].DisplayOrder = items[index - 1].DisplayOrder + 1;
                    }
                    _context.SaveChanges();
                }
                else if (direction == "down" && index < items.Count - 1)
                {
                    int temp = items[index].DisplayOrder;
                    items[index].DisplayOrder = items[index + 1].DisplayOrder;
                    items[index + 1].DisplayOrder = temp;

                    if (items[index].DisplayOrder == items[index + 1].DisplayOrder)
                    {
                        items[index + 1].DisplayOrder = items[index].DisplayOrder + 1;
                    }
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        // Child action phục vụ hiển thị thanh menu động trên layout người dùng
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult NavMenu()
        {
            List<MenuItem> items;
            try
            {
                items = _context.MenuItems
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.DisplayOrder)
                    .ThenBy(m => m.Id)
                    .ToList();
            }
            catch
            {
                items = new List<MenuItem>();
            }

            // Nếu CSDL chưa có mục menu nào, trả về danh sách mặc định an toàn
            if (items == null || !items.Any())
            {
                items = new List<MenuItem>
                {
                    new MenuItem { Name = "Trang chủ", Url = "/", Icon = "bi-house-door", DisplayOrder = 1, IsActive = true },
                    new MenuItem { Name = "Sản phẩm hoa", Url = "/Product", Icon = "bi-flower1", DisplayOrder = 2, IsActive = true },
                    new MenuItem { Name = "Hot Deal", Url = "/HotDeal", Icon = "bi-fire", BadgeText = "HOT", BadgeColor = "danger", DisplayOrder = 3, IsActive = true },
                    new MenuItem { Name = "Tin tức & Cẩm nang", Url = "/Home/News", Icon = "bi-newspaper", DisplayOrder = 4, IsActive = true },
                    new MenuItem { Name = "Giới thiệu", Url = "/Home/About", Icon = "bi-info-circle", DisplayOrder = 5, IsActive = true }
                };
            }

            return PartialView("_NavMenu", items);
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
