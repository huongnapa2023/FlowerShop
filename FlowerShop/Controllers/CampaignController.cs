using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Banner)]
    public class CampaignController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // GET: Campaign
        public ActionResult Index(string search, string season)
        {
            // Tự động nâng cấp ảnh Banner Hero chuẩn chủ đề hoa tươi nếu đang dùng link cũ
            try
            {
                _context.Database.ExecuteSqlCommand(@"
                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1587317996270-908c352f1b09?auto=format&fit=crop&w=1920&q=85'
                    WHERE ([SeasonType] = '20-11' OR [Name] LIKE N'%20/11%' OR [Name] LIKE N'%Tri Ân%')
                      AND ([BannerUrl] LIKE '%photo-1508615039623%' OR [BannerUrl] IS NULL);

                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1576895225168-a13e88ce5fae?auto=format&fit=crop&w=1920&q=85'
                    WHERE ([SeasonType] = 'Noel' OR [Name] LIKE N'%Giáng Sinh%' OR [Name] LIKE N'%Noel%')
                      AND ([BannerUrl] LIKE '%photo-1543257580%' OR [BannerUrl] IS NULL);

                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1571446914179-df4ab952de19?auto=format&fit=crop&w=1920&q=85'
                    WHERE ([SeasonType] = 'Tet' OR [Name] LIKE N'%Tết%' OR [Name] LIKE N'%Xuân%')
                      AND ([BannerUrl] LIKE '%photo-1518895949%' OR [BannerUrl] IS NULL);

                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1563436798345-8c9b4b97b8d0?auto=format&fit=crop&w=1920&q=85'
                    WHERE ([SeasonType] = 'Valentine' OR [Name] LIKE N'%Valentine%' OR [Name] LIKE N'%Yêu%')
                      AND ([BannerUrl] LIKE '%photo-1518709268%' OR [BannerUrl] IS NULL);
                ");
            }
            catch { }

            var query = _context.Campaigns.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(c => c.Name.Contains(search) || 
                                         c.PromoBarText.Contains(search) || 
                                         c.CouponCode.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(season) && season != "All")
            {
                query = query.Where(c => c.SeasonType == season);
            }

            var allCampaigns = query.OrderByDescending(c => c.StartDate).ToList();

            var now = DateTime.Now;
            ViewBag.Search = search;
            ViewBag.Season = season;
            ViewBag.TotalCount = _context.Campaigns.Count();
            ViewBag.ActiveCount = _context.Campaigns.Count(c => c.IsActive && now >= c.StartDate && now <= c.EndDate);
            ViewBag.UpcomingCount = _context.Campaigns.Count(c => c.IsActive && now < c.StartDate);
            ViewBag.ExpiredCount = _context.Campaigns.Count(c => now > c.EndDate);

            return View(allCampaigns);
        }

        // GET: Campaign/Create
        public ActionResult Create()
        {
            var model = new Campaign
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(14),
                ThemeColor = "#be123c",
                IsActive = true,
                SeasonType = "Other"
            };

            return View(model);
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Campaign model, HttpPostedFileBase bannerImage)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên chiến dịch.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải diễn ra sau hoặc bằng ngày bắt đầu.");
            }

            if (bannerImage != null && bannerImage.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(bannerImage))
                {
                    ModelState.AddModelError("BannerUrl", "Ảnh banner không đúng định dạng cho phép (.jpg, .png, .jpeg, .webp).");
                }
                else
                {
                    model.BannerUrl = AppHelpers.SaveUploadedImage(bannerImage, Server.MapPath("~/"), "campaigns");
                }
            }

            if (ModelState.IsValid)
            {
                model.Name = model.Name.Trim();
                if (!string.IsNullOrWhiteSpace(model.PromoBarText)) model.PromoBarText = model.PromoBarText.Trim();
                if (!string.IsNullOrWhiteSpace(model.CouponCode)) model.CouponCode = model.CouponCode.Trim().ToUpperInvariant();
                if (!string.IsNullOrWhiteSpace(model.BannerTitle)) model.BannerTitle = model.BannerTitle.Trim();
                if (!string.IsNullOrWhiteSpace(model.BannerSubtitle)) model.BannerSubtitle = model.BannerSubtitle.Trim();
                if (!string.IsNullOrWhiteSpace(model.TargetUrl)) model.TargetUrl = model.TargetUrl.Trim();
                if (string.IsNullOrWhiteSpace(model.ThemeColor)) model.ThemeColor = "#be123c";

                model.CreatedAt = DateTime.Now;
                _context.Campaigns.Add(model);
                _context.SaveChanges();

                TempData["Success"] = $"Đã khởi tạo thành công chiến dịch mùa lễ: \"{model.Name}\"!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Campaign/Edit/5
        public ActionResult Edit(int id)
        {
            var campaign = _context.Campaigns.Find(id);
            if (campaign == null)
            {
                TempData["Error"] = "Không tìm thấy chiến dịch yêu cầu.";
                return RedirectToAction("Index");
            }

            return View(campaign);
        }

        // POST: Campaign/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Campaign model, HttpPostedFileBase bannerImage)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên chiến dịch.");
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải diễn ra sau hoặc bằng ngày bắt đầu.");
            }

            var existing = _context.Campaigns.Find(model.Id);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy chiến dịch cần chỉnh sửa.";
                return RedirectToAction("Index");
            }

            if (bannerImage != null && bannerImage.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(bannerImage))
                {
                    ModelState.AddModelError("BannerUrl", "Ảnh banner không đúng định dạng hợp lệ.");
                }
                else
                {
                    existing.BannerUrl = AppHelpers.SaveUploadedImage(bannerImage, Server.MapPath("~/"), "campaigns");
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.BannerUrl))
            {
                existing.BannerUrl = model.BannerUrl.Trim();
            }

            if (ModelState.IsValid)
            {
                existing.Name = model.Name.Trim();
                existing.SeasonType = model.SeasonType;
                existing.PromoBarText = model.PromoBarText?.Trim();
                existing.BannerTitle = model.BannerTitle?.Trim();
                existing.BannerSubtitle = model.BannerSubtitle?.Trim();
                existing.TargetUrl = model.TargetUrl?.Trim();
                existing.CouponCode = model.CouponCode?.Trim().ToUpperInvariant();
                existing.DiscountPercent = model.DiscountPercent;
                existing.ThemeColor = string.IsNullOrWhiteSpace(model.ThemeColor) ? "#be123c" : model.ThemeColor.Trim();
                existing.StartDate = model.StartDate;
                existing.EndDate = model.EndDate;
                existing.IsActive = model.IsActive;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                TempData["Success"] = $"Đã cập nhật chiến dịch \"{existing.Name}\" thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Campaign/ToggleActive/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleActive(int id)
        {
            var campaign = _context.Campaigns.Find(id);
            if (campaign == null)
            {
                TempData["Error"] = "Không tìm thấy chiến dịch.";
                return RedirectToAction("Index");
            }

            campaign.IsActive = !campaign.IsActive;
            campaign.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            string statusMsg = campaign.IsActive ? "kích hoạt" : "tạm dừng";
            TempData["Success"] = $"Đã {statusMsg} chiến dịch \"{campaign.Name}\".";
            return RedirectToAction("Index");
        }

        // POST: Campaign/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var campaign = _context.Campaigns.Find(id);
            if (campaign == null)
            {
                TempData["Error"] = "Không tìm thấy chiến dịch để xóa.";
                return RedirectToAction("Index");
            }

            string name = campaign.Name;
            _context.Campaigns.Remove(campaign);
            _context.SaveChanges();

            TempData["Success"] = $"Đã xóa chiến dịch \"{name}\" thành công!";
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
