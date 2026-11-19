using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Banner)]
    public class BannerController : Controller
    {
        private readonly FlowerShopDbContext db = new FlowerShopDbContext();

        public ActionResult Index(string search)
        {
            var banners = db.Banners.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                banners = banners.Where(b => b.Title.Contains(search));
            }

            banners = banners.OrderByDescending(b => b.CreatedAt);

            ViewBag.Search = search;
            ViewBag.NewsList = db.NewsArticles.OrderByDescending(n => n.CreatedAt).ToList();
            ViewBag.Coupons = db.Coupons.OrderByDescending(c => c.Id).ToList();

            return View(banners.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBanner(Banner model, HttpPostedFileBase imageFile)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                TempData["Error"] = "Tiêu đề banner không được để trống.";
                return RedirectToAction("Index");
            }

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(imageFile))
                {
                    TempData["Error"] = "Chỉ cho phép file ảnh jpg, png, jpeg, gif, webp, svg.";
                    return RedirectToAction("Index");
                }
                model.ImageUrl = AppHelpers.SaveUploadedImage(imageFile, Server.MapPath("~/"), "banners");
            }

            if (model.Id == 0)
            {
                if (string.IsNullOrWhiteSpace(model.ImageUrl))
                    model.ImageUrl = "/images/samflora/editorial-2.jpg";
                model.CreatedAt = DateTime.Now;
                db.Banners.Add(model);
                TempData["Success"] = "Đã thêm banner mới thành công!";
            }
            else
            {
                var banner = db.Banners.Find(model.Id);
                if (banner != null)
                {
                    banner.Title = model.Title.Trim();
                    banner.Subtitle = model.Subtitle != null ? model.Subtitle.Trim() : null;
                    banner.LinkUrl = model.LinkUrl != null ? model.LinkUrl.Trim() : null;
                    banner.ButtonText = model.ButtonText != null ? model.ButtonText.Trim() : null;
                    banner.DisplayOrder = model.DisplayOrder;
                    banner.IsActive = model.IsActive;
                    if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                        banner.ImageUrl = model.ImageUrl.Trim();
                    TempData["Success"] = "Đã cập nhật banner thành công!";
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBanner(int id)
        {
            var banner = db.Banners.Find(id);
            if (banner != null)
            {
                db.Banners.Remove(banner);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa banner quảng cáo.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SaveNews(NewsArticle model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || string.IsNullOrWhiteSpace(model.Content))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ tiêu đề và nội dung bài viết.";
                return RedirectToAction("Index");
            }

            if (model.Id == 0)
            {
                model.Slug = AppHelpers.GenerateSlug(model.Title);
                model.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? "/images/samflora/editorial-2.jpg" : model.ImageUrl.Trim();
                model.CreatedAt = DateTime.Now;
                db.NewsArticles.Add(model);
                TempData["Success"] = "Đã đăng bài viết tin tức mới!";
            }
            else
            {
                var existing = db.NewsArticles.Find(model.Id);
                if (existing != null)
                {
                    existing.Title = model.Title.Trim();
                    existing.Slug = AppHelpers.GenerateSlug(model.Title);
                    existing.CategoryName = model.CategoryName;
                    existing.Summary = model.Summary;
                    existing.Content = model.Content;
                    if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                        existing.ImageUrl = model.ImageUrl.Trim();
                    existing.IsPublished = model.IsPublished;
                    TempData["Success"] = "Đã cập nhật bài viết tin tức!";
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNews(int id)
        {
            var art = db.NewsArticles.Find(id);
            if (art != null)
            {
                db.NewsArticles.Remove(art);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa bài viết tin tức.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCoupon(Coupon model)
        {
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                TempData["Error"] = "Vui lòng nhập mã giảm giá.";
                return RedirectToAction("Index");
            }

            model.Code = model.Code.Trim().ToUpperInvariant();
            if (model.Id == 0)
            {
                db.Coupons.Add(model);
                TempData["Success"] = string.Format("Đã tạo mã khuyến mãi {0}!", model.Code);
            }
            else
            {
                var c = db.Coupons.Find(model.Id);
                if (c != null)
                {
                    c.Code = model.Code;
                    c.Description = model.Description;
                    c.DiscountPercent = model.DiscountPercent;
                    c.MaxDiscountAmount = model.MaxDiscountAmount;
                    c.MinOrderAmount = model.MinOrderAmount;
                    c.VipOnly = model.VipOnly;
                    c.ExpiryDate = model.ExpiryDate;
                    c.IsActive = model.IsActive;
                    TempData["Success"] = string.Format("Đã cập nhật mã khuyến mãi {0}!", c.Code);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCoupon(int id)
        {
            var c = db.Coupons.Find(id);
            if (c != null)
            {
                db.Coupons.Remove(c);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa mã giảm giá.";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
