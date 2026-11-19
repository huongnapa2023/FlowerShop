using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<FlowerShopDbContext>(null);
            try
            {
                using (var db = new FlowerShopDbContext())
                {
                    string webRoot = HostingEnvironment.MapPath("~/");
                    DbInitializer.Initialize(db, webRoot);
                }
            }
            catch
            {
                // Bỏ qua nếu DB đã tồn tại và sẵn sàng
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                return;

            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || ticket.Expired)
                    return;

                // UserData format: "UserId|Role|FullName|IsVip"
                string[] parts = (ticket.UserData ?? "").Split('|');
                int userId = 0;
                if (parts.Length > 0) int.TryParse(parts[0], out userId);
                string role = parts.Length > 1 && !string.IsNullOrEmpty(parts[1]) ? parts[1] : "Customer";
                string fullName = parts.Length > 2 && !string.IsNullOrEmpty(parts[2]) ? parts[2] : ticket.Name;
                bool isVip = parts.Length > 3 && string.Equals(parts[3], "true", StringComparison.OrdinalIgnoreCase);

                bool isSuperAdmin = false;
                string allowedPermissions = string.Empty;
                string branchName = string.Empty;

                // Với tài khoản Admin: Truy vấn trực tiếp từ CSDL theo UserId để cập nhật quyền theo thời gian thực (Real-time RBAC)
                // giúp Admin con nhận ngay quyền mới khi Admin Tổng vừa chỉnh sửa mà KHÔNG cần đăng xuất.
                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) && userId > 0)
                {
                    try
                    {
                        using (var db = new FlowerShopDbContext())
                        {
                            var adminInfo = db.Users.AsNoTracking()
                                .Where(u => u.Id == userId)
                                .Select(u => new { u.IsSuperAdmin, u.AllowedPermissions, u.Address, u.FullName })
                                .FirstOrDefault();

                            if (adminInfo != null)
                            {
                                isSuperAdmin = adminInfo.IsSuperAdmin;
                                allowedPermissions = adminInfo.AllowedPermissions;
                                branchName = adminInfo.Address;
                                if (!string.IsNullOrWhiteSpace(adminInfo.FullName))
                                {
                                    fullName = adminInfo.FullName;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Fallback an toàn nếu có gián đoạn kết nối tạm thời
                    }
                }

                var identity = new GenericIdentity(fullName, "Forms");
                var principal = new ChamPrincipal(identity, role, userId, ticket.Name, isVip, isSuperAdmin, allowedPermissions, branchName);

                HttpContext.Current.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }
            catch
            {
                // Ignore corrupted cookie
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // Redirect unauthenticated HTML requests trying to access [Authorize(Roles="Admin")] when already logged in as Customer
            if (Response.StatusCode == 302 && User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                string loc = Response.RedirectLocation ?? "";
                if (loc.IndexOf("/Account/Login", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Response.Redirect("~/Account/AccessDenied", true);
                }
            }
        }
    }
}
