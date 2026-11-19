using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index()
        {
            var banners = _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ThenByDescending(b => b.CreatedAt)
                .ToList();

            // Tự động kiểm tra và chèn Banner Chiến dịch / Mùa lễ hội vào vị trí đầu tiên nếu đang diễn ra (IsActiveNow)
            var now = DateTime.Now;
            var activeCamp = _context.Campaigns.AsNoTracking()
                .Where(c => c.IsActive && now >= c.StartDate && now <= c.EndDate)
                .OrderByDescending(c => c.StartDate)
                .FirstOrDefault();

            if (activeCamp != null && !string.IsNullOrWhiteSpace(activeCamp.BannerUrl))
            {
                var campBanner = new Banner
                {
                    Id = -activeCamp.Id,
                    Title = string.IsNullOrWhiteSpace(activeCamp.BannerTitle) ? activeCamp.Name : activeCamp.BannerTitle,
                    Subtitle = string.IsNullOrWhiteSpace(activeCamp.BannerSubtitle)
                        ? (string.IsNullOrWhiteSpace(activeCamp.PromoBarText) ? "Ưu đãi mùa lễ hội đặc biệt tại CHAM Flowers" : activeCamp.PromoBarText)
                        : activeCamp.BannerSubtitle,
                    ImageUrl = activeCamp.BannerUrl,
                    LinkUrl = string.IsNullOrWhiteSpace(activeCamp.TargetUrl) ? "/Product" : activeCamp.TargetUrl,
                    ButtonText = "Khám phá BST",
                    DisplayOrder = 0,
                    IsActive = true
                };
                banners.Insert(0, campBanner);
            }

            var flowerCategories = _context.FlowerCategories
                .OrderBy(fc => fc.Id)
                .ToList();

            var categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .ToList();

            var featured = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToList();

            var hotDeals = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && p.IsHotDeal)
                .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .Take(4)
                .ToList();

            var newArrivals = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToList();

            var news = _context.NewsArticles
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .Take(3)
                .ToList();

            var popups = _context.HotDealPopups
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var activePopup = popups.FirstOrDefault(p => p.IsActiveNow);

            var coupons = _context.Coupons
                .Where(c => c.IsActive && (!c.ExpiryDate.HasValue || c.ExpiryDate >= now))
                .ToList();

            var vm = new HomeViewModel
            {
                Banners = banners,
                FlowerCategories = flowerCategories,
                Categories = categories,
                FeaturedProducts = featured,
                HotDealProducts = hotDeals,
                NewArrivals = newArrivals,
                LatestNews = news,
                ActivePopup = activePopup,
                ActiveCoupons = coupons
            };

            return View(vm);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult News()
        {
            var articles = _context.NewsArticles
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return View(articles);
        }

        public ActionResult NewsDetail(int id)
        {
            var article = _context.NewsArticles.FirstOrDefault(n => n.Id == id && n.IsPublished);
            if (article == null)
                return HttpNotFound();

            ViewBag.RelatedNews = _context.NewsArticles
                .Where(n => n.IsPublished && n.Id != id)
                .OrderByDescending(n => n.CreatedAt)
                .Take(3)
                .ToList();

            return View(article);
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
