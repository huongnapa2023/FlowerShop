using System;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;

namespace FlowerShop.Services
{
    /// <summary>
    /// Bộ lọc phân quyền động cho từng tác vụ của Quản trị viên CHAM Flowers.
    /// - Chưa đăng nhập -> Chuyển về /Account/Login
    /// - Đã đăng nhập nhưng không phải Admin -> Chuyển về /Account/AccessDenied
    /// - Là Admin:
    ///     + Nếu là Admin Tổng (IsSuperAdmin == true) -> Luôn được phép truy cập
    ///     + Nếu là Admin con -> Kiểm tra mã quyền được cấp (PermissionCode)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AdminPermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public string PermissionCode { get; set; }

        public AdminPermissionAuthorizeAttribute(string permissionCode = null)
        {
            PermissionCode = permissionCode;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;

            var user = httpContext.User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return false;

            var cp = user as ChamPrincipal;
            if (cp == null || !string.Equals(cp.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return false;

            // Admin Tổng luôn có toàn quyền tối cao
            if (cp.IsSuperAdmin)
                return true;

            // Nếu không yêu cầu mã quyền cụ thể, chỉ cần là Admin
            if (string.IsNullOrWhiteSpace(PermissionCode))
                return true;

            // Kiểm tra mã quyền của Admin con
            return cp.HasPermission(PermissionCode);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var user = httpContext.User;

            // 1. Nếu chưa đăng nhập -> Chuyển về trang đăng nhập
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                if (httpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new { success = false, message = "Phiên làm việc đã hết hạn. Vui lòng đăng nhập lại.", requireLogin = true },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    string returnUrl = httpContext.Request.RawUrl;
                    filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
                }
                return;
            }

            // 2. Đã đăng nhập nhưng không đủ quyền
            string actionDesc = PermissionCode ?? "";
            if (actionDesc.EndsWith("_DELETE", StringComparison.OrdinalIgnoreCase))
                actionDesc = "XÓA dữ liệu (" + PermissionCode + ")";
            else if (actionDesc.EndsWith("_CREATE", StringComparison.OrdinalIgnoreCase))
                actionDesc = "THÊM MỚI dữ liệu (" + PermissionCode + ")";
            else if (actionDesc.EndsWith("_EDIT", StringComparison.OrdinalIgnoreCase))
                actionDesc = "CHỈNH SỬA / CẬP NHẬT dữ liệu (" + PermissionCode + ")";

            if (httpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new { success = false, message = "⛔ Tài khoản cơ sở của bạn không được cấp quyền " + actionDesc + ". Vui lòng liên hệ Admin Tổng." },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else if (!string.IsNullOrWhiteSpace(PermissionCode) && PermissionCode.Contains("_") && httpContext.Request.UrlReferrer != null)
            {
                filterContext.Controller.TempData["Error"] = "⛔ Giới hạn phân quyền cơ sở: Tài khoản của bạn không được cấp quyền " + actionDesc + "!";
                filterContext.Result = new RedirectResult(httpContext.Request.UrlReferrer.ToString());
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");
            }
        }
    }

    /// <summary>
    /// Bộ lọc đặc quyền dành riêng cho Admin Tổng (IsSuperAdmin = true).
    /// Áp dụng cho các tính năng quản lý, phân quyền và cấp tài khoản Admin con.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminSuperAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                return false;

            var cp = httpContext.User as ChamPrincipal;
            return cp != null && cp.IsSuperAdmin;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var user = httpContext.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                string returnUrl = httpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult("~/Account/Login?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
                return;
            }

            filterContext.Result = new RedirectResult("~/Account/AccessDenied");
        }
    }
}
