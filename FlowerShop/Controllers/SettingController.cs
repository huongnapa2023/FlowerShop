using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [AdminPermissionAuthorize(AdminPermissions.Banner)]
    public class SettingController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // GET: Setting
        public ActionResult Index()
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);

            var setting = _context.SiteSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new SiteSetting();
                _context.SiteSettings.Add(setting);
                _context.SaveChanges();
            }

            return View(setting);
        }

        // POST: Setting/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Save(SiteSetting model, HttpPostedFileBase headerLogoFile, HttpPostedFileBase footerLogoFile)
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);

            var setting = _context.SiteSettings.FirstOrDefault();
            if (setting == null)
            {
                setting = new SiteSetting();
                _context.SiteSettings.Add(setting);
            }

            if (headerLogoFile != null && headerLogoFile.ContentLength > 0)
            {
                if (AppHelpers.IsImageValid(headerLogoFile))
                {
                    setting.LogoUrl = AppHelpers.SaveUploadedImage(headerLogoFile, Server.MapPath("~/"), "branding");
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.LogoUrl))
            {
                setting.LogoUrl = model.LogoUrl.Trim();
            }

            if (footerLogoFile != null && footerLogoFile.ContentLength > 0)
            {
                if (AppHelpers.IsImageValid(footerLogoFile))
                {
                    setting.FooterLogoUrl = AppHelpers.SaveUploadedImage(footerLogoFile, Server.MapPath("~/"), "branding");
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.FooterLogoUrl))
            {
                setting.FooterLogoUrl = model.FooterLogoUrl.Trim();
            }

            // 1. Cập nhật Header & Topbar
            setting.ShopName = string.IsNullOrWhiteSpace(model.ShopName) ? "CHAM Flowers" : model.ShopName.Trim();
            setting.Hotline = string.IsNullOrWhiteSpace(model.Hotline) ? "1900 6868" : model.Hotline.Trim();
            setting.DefaultTopbarText = model.DefaultTopbarText ?? string.Empty;
            setting.TopbarBgColor = string.IsNullOrWhiteSpace(model.TopbarBgColor) ? "#881337" : model.TopbarBgColor.Trim();
            setting.HeaderBgColor = string.IsNullOrWhiteSpace(model.HeaderBgColor) ? "#ffffff" : model.HeaderBgColor.Trim();
            setting.ShowTopbar = model.ShowTopbar;
            setting.ShowSearchBar = model.ShowSearchBar;
            setting.ShowHotDealButton = model.ShowHotDealButton;

            // 2. Cập nhật Footer
            setting.FooterBgColor = string.IsNullOrWhiteSpace(model.FooterBgColor) ? "#0f172a" : model.FooterBgColor.Trim();
            setting.FooterTextColor = string.IsNullOrWhiteSpace(model.FooterTextColor) ? "#f8fafc" : model.FooterTextColor.Trim();
            setting.FooterAbout = model.FooterAbout ?? string.Empty;
            setting.Address = model.Address ?? string.Empty;
            setting.SecondaryPhone = model.SecondaryPhone ?? string.Empty;
            setting.Email = model.Email ?? string.Empty;
            setting.WorkingHours = model.WorkingHours ?? string.Empty;
            setting.FooterCol1Title = string.IsNullOrWhiteSpace(model.FooterCol1Title) ? "Danh mục hoa" : model.FooterCol1Title.Trim();
            setting.FooterCol1Links = model.FooterCol1Links ?? string.Empty;
            setting.FooterCol2Title = string.IsNullOrWhiteSpace(model.FooterCol2Title) ? "Hỗ trợ khách hàng" : model.FooterCol2Title.Trim();
            setting.FooterCol2Links = model.FooterCol2Links ?? string.Empty;
            setting.FacebookUrl = model.FacebookUrl?.Trim();
            setting.InstagramUrl = model.InstagramUrl?.Trim();
            setting.ZaloUrl = model.ZaloUrl?.Trim();
            setting.TikTokUrl = model.TikTokUrl?.Trim();
            setting.CopyrightText = string.IsNullOrWhiteSpace(model.CopyrightText) ? "© 2026 CHAM Flowers." : model.CopyrightText.Trim();
            setting.ShowFloatingChat = model.ShowFloatingChat;
            setting.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Đã lưu cấu hình Tuỳ biến Header & Footer thành công! Thay đổi đã có hiệu lực trên toàn bộ trang khách hàng.";
            return RedirectToAction("Index");
        }

        // POST: Setting/ResetDefault
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetDefault()
        {
            DbInitializer.EnsurePopupBuilderAndSiteSettings(_context);

            var setting = _context.SiteSettings.FirstOrDefault();
            var def = new SiteSetting();

            if (setting != null)
            {
                setting.ShopName = def.ShopName;
                setting.LogoUrl = def.LogoUrl;
                setting.Hotline = def.Hotline;
                setting.DefaultTopbarText = def.DefaultTopbarText;
                setting.TopbarBgColor = def.TopbarBgColor;
                setting.HeaderBgColor = def.HeaderBgColor;
                setting.ShowTopbar = def.ShowTopbar;
                setting.ShowSearchBar = def.ShowSearchBar;
                setting.ShowHotDealButton = def.ShowHotDealButton;
                setting.FooterLogoUrl = def.FooterLogoUrl;
                setting.FooterBgColor = def.FooterBgColor;
                setting.FooterTextColor = def.FooterTextColor;
                setting.FooterAbout = def.FooterAbout;
                setting.Address = def.Address;
                setting.SecondaryPhone = def.SecondaryPhone;
                setting.Email = def.Email;
                setting.WorkingHours = def.WorkingHours;
                setting.FooterCol1Title = def.FooterCol1Title;
                setting.FooterCol1Links = def.FooterCol1Links;
                setting.FooterCol2Title = def.FooterCol2Title;
                setting.FooterCol2Links = def.FooterCol2Links;
                setting.FacebookUrl = def.FacebookUrl;
                setting.InstagramUrl = def.InstagramUrl;
                setting.ZaloUrl = def.ZaloUrl;
                setting.TikTokUrl = def.TikTokUrl;
                setting.CopyrightText = def.CopyrightText;
                setting.ShowFloatingChat = def.ShowFloatingChat;
                setting.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }

            TempData["Success"] = "Đã khôi phục giao diện Header & Footer về mặc định ban đầu!";
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
