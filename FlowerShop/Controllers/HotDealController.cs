using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    public class HotDealController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index(string search, int? categoryId, string sortOrder, int page = 1)
        {
            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsHotDeal && p.IsActive);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    (p.Description != null && p.Description.ToLower().Contains(kw)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                default:
                    products = products.OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt);
                    break;
            }

            ViewBag.Categories = _context.Categories.Where(c => c.IsActive).ToList();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortOrder = sortOrder;

            var popups = _context.HotDealPopups.OrderByDescending(p => p.CreatedAt).ToList();
            var activePopup = popups.FirstOrDefault(p => p.IsActiveNow);
            ViewBag.EndDate = activePopup != null && activePopup.EndDate.HasValue ? activePopup.EndDate.Value : DateTime.Now.AddDays(5);
            ViewBag.ActivePopup = activePopup;

            return View(products.ToPagedList(page, 8));
        }

        [HttpGet]
        public ActionResult GetActivePopup()
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);

            var popups = _context.HotDealPopups
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var active = popups.FirstOrDefault(p => p.IsActiveNow);
            if (active == null)
            {
                return Json(new { hasPopup = false }, JsonRequestBehavior.AllowGet);
            }

            string finalUrl = active.ProductId.HasValue
                ? string.Format("/Product/Details/{0}", active.ProductId.Value)
                : (!string.IsNullOrWhiteSpace(active.TargetUrl) ? active.TargetUrl : "/HotDeal");

            return Json(new
            {
                hasPopup = true,
                id = active.Id,
                title = active.Title ?? "Ưu đãi Hot Deal Hoa Tươi!",
                subtitle = active.Subtitle,
                imageUrl = active.ImageUrl,
                couponCode = active.CouponCode,
                discountPercent = active.DiscountPercent,
                productUrl = finalUrl,
                layoutType = string.IsNullOrWhiteSpace(active.LayoutType) ? "TopImage" : active.LayoutType,
                badgeText = string.IsNullOrWhiteSpace(active.BadgeText) ? "FLASH SALE HÔM NAY" : active.BadgeText,
                buttonText = string.IsNullOrWhiteSpace(active.ButtonText) ? "Xem ngay chương trình khuyến mãi" : active.ButtonText,
                backgroundColor = string.IsNullOrWhiteSpace(active.BackgroundColor) ? "#ffffff" : active.BackgroundColor,
                textColor = string.IsNullOrWhiteSpace(active.TextColor) ? "#1e293b" : active.TextColor,
                accentColor = string.IsNullOrWhiteSpace(active.AccentColor) ? "#be123c" : active.AccentColor,
                delaySeconds = active.DelaySeconds,
                showFrequency = string.IsNullOrWhiteSpace(active.ShowFrequency) ? "OncePerSession" : active.ShowFrequency,
                endDate = active.EndDate.HasValue ? active.EndDate.Value.ToString("yyyy-MM-ddTHH:mm:ss") : null
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetHotDealProducts()
        {
            var items = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsHotDeal && p.IsActive)
                .Take(6)
                .ToList();

            var result = items.Select(p => new
            {
                id = p.Id,
                name = p.Name,
                category = p.Category != null ? p.Category.Name : null,
                price = p.Price,
                originalPrice = p.OriginalPrice,
                discountPercent = p.DiscountPercent,
                imageUrl = p.MainImageUrl
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // ==================== QUẢN LÝ HOT DEAL & POPUP (ADMIN) ====================

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        public ActionResult Manage()
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);

            ViewBag.Popups = _context.HotDealPopups
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.HotDealProducts = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsHotDeal)
                .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .ToList();

            ViewBag.AllProducts = _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToList();

            return View();
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpGet]
        public ActionResult CreateHotDealPopup()
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);
            ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name");
            return View(new HotDealPopup
            {
                IsActive = true,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                DiscountPercent = 20,
                CouponCode = "CHAMFLOWERS15",
                ImageUrl = "https://images.unsplash.com/photo-1561181286-d3fee7d55364?auto=format&fit=crop&w=1000&q=85",
                LayoutType = "TopImage",
                BadgeText = "FLASH SALE HÔM NAY",
                ButtonText = "Xem ngay chương trình khuyến mãi",
                TargetUrl = "/HotDeal",
                BackgroundColor = "#ffffff",
                TextColor = "#1e293b",
                AccentColor = "#be123c",
                DelaySeconds = 1,
                ShowFrequency = "OncePerSession"
            });
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHotDealPopup(HotDealPopup model, HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(imageFile))
                {
                    ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif, webp");
                    ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name", model.ProductId);
                    return View(model);
                }

                model.ImageUrl = AppHelpers.SaveUploadedImage(imageFile, Server.MapPath("~/"), "hotdeals");
            }
            else if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                model.ImageUrl = "/images/samflora/b12-main.jpg";
            }

            if (model.StartDate.HasValue && model.EndDate.HasValue && model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Thời gian kết thúc phải sau thời gian bắt đầu.");
                ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name", model.ProductId);
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.LayoutType)) model.LayoutType = "TopImage";
            if (string.IsNullOrWhiteSpace(model.BadgeText)) model.BadgeText = "FLASH SALE HÔM NAY";
            if (string.IsNullOrWhiteSpace(model.ButtonText)) model.ButtonText = "Xem ngay chương trình khuyến mãi";
            if (string.IsNullOrWhiteSpace(model.TargetUrl)) model.TargetUrl = "/HotDeal";
            if (string.IsNullOrWhiteSpace(model.BackgroundColor)) model.BackgroundColor = "#ffffff";
            if (string.IsNullOrWhiteSpace(model.TextColor)) model.TextColor = "#1e293b";
            if (string.IsNullOrWhiteSpace(model.AccentColor)) model.AccentColor = "#be123c";
            if (string.IsNullOrWhiteSpace(model.ShowFrequency)) model.ShowFrequency = "OncePerSession";

            model.CreatedAt = DateTime.Now;
            _context.HotDealPopups.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đã thiết kế và lưu chương trình Popup thành công!";
            return RedirectToAction("Manage");
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpGet]
        public ActionResult EditHotDealPopup(int id)
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);
            var popup = _context.HotDealPopups.Find(id);
            if (popup == null)
                return HttpNotFound();

            ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name", popup.ProductId);
            return View(popup);
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHotDealPopup(int id, HotDealPopup model, HttpPostedFileBase imageFile)
        {
            var existing = _context.HotDealPopups.Find(id);
            if (existing == null)
                return HttpNotFound();

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(imageFile))
                {
                    ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif, webp");
                    ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name", model.ProductId);
                    return View(model);
                }

                existing.ImageUrl = AppHelpers.SaveUploadedImage(imageFile, Server.MapPath("~/"), "hotdeals");
            }
            else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                existing.ImageUrl = model.ImageUrl.Trim();
            }

            existing.Title = model.Title;
            existing.Subtitle = model.Subtitle;
            existing.ProductId = model.ProductId;
            existing.DiscountPercent = model.DiscountPercent;
            existing.CouponCode = model.CouponCode;
            existing.LayoutType = string.IsNullOrWhiteSpace(model.LayoutType) ? "TopImage" : model.LayoutType;
            existing.BadgeText = string.IsNullOrWhiteSpace(model.BadgeText) ? "FLASH SALE HÔM NAY" : model.BadgeText;
            existing.ButtonText = string.IsNullOrWhiteSpace(model.ButtonText) ? "Xem ngay chương trình khuyến mãi" : model.ButtonText;
            existing.TargetUrl = string.IsNullOrWhiteSpace(model.TargetUrl) ? "/HotDeal" : model.TargetUrl;
            existing.BackgroundColor = string.IsNullOrWhiteSpace(model.BackgroundColor) ? "#ffffff" : model.BackgroundColor;
            existing.TextColor = string.IsNullOrWhiteSpace(model.TextColor) ? "#1e293b" : model.TextColor;
            existing.AccentColor = string.IsNullOrWhiteSpace(model.AccentColor) ? "#be123c" : model.AccentColor;
            existing.DelaySeconds = model.DelaySeconds < 0 ? 0 : model.DelaySeconds;
            existing.ShowFrequency = string.IsNullOrWhiteSpace(model.ShowFrequency) ? "OncePerSession" : model.ShowFrequency;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.IsActive = model.IsActive;

            _context.SaveChanges();
            TempData["Success"] = "Đã cập nhật thiết kế Popup thành công!";
            return RedirectToAction("Manage");
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TogglePopupActive(int id)
        {
            var popup = _context.HotDealPopups.Find(id);
            if (popup != null)
            {
                popup.IsActive = !popup.IsActive;
                _context.SaveChanges();
                TempData["Success"] = popup.IsActive
                    ? "Đã bật hiển thị Popup khuyến mãi."
                    : "Đã tạm tắt Popup khuyến mãi.";
            }
            return RedirectToAction("Manage");
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHotDealPopup(int id)
        {
            var popup = _context.HotDealPopups.Find(id);
            if (popup != null)
            {
                _context.HotDealPopups.Remove(popup);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa chương trình Hot Deal Popup.";
            }
            return RedirectToAction("Manage");
        }

        [AdminPermissionAuthorize(AdminPermissions.HotDeal)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetProductHotDeal(int productId, bool isHotDeal, decimal? salePrice)
        {
            var prod = _context.Products.Find(productId);
            if (prod != null)
            {
                if (isHotDeal && salePrice.HasValue && salePrice.Value > 0)
                {
                    if (!prod.OriginalPrice.HasValue || prod.OriginalPrice < prod.Price)
                    {
                        prod.OriginalPrice = prod.Price;
                    }
                    prod.Price = salePrice.Value;
                }
                prod.IsHotDeal = isHotDeal;
                prod.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["Success"] = isHotDeal
                    ? string.Format("Đã đưa \"{0}\" vào danh sách Hot Deal!", prod.Name)
                    : string.Format("Đã gỡ \"{0}\" khỏi danh sách Hot Deal.", prod.Name);
            }
            return RedirectToAction("Manage");
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
