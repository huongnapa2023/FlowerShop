# 🌸 TỔNG HỢP TOÀN BỘ DỰ ÁN WEBSITE HOA TƯƠI CHAM FLOWERS (ASP.NET MVC 5)
> **Tài liệu học tập & Toàn bộ mã nguồn đầy đủ dùng để tải lên Claude / ChatGPT để học lập trình web**
> **Nền tảng:** ASP.NET Web Application (.NET Framework 4.8) | Entity Framework 6.5.1 | Microsoft SQL Server LocalDB | SignalR 2.2.2 | Bootstrap 5

---

## 📌 PHẦN 1: GIÁO TRÌNH NHẬP MÔN DÀNH CHO BẠN (NGƯỜI MỚI HỌC ASP.NET MVC)

Chào bạn! Đừng lo lắng nếu bạn nhìn vào dự án và cảm thấy choáng ngợp vì nhiều file code. Bất kỳ lập trình viên nào khi mới bắt đầu cũng từng trải qua cảm giác này. Dưới đây là bức tranh toàn cảnh giải thích cực kỳ trực quan và dễ hiểu để bạn nắm được bản chất:

### 1. Mô hình MVC (Model - View - Controller) là gì?
Dự án này được tổ chức theo kiến trúc 3 lớp kinh điển của công nghệ web thế giới:
- **M (Model - Dữ liệu):** Nằm trong thư mục `Models/`. Đây là các lớp (class) C# đại diện cho các bảng trong Cơ sở dữ liệu (Database). Ví dụ:
  - `Product.cs`: Đại diện cho sản phẩm hoa (Tên hoa, Giá tiền, Số lượng tồn kho, Mô tả...).
  - `Order.cs` & `OrderItem.cs`: Đại diện cho Đơn hàng và các món hoa trong đơn.
  - `User.cs`: Đại diện cho Tài khoản người dùng (Khách hàng hoặc Quản trị viên).
  - `FlowerShopDbContext.cs`: Là "cây cầu" trung gian kết nối giữa code C# và SQL Server (sử dụng thư viện Entity Framework 6).
- **V (View - Giao diện):** Nằm trong thư mục `Views/`. Đây là các file `.cshtml` chứa mã HTML/CSS kết hợp cú pháp C# Razor (bắt đầu bằng dấu `@`). Nhiệm vụ của View là nhận dữ liệu từ Controller và hiển thị đẹp mắt lên màn hình cho khách xem.
- **C (Controller - Bộ điều khiển):** Nằm trong thư mục `Controllers/`. Đây là "bộ não" của hệ thống. Khi khách bấm nút trên trình duyệt (như bấm "Thêm vào giỏ", "Đặt hàng", "Đăng nhập"), trình duyệt gửi yêu cầu đến Controller. Controller nhận lệnh, tương tác với Database để lấy hoặc lưu dữ liệu, sau đó quyết định chuyển sang trang nào hoặc trả về giao diện nào.

### 2. Luồng chạy của một yêu cầu (Request Flow) từ A - Z
**Ví dụ thực tế: Khách vào xem chi tiết một bó hoa `http://localhost:5188/Product/Details/1`**
1. **Bước 1:** Trình duyệt gửi yêu cầu `GET /Product/Details/1` tới Web Server (IIS Express cổng 5188).
2. **Bước 2:** File định tuyến `App_Start/RouteConfig.cs` đọc đường dẫn và xác định cần gọi hàm `Details(int id = 1)` bên trong file `Controllers/ProductController.cs`.
3. **Bước 3:** Trong `ProductController.cs`:
   - Hàm `Details` gọi `_context.Products.Find(id)` để tìm bó hoa có mã ID là 1 trong SQL Server.
   - Lấy danh sách ảnh phụ, danh sách đánh giá từ khách hàng đã mua.
   - Đóng gói thông tin vào một gói dữ liệu (Model).
4. **Bước 4:** Controller gọi lệnh `return View(model);` để đẩy gói dữ liệu sang file giao diện `Views/Product/Details.cshtml`.
5. **Bước 5:** File View nhận dữ liệu, hiển thị tên hoa qua `@Model.Product.Name`, giá tiền qua `@Model.Product.Price.ToString("N0") đ`, danh sách ảnh hoa, nút "Thêm vào giỏ hàng"... rồi gửi mã HTML về cho Google Chrome hiển thị.

### 3. Giải thích chức năng các thư mục trong dự án
- 📁 **App_Start/**: Chứa các cấu hình cốt lõi khi ứng dụng khởi chạy:
  - `RouteConfig.cs`: Cấu hình quy tắc định tuyến URL (mặc định là: `/{controller}/{action}/{id}`).
  - `BundleConfig.cs`: Đóng gói và tối ưu tải CSS, JavaScript.
  - `FilterConfig.cs`: Đăng ký bộ lọc xử lý lỗi toàn cục.
- 📁 **Controllers/**: Chứa 11 bộ não điều khiển toàn bộ chức năng của website:
  - `HomeController.cs`: Trang chủ (Carousel banner, danh mục, hoa nổi bật, hot deal), Trang Giới thiệu, Tin tức.
  - `ProductController.cs`: Xem tất cả hoa, bộ lọc theo giá/danh mục/loại hoa, tìm kiếm hoa, và trang Admin quản lý thêm/sửa/xóa hoa, danh mục, nhà cung cấp.
  - `OrderController.cs`: Quản lý giỏ hàng (hỗ trợ cả khách vãng lai và khách đã đăng nhập), cập nhật số lượng, xóa món, xem lịch sử đơn hàng, cập nhật trạng thái đơn (Chờ xác nhận, Đang xử lý, Đang giao, Hoàn thành, Đã hủy).
  - `PaymentController.cs`: Quy trình Đặt hàng (Checkout), chọn đơn vị vận chuyển, thanh toán tiền mặt (COD) hoặc thanh toán online qua cổng VNPay Sandbox.
  - `AccountController.cs`: Đăng ký tài khoản mới, Đăng nhập hệ thống (bằng `FormsAuthentication`), Quên mật khẩu, Trang thông tin cá nhân (Profile).
  - `HotDealController.cs`: Quản lý các chương trình hoa giảm giá sốc và Banner Popup đếm ngược khuyến mãi.
  - `ChatController.cs`: Hệ thống Chat trực tuyến chăm sóc khách hàng giữa khách và Admin.
  - `ReviewController.cs`: Đánh giá số sao (1-5 sao) và nhận xét của khách sau khi hoàn thành đơn mua hoa; Admin duyệt hoặc ẩn đánh giá.
  - `CustomerController.cs`: Dành cho Admin quản lý danh sách khách hàng, nâng cấp tài khoản VIP, khóa/mở tài khoản.
  - `BannerController.cs`: Dành cho Admin quản lý banner trang chủ, bài viết cẩm nang hoa tươi, tạo mã giảm giá (Coupon).
  - `ReportController.cs`: Bảng điều khiển (Dashboard) của Admin: biểu đồ doanh thu, thống kê số lượng đơn hàng, tỷ lệ chuyển đổi và nút xuất dữ liệu ra file Excel/CSV.
- 📁 **Models/**: Chứa toàn bộ cấu trúc dữ liệu và liên kết Database:
  - `Entities.cs`: Chứa 18 class thực thể (User, Product, Order, OrderItem, Review, Category, Supplier, Coupon, Banner, ChatMessage...).
  - `FlowerShopDbContext.cs`: Lớp DbContext của Entity Framework kết nối tới SQL Server LocalDB.
  - `LocalDbConnection.cs`: Bộ kết nối thông minh tự động kích hoạt máy chủ LocalDB và phân giải Named Pipe (tương thích 100% cả chip Intel và chip ARM64).
  - `DbInitializer.cs`: Tự động nạp sẵn dữ liệu mẫu (hoa tươi, banner, đơn hàng, tài khoản admin/khách hàng) khi lần đầu chạy.
  - `ViewModels.cs`: Các class gom dữ liệu chuyên biệt phục vụ cho các form (CheckoutViewModel, LoginViewModel, ReportViewModel...).
- 📁 **Services/**: Các dịch vụ bổ trợ tiện ích:
  - `AppHelpers.cs`: Giải thuật băm mật khẩu PBKDF2 bảo mật cao cấp (chuẩn công nghiệp, không bao giờ lưu mật khẩu dạng text), định dạng tiền tệ VND, quản lý danh tính người dùng (`ChamPrincipal`).
  - `VnPayService.cs`: Tạo URL thanh toán VNPay kèm chữ ký số SHA512 chống giả mạo và xử lý phản hồi giao dịch thành công.
- 📁 **Hubs/**:
  - `ChatHub.cs`: Sử dụng công nghệ Microsoft SignalR (WebSocket) để truyền nhận tin nhắn tư vấn và thông báo đơn hàng mới ngay lập tức theo thời gian thực (Real-time) mà khách hàng không cần phải bấm F5 tải lại trang.
- 📁 **Views/**: Chứa hơn 40 file giao diện Razor chia theo từng màn hình nghiệp vụ.
- 📁 **Content/ & images/**: Chứa file CSS tùy biến phong cách hiện đại (`site.css`), Bootstrap 5, và kho hình ảnh hoa tươi chuẩn nghệ thuật.
- 📄 **Web.config**: File cấu hình trung tâm của toàn bộ ứng dụng ASP.NET (.NET Framework 4.8).
- 📄 **Global.asax.cs**: Nơi khởi tạo ban đầu khi máy chủ web được bật lên.

---

## 💡 PHẦN 2: BỘ CÂU HỎI MẪU BẠN HÃY COPY ĐỂ HỎI CLAUDE

Khi bạn tải file này lên Claude (bằng nút hình kẹp ghim 📎 hoặc kéo thả file), bạn hãy copy từng câu hỏi dưới đây để Claude đóng vai gia sư hướng dẫn bạn học:

1. *"Tôi là người mới bắt đầu học ASP.NET MVC 5 (.NET Framework 4.8). Hãy giải thích cho tôi file `Controllers/OrderController.cs`: Luồng thêm sản phẩm vào giỏ hàng (`AddToCart`) và tính tổng tiền giỏ hàng (`Cart`) được code như thế nào?"*
2. *"Hãy đọc file `Controllers/PaymentController.cs` và giải thích quy trình thanh toán Checkout: Dữ liệu từ form giao diện được truyền về Controller xử lý ra sao để lưu vào bảng Orders và OrderItems trong CSDL?"*
3. *"Hãy giải thích cho tôi cơ chế đăng nhập và phân quyền trong file `Controllers/AccountController.cs` và `Global.asax.cs`: `FormsAuthentication` và `ChamPrincipal` hoạt động ra sao để phân biệt tài khoản Admin và Khách hàng thường?"*
4. *"Trong file `Models/FlowerShopDbContext.cs` và `Models/Entities.cs`, Entity Framework 6 hoạt động theo nguyên lý nào để biến các class C# thành các bảng tương ứng trong SQL Server? Mối quan hệ giữa Product, Category và OrderItems được thiết lập như thế nào?"*
5. *"Trong file `Hubs/ChatHub.cs`, công nghệ SignalR hoạt động như thế nào để khi khách hàng gửi tin nhắn thì màn hình của Admin lập tức nhận được thông báo mà không cần bấm F5 tải lại trang?"*
6. *"Hãy chỉ cho tôi cách đặt Breakpoint trong Visual Studio và chạy từng dòng code (F10/F11) ở hàm `HomeController.Index()` để tôi thấy dữ liệu chạy thực tế?"*

---

## 💻 PHẦN 3: TOÀN BỘ MÃ NGUỒN ĐẦY ĐỦ CỦA DỰ ÁN

Mời bạn kéo xuống dưới để xem toàn bộ mã nguồn của từng file:


### File: FlowerShop/Web.config
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="entityFramework" type="System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
  </configSections>

  <connectionStrings>
    <add name="FlowerShopDbContext"
         connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CHAMFlowersDb;Integrated Security=True;MultipleActiveResultSets=True"
         providerName="System.Data.SqlClient" />
  </connectionStrings>

  <appSettings>
    <add key="webpages:Version" value="3.0.0.0" />
    <add key="webpages:Enabled" value="false" />
    <add key="ClientValidationEnabled" value="true" />
    <add key="UnobtrusiveJavaScriptEnabled" value="true" />
    <add key="owin:AppStartup" value="FlowerShop.Startup" />

    <!-- Cấu hình cổng thanh toán trực tuyến VNPay Sandbox -->
    <add key="VnPay:TmnCode" value="CHAMFL01" />
    <add key="VnPay:HashSecret" value="CHAMFLOWERSVNPAYSECRETKEY2026SECURE" />
    <add key="VnPay:BaseUrl" value="https://sandbox.vnpayment.vn/paymentv2/vpcpay.html" />
    <add key="VnPay:UseLocalSandboxSimulator" value="true" />
  </appSettings>

  <system.web>
    <compilation debug="true" targetFramework="4.8" />
    <httpRuntime targetFramework="4.8" maxRequestLength="20480" />
    <globalization culture="vi-VN" uiCulture="vi-VN" requestEncoding="utf-8" responseEncoding="utf-8" fileEncoding="utf-8" />
    <httpCookies httpOnlyCookies="true" />
    <authentication mode="Forms">
      <forms name="CHAMFlowers.Auth" loginUrl="~/Account/Login" timeout="10080" slidingExpiration="true" />
    </authentication>
    <sessionState mode="InProc" timeout="480" cookieName="CHAMFlowers.Session" />
    <customErrors mode="Off" />
  </system.web>

  <system.webServer>
    <modules runAllManagedModulesForAllRequests="true" />
    <staticContent>
      <remove fileExtension=".webp" />
      <mimeMap fileExtension=".webp" mimeType="image/webp" />
      <remove fileExtension=".svg" />
      <mimeMap fileExtension=".svg" mimeType="image/svg+xml" />
    </staticContent>
  </system.webServer>

  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Antlr3.Runtime" publicKeyToken="eb42632606e9261f" />
        <bindingRedirect oldVersion="0.0.0.0-3.5.0.2" newVersion="3.5.0.2" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Web.Infrastructure" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-2.0.0.0" newVersion="2.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" />
        <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Optimization" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-1.1.0.0" newVersion="1.1.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="WebGrease" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-1.6.5135.21930" newVersion="1.6.5135.21930" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Helpers" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-5.2.9.0" newVersion="5.2.9.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.2.0" newVersion="4.2.2.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin.Security" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-4.2.2.0" newVersion="4.2.2.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>

  <entityFramework>
    <providers>
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
    </providers>
  </entityFramework>

  <system.codedom>
    <compilers>
      <compiler language="c#;cs;csharp" extension=".cs" type="Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider, Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=2.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" warningLevel="4" compilerOptions="/langversion:default /nowarn:1659;1699;1701" />
      <compiler language="vb;vbs;visualbasic;vbscript" extension=".vb" type="Microsoft.CodeDom.Providers.DotNetCompilerPlatform.VBCodeProvider, Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=2.0.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" warningLevel="4" compilerOptions="/langversion:default /nowarn:41008 /define:_MYTYPE=\&quot;Web\&quot; /optionInfer+" />
    </compilers>
  </system.codedom>
</configuration>
```

### File: FlowerShop/Global.asax.cs
```csharp
using System;
using System.Data.Entity;
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

                var identity = new GenericIdentity(fullName, "Forms");
                var principal = new ChamPrincipal(identity, role, userId, ticket.Name, isVip);

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
```

### File: FlowerShop/Startup.cs
```csharp
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FlowerShop.Startup))]

namespace FlowerShop
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Khởi tạo định tuyến SignalR 2 cho tính năng Chat thời gian thực (/signalr)
            app.MapSignalR();
        }
    }
}
```

### File: FlowerShop/App_Start\RouteConfig.cs
```csharp
using System.Web.Mvc;
using System.Web.Routing;

namespace FlowerShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
```

### File: FlowerShop/App_Start\BundleConfig.cs
```csharp
using System.Web.Optimization;

namespace FlowerShop
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.7.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.bundle.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/site.css"));
        }
    }
}
```

### File: FlowerShop/App_Start\FilterConfig.cs
```csharp
using System.Web.Mvc;

namespace FlowerShop
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
```

### File: FlowerShop/Models\Entities.cs
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace FlowerShop.Models
{
    public enum ReviewStatus
    {
        Pending = 0,   // Chờ duyệt
        Approved = 1,  // Đã đăng
        Hidden = 2     // Bị ẩn
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Full_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Avatar { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer"; // Admin, Customer

        public bool IsActive { get; set; } = true;

        public DateTime? BirthDate { get; set; }

        public decimal TotalSpent { get; set; } = 0;

        public bool IsVip { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }

    [Table("Categories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("FlowerCategories")]
    public class FlowerCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại hoa")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("Suppliers")]
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà cung cấp")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("Products")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;

        public bool IsHotDeal { get; set; } = false;

        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        public int? SupplierId { get; set; }

        public int? FlowerCategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        public decimal Price { get; set; }

        public decimal? OriginalPrice { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int StockQuantity { get; set; } = 0;

        public DateTime? ExpiryDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("FlowerCategoryId")]
        public virtual FlowerCategory FlowerCategory { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [NotMapped]
        public string MainImageUrl
        {
            get
            {
                if (Images != null)
                {
                    var main = Images.FirstOrDefault(i => i.IsMain);
                    if (main != null && !string.IsNullOrEmpty(main.ImageUrl))
                        return main.ImageUrl;

                    var first = Images.FirstOrDefault();
                    if (first != null && !string.IsNullOrEmpty(first.ImageUrl))
                        return first.ImageUrl;
                }
                return "/images/samflora/b10-main.jpg";
            }
        }

        [NotMapped]
        public int DiscountPercent
        {
            get
            {
                return OriginalPrice.HasValue && OriginalPrice.Value > Price && OriginalPrice.Value > 0
                    ? (int)Math.Round((OriginalPrice.Value - Price) / OriginalPrice.Value * 100)
                    : 0;
            }
        }

        [NotMapped]
        public double AverageRating
        {
            get
            {
                var approved = Reviews != null ? Reviews.Where(r => r.IsApproved).ToList() : null;
                if (approved != null && approved.Count > 0)
                    return Math.Round(approved.Average(r => r.Rating), 1);
                return 5.0;
            }
        }
    }

    [Table("ProductImages")]
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    [Table("Orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [MaxLength(100)]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
        [MaxLength(20)]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [MaxLength(255)]
        public string DeliveryAddress { get; set; } = string.Empty;

        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddHours(3);

        [MaxLength(100)]
        public string ShippingCarrier { get; set; } = "Giao Hoa Hỏa Tốc 2H (CHAM Express)";

        public decimal ShippingFee { get; set; } = 30000;

        [MaxLength(50)]
        public string CouponCode { get; set; }

        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [MaxLength(100)]
        public string PaymentMethod { get; set; } = "COD"; // COD, VNPay

        public decimal Total { get; set; }

        [Required]
        [MaxLength(100)]
        public string Status { get; set; } = "Chờ xác nhận";

        [MaxLength(500)]
        public string Note { get; set; }

        [MaxLength(100)]
        public string VnPayTransactionNo { get; set; }

        public decimal? PaidAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public bool IsPaid { get; set; } = false;

        public bool StockDeducted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        public string Content { get; set; } = string.Empty;

        public ReviewStatus Status { get; set; } = ReviewStatus.Pending;

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    [Table("ProductReviews")]
    public class ProductReview
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Required]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    [Table("Carts")]
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [MaxLength(100)]
        public string SessionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [ForeignKey("CartId")]
        public virtual Cart Cart { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

    [Table("HotDealPopups")]
    public class HotDealPopup
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string Subtitle { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public int? ProductId { get; set; }

        public int? DiscountPercent { get; set; }

        [MaxLength(50)]
        public string CouponCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [NotMapped]
        public bool IsActiveNow
        {
            get
            {
                return IsActive &&
                       (!StartDate.HasValue || DateTime.Now >= StartDate.Value) &&
                       (!EndDate.HasValue || DateTime.Now <= EndDate.Value);
            }
        }
    }

    [Table("Banners")]
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề banner")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Subtitle { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(255)]
        public string LinkUrl { get; set; }

        [MaxLength(80)]
        public string ButtonText { get; set; } = "Khám phá ngay";

        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    [Table("ChatConversations")]
    public class ChatConversation
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "Khách truy cập";

        [MaxLength(100)]
        public string CustomerEmail { get; set; }

        [Required]
        [MaxLength(100)]
        public string SessionKey { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; set; } = "Đang xử lý";

        public bool IsCustomerOnline { get; set; } = true;

        public int UnreadAdminCount { get; set; } = 0;

        public int UnreadCustomerCount { get; set; } = 0;

        public DateTime LastMessageAt { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }

    [Table("ChatMessages")]
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        public int ConversationId { get; set; }

        [Required]
        [MaxLength(30)]
        public string SenderRole { get; set; } = "Customer";

        [Required]
        [MaxLength(100)]
        public string SenderName { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(30)]
        public string MessageType { get; set; } = "Text";

        [MaxLength(255)]
        public string AttachmentUrl { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ConversationId")]
        public virtual ChatConversation Conversation { get; set; }
    }

    [Table("NewsArticles")]
    public class NewsArticle
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề bài viết")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [MaxLength(400)]
        public string Summary { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Vui lòng nhập nội dung bài viết")]
        public string Content { get; set; } = string.Empty;

        public string ImageUrl { get; set; }

        [MaxLength(80)]
        public string CategoryName { get; set; } = "Cẩm nang hoa tươi";

        public bool IsPublished { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    [Table("Coupons")]
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public int DiscountPercent { get; set; } = 10;

        public decimal MaxDiscountAmount { get; set; } = 100000;

        public decimal MinOrderAmount { get; set; } = 200000;

        public bool VipOnly { get; set; } = false;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }

    [Table("MenuItems")]
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hiển thị của menu")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập đường dẫn URL")]
        [MaxLength(255)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Icon { get; set; }

        [MaxLength(30)]
        public string BadgeText { get; set; }

        [MaxLength(30)]
        public string BadgeColor { get; set; } = "danger";

        public int DisplayOrder { get; set; } = 0;

        public bool OpenNewTab { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public int? ParentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
```

### File: FlowerShop/Models\ViewModels.cs
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Models
{
    public class HomeViewModel
    {
        public List<Banner> Banners { get; set; } = new List<Banner>();
        public List<FlowerCategory> FlowerCategories { get; set; } = new List<FlowerCategory>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Product> HotDealProducts { get; set; } = new List<Product>();
        public List<Product> NewArrivals { get; set; } = new List<Product>();
        public List<NewsArticle> LatestNews { get; set; } = new List<NewsArticle>();
        public HotDealPopup ActivePopup { get; set; }
        public List<Coupon> ActiveCoupons { get; set; } = new List<Coupon>();
    }

    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = new Product();
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
        public List<Review> ApprovedReviews { get; set; } = new List<Review>();
        public bool CanReview { get; set; }
        public bool HasReviewed { get; set; }
    }

    public class CartViewModel
    {
        public Cart Cart { get; set; } = new Cart();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 30000;
        public string AppliedCoupon { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get { return Math.Max(0, SubTotal + ShippingFee - DiscountAmount); } }
        public List<Coupon> AvailableCoupons { get; set; } = new List<Coupon>();
    }

    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận")]
        [RegularExpression(@"^[0-9\+\-\s]{9,15}$", ErrorMessage = "Số điện thoại không hợp lệ (9-15 chữ số)")]
        public string RecipientPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng chi tiết")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn thời gian giao hoa")]
        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddHours(3);

        [Required(ErrorMessage = "Vui lòng chọn đơn vị vận chuyển")]
        public string ShippingCarrier { get; set; } = "Giao Hoa Hỏa Tốc 2H (CHAM Express)";

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "COD";

        public string CouponCode { get; set; }

        public string Note { get; set; }

        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 30000;
        public decimal DiscountAmount { get; set; }
        public decimal GrandTotal { get { return Math.Max(0, SubTotal + ShippingFee - DiscountAmount); } }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email đăng nhập")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = true;
        public string ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^[0-9\+\-\s]{9,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng mặc định")]
        public string Address { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        public string Address { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsVip { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu mới không khớp")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class RevenuePoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal GrossProfit { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductStat
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int SoldQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ReportDashboardViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PeriodLabel { get; set; } = "30 ngày gần nhất";

        public decimal TotalRevenue { get; set; }
        public decimal GrossProfit { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

        public int TotalCustomers { get; set; }
        public int VipCustomers { get; set; }
        public int NewCustomersInPeriod { get; set; }

        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int ExpiringSoonCount { get; set; }

        public List<RevenuePoint> RevenueSeries { get; set; } = new List<RevenuePoint>();
        public List<TopProductStat> BestSellingProducts { get; set; } = new List<TopProductStat>();
        public List<User> TopVipCustomers { get; set; } = new List<User>();
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public List<Product> ExpiringProducts { get; set; } = new List<Product>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}
```

### File: FlowerShop/Models\FlowerShopDbContext.cs
```csharp
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FlowerShop.Models
{
    public class FlowerShopDbContext : DbContext
    {
        public FlowerShopDbContext() : base(LocalDbConnection.GetConnectionString())
        {
        }

        // 12 bảng dữ liệu cốt lõi
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FlowerCategory> FlowerCategories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        // Các thực thể mở rộng nghiệp vụ
        public DbSet<HotDealPopup> HotDealPopups { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tránh lỗi Multiple Cascade Paths trên SQL Server (Users -> Orders/Reviews, Products -> OrderItems)
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Cấu hình độ chính xác tiền tệ VND
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(18, 2));

            // Chỉ bật Cascade Delete cho các bảng con phụ thuộc trực tiếp
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithRequired(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithRequired(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithRequired(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithRequired(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithRequired(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductReviews)
                .WithRequired(pr => pr.Product)
                .HasForeignKey(pr => pr.ProductId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChatConversation>()
                .HasMany(c => c.Messages)
                .WithRequired(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .WillCascadeOnDelete(true);
        }
    }
}
```

### File: FlowerShop/Models\LocalDbConnection.cs
```csharp
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace FlowerShop.Models
{
    /// <summary>
    /// Giải quyết lỗi "Unable to load the SQLUserInstance.dll" (error 56 / %1 is not a valid Win32 application)
    /// khi IIS Express chạy dạng ARM64 gốc nhưng LocalDB chỉ có DLL x64.
    /// Thay vì để SqlClient tự nạp SQLUserInstance.dll để phân giải "(localdb)\Instance",
    /// lớp này gọi SqlLocalDB.exe (tiến trình riêng) để khởi động instance và lấy tên named pipe,
    /// sau đó kết nối thẳng qua "np:\\.\pipe\LOCALDB#xxxx\tsql\query".
    /// </summary>
    public static class LocalDbConnection
    {
        private const string ConnectionName = "FlowerShopDbContext";
        private static readonly object SyncRoot = new object();
        private static string _cachedConnectionString;
        private static string _cachedPipe;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);

        public static string GetConnectionString()
        {
            string configured = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
            var builder = new SqlConnectionStringBuilder(configured);
            string dataSource = builder.DataSource ?? string.Empty;

            if (!dataSource.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
                return configured;

            lock (SyncRoot)
            {
                if (_cachedConnectionString != null && PipeExists(_cachedPipe))
                    return _cachedConnectionString;

                int slash = dataSource.IndexOf('\\');
                string instance = slash >= 0 ? dataSource.Substring(slash + 1) : "MSSQLLocalDB";

                string pipe = ResolvePipe(instance);
                if (string.IsNullOrEmpty(pipe))
                    return configured; // fallback về cách kết nối mặc định

                builder.DataSource = pipe;
                _cachedPipe = pipe;
                _cachedConnectionString = builder.ConnectionString;
                return _cachedConnectionString;
            }
        }

        private static bool PipeExists(string npDataSource)
        {
            if (string.IsNullOrEmpty(npDataSource)) return false;
            string pipePath = npDataSource.StartsWith("np:", StringComparison.OrdinalIgnoreCase)
                ? npDataSource.Substring(3)
                : npDataSource;

            if (WaitNamedPipe(pipePath, 1)) return true;
            int err = Marshal.GetLastWin32Error();
            return err == 121; // ERROR_SEM_TIMEOUT: pipe tồn tại nhưng đang bận
        }

        private static string ResolvePipe(string instance)
        {
            string exe = FindSqlLocalDbExe();
            if (exe == null) return null;

            // 1. Kiểm tra trước xem instance đã đang chạy và có pipe sẵn chưa
            string info = RunSqlLocalDb(exe, "info \"" + instance + "\"");
            if (!string.IsNullOrEmpty(info))
            {
                var m = Regex.Match(info, @"np:\\\\\.\\pipe\\[^\s\r\n]+", RegexOptions.IgnoreCase);
                if (m.Success && !string.IsNullOrEmpty(m.Value))
                {
                    string existingPipe = m.Value.Trim();
                    if (PipeExists(existingPipe))
                        return existingPipe;
                }
            }

            // 2. Khởi động instance nếu chưa chạy
            RunSqlLocalDb(exe, "start \"" + instance + "\"");

            // 3. Vòng lặp chờ tối đa 10 giây (20 x 500ms) để SQL Server khởi tạo xong Named Pipe
            for (int i = 0; i < 20; i++)
            {
                info = RunSqlLocalDb(exe, "info \"" + instance + "\"");
                if (!string.IsNullOrEmpty(info))
                {
                    var match = Regex.Match(info, @"np:\\\\\.\\pipe\\[^\s\r\n]+", RegexOptions.IgnoreCase);
                    if (match.Success && !string.IsNullOrEmpty(match.Value))
                    {
                        string p = match.Value.Trim();
                        // Chờ thêm 200ms để SQL Server hoàn tất đăng ký pipe listener
                        System.Threading.Thread.Sleep(200);
                        return p;
                    }
                }
                System.Threading.Thread.Sleep(500);
            }

            return null;
        }

        private static string RunSqlLocalDb(string exe, string args)
        {
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit(30000);
                    return output;
                }
            }
            catch
            {
                return null;
            }
        }

        private static string FindSqlLocalDbExe()
        {
            // Các đường dẫn tiêu chuẩn phổ biến nhất
            string[] directPaths =
            {
                @"C:\Program Files\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\140\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files\Microsoft SQL Server\130\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe",
                @"C:\Program Files (x86)\Microsoft SQL Server\150\Tools\Binn\SqlLocalDB.exe"
            };

            foreach (string p in directPaths)
            {
                if (File.Exists(p)) return p;
            }

            string[] roots =
            {
                Environment.GetEnvironmentVariable("ProgramW6432"),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"C:\Program Files"
            };

            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                string sqlRoot = Path.Combine(root, "Microsoft SQL Server");
                if (!Directory.Exists(sqlRoot)) continue;

                string[] versions = Directory.GetDirectories(sqlRoot);
                Array.Sort(versions);
                Array.Reverse(versions); // ưu tiên phiên bản mới nhất (170, 160, 150...)
                foreach (string v in versions)
                {
                    string candidate = Path.Combine(v, @"Tools\Binn\SqlLocalDB.exe");
                    if (File.Exists(candidate)) return candidate;
                }
            }

            return null;
        }
    }
}
```

### File: FlowerShop/Models\DbInitializer.cs
```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using FlowerShop.Services;

namespace FlowerShop.Models
{
    public static class DbInitializer
    {
        private class SeedProductEntry
        {
            public Product Prod { get; set; }
            public string MainImg { get; set; }
            public string SubImg { get; set; }
        }

        private class SeedOrderItemEntry
        {
            public Product Prod { get; set; }
            public int Qty { get; set; }
        }

        private class SeedOrderEntry
        {
            public User User { get; set; }
            public string Status { get; set; }
            public string Payment { get; set; }
            public bool IsPaid { get; set; }
            public int DaysAgo { get; set; }
            public List<SeedOrderItemEntry> Items { get; set; }
        }

        public static void Initialize(FlowerShopDbContext context, string webRootPath)
        {
            if (!string.IsNullOrEmpty(webRootPath))
            {
                string appDataDir = Path.Combine(webRootPath, "App_Data");
                Directory.CreateDirectory(appDataDir);
            }

            try
            {
                if (!context.Database.Exists())
                {
                    context.Database.CreateIfNotExists();
                }
            }
            catch
            {
                // CSDL CHAMFlowersDb đã tồn tại sẵn trên LocalDB
            }

            // Đảm bảo bảng MenuItems tồn tại và có dữ liệu mẫu
            EnsureMenuItemsTableAndSeed(context);

            try
            {
                if (context.Users.Any())
                {
                    // Đảm bảo mật khẩu tài khoản quản trị và mẫu luôn chính xác
                    var adm = context.Users.FirstOrDefault(u => u.Email == "admin@chamflowers.vn" || u.Role == "Admin");
                    if (adm != null)
                    {
                        adm.Password = AppHelpers.HashPassword("Admin@123");
                        adm.IsActive = true;
                    }
                    var vip = context.Users.FirstOrDefault(u => u.Email == "khachvip@chamflowers.vn");
                    if (vip != null)
                    {
                        vip.Password = AppHelpers.HashPassword("Khach@123");
                        vip.IsActive = true;
                    }
                    var cust = context.Users.FirstOrDefault(u => u.Email == "khachhang@chamflowers.vn");
                    if (cust != null)
                    {
                        cust.Password = AppHelpers.HashPassword("Khach@123");
                        cust.IsActive = true;
                    }
                    context.SaveChanges();
                    return;
                }
            }
            catch
            {
                return;
            }

            var rng = new Random(2026);

            // 1. Seed Users (Admin Nguyễn Quang Hưởng + Customers)
            var admin = new User
            {
                FullName = "Nguyễn Quang Hưởng",
                Full_name = "Nguyễn Quang Hưởng",
                Email = "admin@chamflowers.vn",
                Password = AppHelpers.HashPassword("Admin@123"),
                Phone = "0909123456",
                Address = "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh",
                Role = "Admin",
                IsActive = true,
                IsVip = true,
                TotalSpent = 0,
                CreatedAt = DateTime.Now.AddMonths(-6)
            };

            var vip1 = new User
            {
                FullName = "Trần Minh Anh",
                Full_name = "Trần Minh Anh",
                Email = "khachvip@chamflowers.vn",
                Password = AppHelpers.HashPassword("Khach@123"),
                Phone = "0912345678",
                Address = "45 Lê Lợi, Phường Bến Nghé, Quận 1, TP.HCM",
                BirthDate = new DateTime(1996, 5, 18),
                Role = "Customer",
                IsActive = true,
                IsVip = true,
                TotalSpent = 4850000,
                CreatedAt = DateTime.Now.AddMonths(-3)
            };

            var vip2 = new User
            {
                FullName = "Lê Hoàng Nam",
                Full_name = "Lê Hoàng Nam",
                Email = "hoangnam@gmail.com",
                Password = AppHelpers.HashPassword("Khach@123"),
                Phone = "0987654321",
                Address = "210 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM",
                BirthDate = new DateTime(1992, 10, 12),
                Role = "Customer",
                IsActive = true,
                IsVip = true,
                TotalSpent = 3190000,
                CreatedAt = DateTime.Now.AddMonths(-2)
            };

            var cust1 = new User
            {
                FullName = "Phạm Thu Hà",
                Full_name = "Phạm Thu Hà",
                Email = "khachhang@chamflowers.vn",
                Password = AppHelpers.HashPassword("Khach@123"),
                Phone = "0938112233",
                Address = "88 Phan Xích Long, Quận Phú Nhuận, TP.HCM",
                BirthDate = new DateTime(1999, 8, 24),
                Role = "Customer",
                IsActive = true,
                IsVip = false,
                TotalSpent = 1130000,
                CreatedAt = DateTime.Now.AddDays(-18)
            };

            var lockedCust = new User
            {
                FullName = "Đặng Quốc Bảo",
                Full_name = "Đặng Quốc Bảo",
                Email = "bikhoa@gmail.com",
                Password = AppHelpers.HashPassword("Khach@123"),
                Phone = "0977001122",
                Address = "12 Cộng Hòa, Quận Tân Bình, TP.HCM",
                Role = "Customer",
                IsActive = false,
                IsVip = false,
                TotalSpent = 0,
                CreatedAt = DateTime.Now.AddDays(-10)
            };

            context.Users.AddRange(new[] { admin, vip1, vip2, cust1, lockedCust });
            context.SaveChanges();

            // 2. Seed Categories
            var catBirthday = new Category { Name = "Hoa Sinh Nhật", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) };
            var catOpening = new Category { Name = "Hoa Khai Trương", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) };
            var catLove = new Category { Name = "Hoa Tình Yêu & Kỷ Niệm", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) };
            var catWedding = new Category { Name = "Hoa Cưới Cầm Tay", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) };
            var catCongrats = new Category { Name = "Hoa Chúc Mừng & Tri Ân", IsActive = true, CreatedAt = DateTime.Now.AddMonths(-5) };

            context.Categories.AddRange(new[] { catBirthday, catOpening, catLove, catWedding, catCongrats });
            context.SaveChanges();

            // 3. Seed FlowerCategories
            var fcRose = new FlowerCategory
            {
                Name = "Hoa Hồng Thiết Kế Cao Cấp",
                Description = "Biểu tượng của tình yêu nồng cháy, sang trọng và quyến rũ.",
                ImageUrl = "/images/samflora/b7-main.jpg",
                CreatedAt = DateTime.Now.AddMonths(-5)
            };
            var fcOrchid = new FlowerCategory
            {
                Name = "Cẩm Tú Cầu & Lan Hồ Điệp",
                Description = "Vẻ đẹp vương giả, phú quý và thịnh vượng bền lâu.",
                ImageUrl = "/images/samflora/b6-sub2.jpg",
                CreatedAt = DateTime.Now.AddMonths(-5)
            };
            var fcSunflower = new FlowerCategory
            {
                Name = "Hoa Tone Xanh Baby Blue",
                Description = "Thanh lịch, dịu mắt và dẫn đầu xu hướng quà tặng hiện đại.",
                ImageUrl = "/images/samflora/b4-main.jpg",
                CreatedAt = DateTime.Now.AddMonths(-5)
            };
            var fcTulip = new FlowerCategory
            {
                Name = "Hoa Tulip Hà Lan",
                Description = "Thanh lịch, tinh tế mang hơi thở mùa xuân châu Âu.",
                ImageUrl = "/images/samflora/editorial-3.jpg",
                CreatedAt = DateTime.Now.AddMonths(-5)
            };
            var fcHydrangea = new FlowerCategory
            {
                Name = "Hoa Mẫu Đơn & Pastel",
                Description = "Dịu dàng, tròn đầy cảm xúc cho những dịp kỷ niệm đáng nhớ.",
                ImageUrl = "/images/samflora/editorial-4.jpg",
                CreatedAt = DateTime.Now.AddMonths(-5)
            };

            context.FlowerCategories.AddRange(new[] { fcRose, fcOrchid, fcSunflower, fcTulip, fcHydrangea });
            context.SaveChanges();

            // 4. Seed Suppliers
            var supDalat = new Supplier
            {
                Name = "Nông Trại Hoa Đà Lạt Hasfarm",
                Address = "450 Nguyên Tử Lực, TP. Đà Lạt, Lâm Đồng",
                Phone = "02633824999",
                Email = "contact@dalathasfarm-partner.vn",
                IsActive = true
            };
            var supOrchid = new Supplier
            {
                Name = "Vườn Lan Hồ Điệp Bảo Lộc",
                Address = "QL20 Lộc Châu, TP. Bảo Lộc, Lâm Đồng",
                Phone = "02633711888",
                Email = "vuonlanbaoloc@gmail.com",
                IsActive = true
            };
            var supImport = new Supplier
            {
                Name = "Flora Holland Import VN",
                Address = "Khu Công Nghệ Cao, TP. Thủ Đức, TP.HCM",
                Phone = "02839105566",
                Email = "import@floraholland.vn",
                IsActive = true
            };

            context.Suppliers.AddRange(new[] { supDalat, supOrchid, supImport });
            context.SaveChanges();

            // 5. Seed Products (12 sản phẩm hoa thiết kế đặc sắc với ảnh chụp thực tế)
            var products = new List<SeedProductEntry>
            {
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Trà Cụm Nơ Lụa Lãng Mạn",
                        Slug = "bo-hoa-hong-tra-cum-no-lua-lang-man",
                        CategoryId = catLove.Id,
                        FlowerCategoryId = fcRose.Id,
                        SupplierId = supImport.Id,
                        Price = 650000,
                        OriginalPrice = 820000,
                        IsFeatured = true,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 28,
                        ExpiryDate = DateTime.Now.AddDays(10),
                        Description = "Kết từ những cụm hồng trà hồng thắm bung nở tròn đầy, phối giấy lưới tuyết trắng Hàn Quốc và nơ lụa voan hồng tinh tế. Món quà hoàn hảo trao gửi lời yêu thương chân thành."
                    },
                    MainImg = "/images/samflora/b10-main.jpg",
                    SubImg = "/images/samflora/b10-sub2.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Pastel Mix Tỉ Muội Ngọt Ngào",
                        Slug = "bo-hoa-hong-pastel-mix-ti-muoi-ngot-ngao",
                        CategoryId = catBirthday.Id,
                        FlowerCategoryId = fcRose.Id,
                        SupplierId = supDalat.Id,
                        Price = 580000,
                        OriginalPrice = 690000,
                        IsFeatured = true,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 22,
                        ExpiryDate = DateTime.Now.AddDays(8),
                        Description = "Sự hòa quyện ngọt ngào giữa hoa hồng kem phấn và hồng tỉ muội cánh kép đính nơ ruy băng hồng mềm mại, kèm thiệp viết tay gửi trọn lời chúc sinh nhật đáng nhớ."
                    },
                    MainImg = "/images/samflora/b12-main.jpg",
                    SubImg = "/images/samflora/b11-main.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Hộp Hoa Cẩm Tú Cầu Xanh Phối Lan Hồ Điệp Thượng Hạng",
                        Slug = "hop-hoa-cam-tu-cau-xanh-phoi-lan-ho-diep-thuong-hang",
                        CategoryId = catOpening.Id,
                        FlowerCategoryId = fcOrchid.Id,
                        SupplierId = supOrchid.Id,
                        Price = 1850000,
                        OriginalPrice = 2200000,
                        IsFeatured = true,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 14,
                        ExpiryDate = DateTime.Now.AddDays(25),
                        Description = "Hộp hoa lụa cỡ đại sang trọng kết hợp Cẩm Tú Cầu xanh da trời, Hồng xanh dương, Hồng trắng kem và cành Lan Hồ Điệp trắng điểm nhấn vương giả cho dịp khai trương, chúc mừng đối tác."
                    },
                    MainImg = "/images/samflora/b6-sub2.jpg",
                    SubImg = "/images/samflora/b6-main.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Juliet Cam Đào Khai Trương Hồng Phát",
                        Slug = "bo-hoa-hong-juliet-cam-dao-khai-truong-hong-phat",
                        CategoryId = catOpening.Id,
                        FlowerCategoryId = fcHydrangea.Id,
                        SupplierId = supDalat.Id,
                        Price = 1450000,
                        OriginalPrice = 1750000,
                        IsFeatured = true,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 16,
                        ExpiryDate = DateTime.Now.AddDays(9),
                        Description = "Thiết kế hoa nghệ thuật phóng khoáng phối hợp Hồng ngoại Juliet cam đào, Hồng trắng cồ, chuỗi ngọc đỏ may mắn và lá bạc Khuynh Diệp thơm mát, rước tài lộc thịnh vượng."
                    },
                    MainImg = "/images/samflora/editorial-2.jpg",
                    SubImg = "/images/samflora/editorial-4.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Pastel Mix Sắc Vàng Nắng Ban Mai",
                        Slug = "bo-hoa-pastel-mix-sac-vang-nang-ban-mai",
                        CategoryId = catCongrats.Id,
                        FlowerCategoryId = fcRose.Id,
                        SupplierId = supDalat.Id,
                        Price = 450000,
                        OriginalPrice = 520000,
                        IsFeatured = true,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 35,
                        ExpiryDate = DateTime.Now.AddDays(7),
                        Description = "Bó hoa rạng rỡ đa sắc màu kết hợp hồng kem phấn, cúc họa mi hồng nhụy vàng, hoa chuông vàng và phi yến xanh, thích hợp tặng lễ tốt nghiệp, thăng chức và chúc mừng thành công."
                    },
                    MainImg = "/images/samflora/b8-main.jpg",
                    SubImg = "/images/samflora/editorial-1.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Tulip Hồng Nhập Khẩu Hà Lan",
                        Slug = "bo-hoa-tulip-hong-nhap-khau-ha-lan",
                        CategoryId = catLove.Id,
                        FlowerCategoryId = fcTulip.Id,
                        SupplierId = supImport.Id,
                        Price = 920000,
                        OriginalPrice = 1150000,
                        IsFeatured = true,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 12,
                        ExpiryDate = DateTime.Now.AddDays(6),
                        Description = "Những cành Tulip hồng cánh kép nhập khẩu trực tiếp từ Hà Lan được bảo quản lạnh tiêu chuẩn, mang vẻ đẹp kiêu sa, thanh lịch dành tặng người thương."
                    },
                    MainImg = "/images/samflora/editorial-3.jpg",
                    SubImg = "/images/samflora/b10-sub2.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Cẩm Tú Cầu Xanh Phối Hồng Trắng Sương Mai",
                        Slug = "bo-hoa-cam-tu-cau-xanh-phoi-hong-trang-suong-mai",
                        CategoryId = catBirthday.Id,
                        FlowerCategoryId = fcSunflower.Id,
                        SupplierId = supDalat.Id,
                        Price = 620000,
                        OriginalPrice = 700000,
                        IsFeatured = true,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 19,
                        ExpiryDate = DateTime.Now.AddDays(8),
                        Description = "Bó hoa tone Xanh Dương – Trắng hiện đại với tâm điểm Cẩm Tú Cầu xanh bơ Đà Lạt kết hợp hồng trắng, phi yến xanh trong lớp giấy gói đen sang trọng."
                    },
                    MainImg = "/images/samflora/b5-main.jpg",
                    SubImg = "/images/samflora/b6-sub2.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Phấn Phối Cẩm Chướng Trắng Tinh Khôi",
                        Slug = "bo-hoa-hong-phan-phoi-cam-chuong-trang-tinh-khoi",
                        CategoryId = catWedding.Id,
                        FlowerCategoryId = fcRose.Id,
                        SupplierId = supImport.Id,
                        Price = 780000,
                        OriginalPrice = 890000,
                        IsFeatured = false,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 10,
                        ExpiryDate = DateTime.Now.AddDays(7),
                        Description = "Bó hoa dáng tròn thanh thoát kết từ Hồng phấn pastel, Cẩm Chướng trắng viền hồng và ruy băng lụa tơ tằm tôn vinh nét đẹp tinh khôi trong ngày trọng đại."
                    },
                    MainImg = "/images/samflora/b11-main.jpg",
                    SubImg = "/images/samflora/editorial-4.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Cẩm Tú Cầu Hồng Điểm Lan Trắng Tri Ân",
                        Slug = "bo-hoa-cam-tu-cau-hong-diem-lan-trang-tri-an",
                        CategoryId = catCongrats.Id,
                        FlowerCategoryId = fcOrchid.Id,
                        SupplierId = supDalat.Id,
                        Price = 540000,
                        OriginalPrice = 650000,
                        IsFeatured = false,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 24,
                        ExpiryDate = DateTime.Now.AddDays(9),
                        Description = "Bó Cẩm Tú Cầu hồng phấn cỡ lớn tròn đầy điểm xuyết cành Lan Hồ Điệp trắng và viền ren tinh xảo, gửi gắm lòng biết ơn sâu sắc đến thầy cô, đối tác và đấng sinh thành."
                    },
                    MainImg = "/images/samflora/b9-main.jpg",
                    SubImg = "/images/samflora/b10-sub2.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Kem Dâu Nơ Lụa Trắng Dịu Dàng",
                        Slug = "bo-hoa-hong-kem-dau-no-lua-trang-diu-dang",
                        CategoryId = catBirthday.Id,
                        FlowerCategoryId = fcRose.Id,
                        SupplierId = supDalat.Id,
                        Price = 490000,
                        OriginalPrice = 560000,
                        IsFeatured = true,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 3,
                        ExpiryDate = DateTime.Now.AddDays(5),
                        Description = "18 đóa hồng kem dâu gói giấy Kraft ánh đồng phối dải nơ lụa trắng bản lớn thướt tha, tông màu ấm áp được yêu thích nhất tại CHAM Flowers."
                    },
                    MainImg = "/images/samflora/b7-main.jpg",
                    SubImg = "/images/samflora/b12-main.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bó Hoa Hồng Tỉ Muội Xanh Dương Baby Blue",
                        Slug = "bo-hoa-hong-ti-muoi-xanh-duong-baby-blue",
                        CategoryId = catLove.Id,
                        FlowerCategoryId = fcSunflower.Id,
                        SupplierId = supImport.Id,
                        Price = 390000,
                        OriginalPrice = null,
                        IsFeatured = false,
                        IsHotDeal = false,
                        IsActive = true,
                        StockQuantity = 15,
                        ExpiryDate = DateTime.Now.AddDays(14),
                        Description = "Bó hoa hồng tỉ muội sắc xanh dương Baby Blue độc đáo phối giấy gói đen huyền bí và ruy băng xanh da trời, biểu tượng của tình yêu vĩnh cửu và điều kỳ diệu."
                    },
                    MainImg = "/images/samflora/b4-main.jpg",
                    SubImg = "/images/samflora/b5-main.jpg"
                },
                new SeedProductEntry
                {
                    Prod = new Product
                    {
                        Name = "Bình Hoa Mẫu Đơn Hồng Phấn & Hồng Ohara Thượng Hạng",
                        Slug = "binh-hoa-mau-don-hong-phan-hong-ohara-thuong-hang",
                        CategoryId = catLove.Id,
                        FlowerCategoryId = fcHydrangea.Id,
                        SupplierId = supImport.Id,
                        Price = 1250000,
                        OriginalPrice = 1550000,
                        IsFeatured = false,
                        IsHotDeal = true,
                        IsActive = true,
                        StockQuantity = 2,
                        ExpiryDate = DateTime.Now.AddDays(2),
                        Description = "Hoa Mẫu Đơn Sarah Bernhardt cánh kép bung nở tròn đầy phối cùng Hồng ngoại Ohara kem đào và lá bạc, thắt nơ lụa tơ tằm sang trọng cho những dịp kỷ niệm đặc biệt."
                    },
                    MainImg = "/images/samflora/editorial-4.jpg",
                    SubImg = "/images/samflora/b12-main.jpg"
                }
            };

            foreach (var item in products)
            {
                item.Prod.CreatedAt = DateTime.Now.AddDays(-rng.Next(5, 45));
                context.Products.Add(item.Prod);
                context.SaveChanges();

                context.ProductImages.AddRange(new[]
                {
                    new ProductImage { ProductId = item.Prod.Id, ImageUrl = item.MainImg, IsMain = true },
                    new ProductImage { ProductId = item.Prod.Id, ImageUrl = item.SubImg, IsMain = false }
                });
                context.SaveChanges();
            }

            var allProds = context.Products.OrderBy(p => p.Id).ToList();

            // 6. Seed Banners
            context.Banners.AddRange(new[]
            {
                new Banner
                {
                    Title = "Trao Hoa Tươi - Gửi Trọn Yêu Thương",
                    Subtitle = "Bộ sưu tập hoa tươi nghệ thuật thiết kế riêng tại CHAM Flowers • Giao hỏa tốc 2 giờ nội thành",
                    ImageUrl = "/images/samflora/editorial-2.jpg",
                    LinkUrl = "/Product",
                    ButtonText = "Khám phá bộ sưu tập",
                    DisplayOrder = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Banner
                {
                    Title = "Ưu Đãi Hot Deal Tuần Lễ Hoa Thiết Kế",
                    Subtitle = "Giảm đến 25% các mẫu Hồng Trà Cụm, Tulip Hà Lan & Mẫu Đơn • Nhập mã CHAMFLOWERS15 giảm thêm 15%",
                    ImageUrl = "/images/samflora/b10-main.jpg",
                    LinkUrl = "/HotDeal",
                    ButtonText = "Săn Hot Deal ngay",
                    DisplayOrder = 2,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-7)
                },
                new Banner
                {
                    Title = "Hộp Hoa Cẩm Tú Cầu Xanh & Lan Hồ Điệp Thượng Hạng",
                    Subtitle = "Miễn phí thiệp & banner chúc mừng in tên doanh nghiệp • Xuất hóa đơn nhanh chóng",
                    ImageUrl = "/images/samflora/b6-sub2.jpg",
                    LinkUrl = string.Format("/Product?categoryId={0}", catOpening.Id),
                    ButtonText = "Xem hoa khai trương",
                    DisplayOrder = 3,
                    IsActive = true,
                    CreatedAt = DateTime.Now.AddDays(-3)
                }
            });

            // 7. Seed HotDealPopup
            context.HotDealPopups.Add(new HotDealPopup
            {
                Title = "Flash Sale Hoa Tươi Hôm Nay - Giảm Tới 25%!",
                Subtitle = "Tặng thêm mã giảm giá 15% cho mọi đơn đặt hoa trực tuyến tại CHAM Flowers.",
                ImageUrl = "/images/samflora/b12-main.jpg",
                ProductId = allProds[0].Id,
                DiscountPercent = 21,
                CouponCode = "CHAMFLOWERS15",
                IsActive = true,
                StartDate = DateTime.Now.AddDays(-5),
                EndDate = DateTime.Now.AddDays(14),
                CreatedAt = DateTime.Now.AddDays(-2)
            });

            // 8. Seed Coupons
            context.Coupons.AddRange(new[]
            {
                new Coupon
                {
                    Code = "CHAMFLOWERS15",
                    Description = "Giảm 15% (tối đa 100.000đ) cho đơn từ 300.000đ",
                    DiscountPercent = 15,
                    MaxDiscountAmount = 100000,
                    MinOrderAmount = 300000,
                    VipOnly = false,
                    ExpiryDate = DateTime.Now.AddMonths(2),
                    IsActive = true
                },
                new Coupon
                {
                    Code = "CHAOHE10",
                    Description = "Giảm 10% (tối đa 60.000đ) cho mọi đơn hàng từ 200.000đ",
                    DiscountPercent = 10,
                    MaxDiscountAmount = 60000,
                    MinOrderAmount = 200000,
                    VipOnly = false,
                    ExpiryDate = DateTime.Now.AddMonths(3),
                    IsActive = true
                },
                new Coupon
                {
                    Code = "VIP20",
                    Description = "Đặc quyền Khách VIP: Giảm 20% (tối đa 250.000đ)",
                    DiscountPercent = 20,
                    MaxDiscountAmount = 250000,
                    MinOrderAmount = 500000,
                    VipOnly = true,
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    IsActive = true
                }
            });

            // 9. Seed NewsArticles
            context.NewsArticles.AddRange(new[]
            {
                new NewsArticle
                {
                    Title = "Bí quyết giữ hoa tươi lâu trên 7 ngày tại nhà cực đơn giản",
                    Slug = "bi-quyet-giu-hoa-tuoi-lau-tren-7-ngay",
                    CategoryName = "Cẩm nang hoa tươi",
                    Summary = "Chỉ với vài mẹo cắt gốc 45 độ, thay nước sạch và dưỡng hoa đúng cách, bình hoa của bạn sẽ luôn rạng rỡ suốt cả tuần.",
                    Content = "<p>Hoa tươi mang lại sức sống và nguồn năng lượng tích cực cho không gian sống. Để giữ bó hoa từ <strong>CHAM Flowers</strong> luôn tươi mới, bạn hãy áp dụng 4 bước sau:</p><ul><li><strong>Cắt xéo gốc 45 độ:</strong> Giúp tăng diện tích hút nước của cành hoa.</li><li><strong>Loại bỏ lá ngập trong nước:</strong> Tránh vi khuẩn làm đục nước bình hoa.</li><li><strong>Sử dụng gói dưỡng hoa Chrysal:</strong> Luôn được tặng kèm miễn phí trong mọi đơn hàng của CHAM Flowers.</li><li><strong>Đặt nơi thoáng mát:</strong> Tránh ánh nắng gắt trực tiếp hoặc luồng gió máy lạnh thổi thẳng vào cánh hoa.</li></ul>",
                    ImageUrl = "/images/samflora/editorial-2.jpg",
                    IsPublished = true,
                    CreatedAt = DateTime.Now.AddDays(-12)
                },
                new NewsArticle
                {
                    Title = "Ý nghĩa các loài hoa tặng sinh nhật theo từng tháng sinh",
                    Slug = "y-nghia-cac-loai-hoa-tang-sinh-nhat",
                    CategoryName = "Tư vấn chọn hoa",
                    Summary = "Mỗi loài hoa ẩn chứa một thông điệp riêng biệt. Khám phá cách chọn bó hoa sinh nhật tinh tế khiến người nhận cảm động.",
                    Content = "<p>Tặng hoa sinh nhật không chỉ là trao một món quà thẩm mỹ mà còn là cách gửi gắm lời chúc ý nghĩa nhất:</p><ul><li><strong>Hoa Hồng Pastel / Kem dâu:</strong> Dịu dàng, trân trọng dành cho bạn gái, chị em gái hoặc đồng nghiệp nữ.</li><li><strong>Hoa Hướng Dương:</strong> Chúc tuổi mới luôn tỏa sáng, thành công trên con đường sự nghiệp.</li><li><strong>Hoa Lan Hồ Điệp:</strong> Sang trọng, kính trọng dành tặng bố mẹ, sếp hoặc đối tác quan trọng.</li></ul>",
                    ImageUrl = "/images/samflora/b7-main.jpg",
                    IsPublished = true,
                    CreatedAt = DateTime.Now.AddDays(-6)
                },
                new NewsArticle
                {
                    Title = "Cách chọn kệ hoa khai trương hợp phong thủy rước tài lộc",
                    Slug = "cach-chon-ke-hoa-khai-truong-hop-phong-thuy",
                    CategoryName = "Hoa sự kiện",
                    Summary = "Tông đỏ hồng phát, vàng hoàng kim hay xanh dương tài lộc? Gợi ý chọn hoa chúc mừng khai trương chuẩn mực cho doanh nghiệp.",
                    Content = "<p>Lễ khai trương đánh dấu bước khởi đầu quan trọng của mỗi cửa hàng, doanh nghiệp. Tại <strong>CHAM Flowers</strong>, các mẫu kệ và hộp hoa khai trương được phối màu theo ý nghĩa phong thủy cát tường: sắc xanh dương Cẩm Tú Cầu và Lan Hồ Điệp trắng tượng trưng cho dòng chảy tài lộc hanh thông; sắc hồng cam của Hồng Juliet mang lại năng lượng bứt phá hồng phát.</p>",
                    ImageUrl = "/images/samflora/b6-sub2.jpg",
                    IsPublished = true,
                    CreatedAt = DateTime.Now.AddDays(-2)
                }
            });

            // 10. Seed Orders & OrderItems
            var sampleOrders = new List<SeedOrderEntry>
            {
                new SeedOrderEntry
                {
                    User = vip1, Status = "Hoàn thành", Payment = "VNPay", IsPaid = true, DaysAgo = 16,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[2], Qty = 1 }, new SeedOrderItemEntry { Prod = allProds[0], Qty = 1 } }
                },
                new SeedOrderEntry
                {
                    User = vip2, Status = "Hoàn thành", Payment = "COD", IsPaid = true, DaysAgo = 12,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[3], Qty = 1 }, new SeedOrderItemEntry { Prod = allProds[4], Qty = 2 } }
                },
                new SeedOrderEntry
                {
                    User = cust1, Status = "Hoàn thành", Payment = "VNPay", IsPaid = true, DaysAgo = 8,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[1], Qty = 1 }, new SeedOrderItemEntry { Prod = allProds[4], Qty = 1 } }
                },
                new SeedOrderEntry
                {
                    User = vip1, Status = "Hoàn thành", Payment = "VNPay", IsPaid = true, DaysAgo = 4,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[5], Qty = 2 }, new SeedOrderItemEntry { Prod = allProds[6], Qty = 1 } }
                },
                new SeedOrderEntry
                {
                    User = vip2, Status = "Đang giao", Payment = "COD", IsPaid = false, DaysAgo = 1,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[8], Qty = 2 } }
                },
                new SeedOrderEntry
                {
                    User = cust1, Status = "Đang xử lý", Payment = "VNPay", IsPaid = true, DaysAgo = 1,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[0], Qty = 1 } }
                },
                new SeedOrderEntry
                {
                    User = cust1, Status = "Chờ xác nhận", Payment = "COD", IsPaid = false, DaysAgo = 0,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[6], Qty = 1 } }
                },
                new SeedOrderEntry
                {
                    User = vip2, Status = "Đã hủy", Payment = "COD", IsPaid = false, DaysAgo = 5,
                    Items = new List<SeedOrderItemEntry> { new SeedOrderItemEntry { Prod = allProds[7], Qty = 1 } }
                }
            };

            foreach (var so in sampleOrders)
            {
                decimal sub = so.Items.Sum(i => i.Prod.Price * i.Qty);
                decimal ship = 30000;
                decimal total = sub + ship;
                DateTime created = DateTime.Now.AddDays(-so.DaysAgo).AddHours(-2);

                var order = new Order
                {
                    UserId = so.User.Id,
                    RecipientName = so.User.FullName,
                    RecipientPhone = so.User.Phone ?? "0909000111",
                    DeliveryAddress = so.User.Address,
                    DeliveryDate = created.AddHours(3),
                    ShippingCarrier = "Giao Hoa Hỏa Tốc 2H (CHAM Express)",
                    ShippingFee = ship,
                    PaymentMethod = so.Payment,
                    Total = total,
                    Status = so.Status,
                    Note = "Giao hoa tươi kèm thiệp chúc mừng viết tay giúp mình nhé!",
                    VnPayTransactionNo = so.Payment == "VNPay" && so.IsPaid ? "VNP" + rng.Next(10000000, 99999999) : null,
                    PaidAmount = so.IsPaid ? (decimal?)total : null,
                    PaidDate = so.IsPaid ? (DateTime?)created.AddMinutes(5) : null,
                    IsPaid = so.IsPaid,
                    StockDeducted = so.Status == "Hoàn thành",
                    CreatedAt = created
                };

                context.Orders.Add(order);
                context.SaveChanges();

                foreach (var it in so.Items)
                {
                    context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = it.Prod.Id,
                        Quantity = it.Qty,
                        UnitPrice = it.Prod.Price
                    });
                }
                context.SaveChanges();
            }

            // 11. Seed Reviews & ProductReviews
            var rev1 = new Review
            {
                ProductId = allProds[0].Id,
                UserId = vip1.Id,
                Rating = 5,
                Content = "Hoa hồng trà cụm bông cực to, thơm dịu và gói giấy rất sang. Giao đúng giờ hẹn trong 90 phút, người nhận rất thích!",
                Status = ReviewStatus.Approved,
                IsApproved = true,
                CreatedAt = DateTime.Now.AddDays(-14)
            };
            var rev2 = new Review
            {
                ProductId = allProds[1].Id,
                UserId = cust1.Id,
                Rating = 5,
                Content = "Bó hoa pastel ở ngoài còn xinh hơn trong hình, shop tặng kèm thiệp chữ viết tay cực kỳ chỉn chu.",
                Status = ReviewStatus.Approved,
                IsApproved = true,
                CreatedAt = DateTime.Now.AddDays(-7)
            };
            var rev3 = new Review
            {
                ProductId = allProds[2].Id,
                UserId = vip1.Id,
                Rating = 5,
                Content = "Hộp hoa cẩm tú cầu xanh phối lan hồ điệp chưng ở sảnh công ty ai đi ngang cũng khen, rất sang trọng.",
                Status = ReviewStatus.Approved,
                IsApproved = true,
                CreatedAt = DateTime.Now.AddDays(-10)
            };
            var revPending1 = new Review
            {
                ProductId = allProds[5].Id,
                UserId = vip1.Id,
                Rating = 5,
                Content = "Tulip Hà Lan cánh cứng cáp, còn ngậm sương mát lạnh khi nhận. Sẽ tiếp tục ủng hộ CHAM Flowers!",
                Status = ReviewStatus.Pending,
                IsApproved = false,
                CreatedAt = DateTime.Now.AddHours(-6)
            };
            var revPending2 = new Review
            {
                ProductId = allProds[3].Id,
                UserId = vip2.Id,
                Rating = 4,
                Content = "Hoa khai trương rất rạng rỡ, shipper gửi ảnh nghiệm thu trước khi giao rất chuyên nghiệp.",
                Status = ReviewStatus.Pending,
                IsApproved = false,
                CreatedAt = DateTime.Now.AddHours(-3)
            };

            context.Reviews.AddRange(new[] { rev1, rev2, rev3, revPending1, revPending2 });
            context.ProductReviews.AddRange(new[]
            {
                new ProductReview { ProductId = allProds[0].Id, UserId = vip1.Id, Rating = 5, Comment = rev1.Content, CreatedAt = rev1.CreatedAt },
                new ProductReview { ProductId = allProds[1].Id, UserId = cust1.Id, Rating = 5, Comment = rev2.Content, CreatedAt = rev2.CreatedAt }
            });

            // 12. Seed ChatConversations & ChatMessages
            var conv1 = new ChatConversation
            {
                UserId = cust1.Id,
                CustomerName = cust1.FullName,
                CustomerEmail = cust1.Email,
                SessionKey = "seed_session_thuha",
                Status = "Đang xử lý",
                IsCustomerOnline = true,
                UnreadAdminCount = 1,
                LastMessageAt = DateTime.Now.AddMinutes(-12),
                CreatedAt = DateTime.Now.AddMinutes(-30)
            };
            var conv2 = new ChatConversation
            {
                UserId = vip1.Id,
                CustomerName = vip1.FullName,
                CustomerEmail = vip1.Email,
                SessionKey = "seed_session_minhanh",
                Status = "Đã giải quyết",
                IsCustomerOnline = false,
                UnreadAdminCount = 0,
                LastMessageAt = DateTime.Now.AddHours(-4),
                CreatedAt = DateTime.Now.AddHours(-5)
            };

            context.ChatConversations.AddRange(new[] { conv1, conv2 });
            context.SaveChanges();

            context.ChatMessages.AddRange(new[]
            {
                new ChatMessage
                {
                    ConversationId = conv1.Id,
                    SenderRole = "System",
                    SenderName = "Hệ thống CHAM Flowers",
                    Content = "Chào mừng quý khách đến với kênh tư vấn hoa tươi trực tuyến CHAM Flowers!",
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    IsRead = true
                },
                new ChatMessage
                {
                    ConversationId = conv1.Id,
                    SenderRole = "Customer",
                    SenderName = cust1.FullName,
                    Content = "Shop ơi mình muốn đặt 1 bó hoa sinh nhật tông hồng pastel giao lúc 16h chiều nay ở Phú Nhuận kịp không ạ?",
                    CreatedAt = DateTime.Now.AddMinutes(-25),
                    IsRead = true
                },
                new ChatMessage
                {
                    ConversationId = conv1.Id,
                    SenderRole = "Admin",
                    SenderName = "CHAM Flowers CSKH",
                    Content = "Dạ chào chị Thu Hà! CHAM Flowers có dịch vụ giao hỏa tốc 2h nội thành nên 16h chiều nay hoàn toàn kịp ạ. Em gửi chị mẫu Bó Hoa Hồng Pastel đang giảm giá nhé!",
                    MessageType = "ProductLink",
                    AttachmentUrl = string.Format("/Product/Details/{0}", allProds[1].Id),
                    CreatedAt = DateTime.Now.AddMinutes(-20),
                    IsRead = true
                },
                new ChatMessage
                {
                    ConversationId = conv1.Id,
                    SenderRole = "Customer",
                    SenderName = cust1.FullName,
                    Content = "Mẫu này xinh quá! Shop có mã giảm giá nào cho đơn hôm nay không gửi mình áp luôn nha.",
                    CreatedAt = DateTime.Now.AddMinutes(-12),
                    IsRead = false
                },
                new ChatMessage
                {
                    ConversationId = conv2.Id,
                    SenderRole = "Customer",
                    SenderName = vip1.FullName,
                    Content = "Cảm ơn shop nhé, đối tác bên mình nhận được hộp hoa Lan Hồ Điệp khen rất đẹp!",
                    CreatedAt = DateTime.Now.AddHours(-4),
                    IsRead = true
                },
                new ChatMessage
                {
                    ConversationId = conv2.Id,
                    SenderRole = "Admin",
                    SenderName = "CHAM Flowers CSKH",
                    Content = "CHAM Flowers cảm ơn chị Minh Anh đã luôn tin tưởng ủng hộ ạ! Chúc chị một ngày làm việc nhiều niềm vui!",
                    CreatedAt = DateTime.Now.AddHours(-4).AddMinutes(3),
                    IsRead = true
                }
            });

            context.SaveChanges();
        }

        private static void EnsureMenuItemsTableAndSeed(FlowerShopDbContext context)
        {
            try
            {
                // 1. Tạo bảng MenuItems nếu chưa có trong SQL Server
                string sqlCreateTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MenuItems')
                    BEGIN
                        CREATE TABLE [dbo].[MenuItems] (
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [Name] NVARCHAR(100) NOT NULL,
                            [Url] NVARCHAR(255) NOT NULL,
                            [Icon] NVARCHAR(50) NULL,
                            [BadgeText] NVARCHAR(30) NULL,
                            [BadgeColor] NVARCHAR(30) NULL DEFAULT 'danger',
                            [DisplayOrder] INT NOT NULL DEFAULT 0,
                            [OpenNewTab] BIT NOT NULL DEFAULT 0,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [ParentId] INT NULL,
                            [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                        );
                    END
                ";
                context.Database.ExecuteSqlCommand(sqlCreateTable);

                // 2. Nạp dữ liệu mẫu nếu bảng MenuItems trống
                int count = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[MenuItems]").FirstOrDefault();
                if (count == 0)
                {
                    string sqlSeed = @"
                        INSERT INTO [dbo].[MenuItems] ([Name], [Url], [Icon], [BadgeText], [BadgeColor], [DisplayOrder], [OpenNewTab], [IsActive], [CreatedAt])
                        VALUES 
                        (N'Trang chủ', '/', 'bi-house-door', NULL, 'danger', 1, 0, 1, GETDATE()),
                        (N'Sản phẩm hoa', '/Product', 'bi-flower1', NULL, 'danger', 2, 0, 1, GETDATE()),
                        (N'Hot Deal', '/HotDeal', 'bi-fire', 'HOT', 'danger', 3, 0, 1, GETDATE()),
                        (N'Tin tức & Cẩm nang', '/Home/News', 'bi-newspaper', NULL, 'danger', 4, 0, 1, GETDATE()),
                        (N'Giới thiệu', '/Home/About', 'bi-info-circle', NULL, 'danger', 5, 0, 1, GETDATE()),
                        (N'Giỏ hàng', '/Order/Cart', 'bi-bag-heart', NULL, 'danger', 6, 0, 1, GETDATE());
                    ";
                    context.Database.ExecuteSqlCommand(sqlSeed);
                }
            }
            catch
            {
                // Bỏ qua nếu lỗi phát sinh
            }
        }
    }
}
```

### File: FlowerShop/Models\PagedList.cs
```csharp
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PagedList
{
    public interface IPagedList : IEnumerable
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
        int FirstItemOnPage { get; }
        int LastItemOnPage { get; }
    }

    public interface IPagedList<out T> : IPagedList, IEnumerable<T>
    {
        T this[int index] { get; }
        int Count { get; }
    }

    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int PageCount { get; private set; }
        public int TotalItemCount { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public bool HasPreviousPage { get { return PageNumber > 1; } }
        public bool HasNextPage { get { return PageNumber < PageCount; } }
        public bool IsFirstPage { get { return PageNumber == 1; } }
        public bool IsLastPage { get { return PageNumber >= PageCount; } }
        public int FirstItemOnPage { get { return TotalItemCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1; } }
        public int LastItemOnPage { get { return TotalItemCount == 0 ? 0 : Math.Min(PageNumber * PageSize, TotalItemCount); } }

        public PagedList(IQueryable<T> superset, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalItemCount = superset.Count();
            PageCount = TotalItemCount > 0
                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                : 1;

            if (PageNumber > PageCount)
            {
                PageNumber = PageCount;
            }

            if (TotalItemCount > 0)
            {
                AddRange(superset.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList());
            }
        }

        public PagedList(IEnumerable<T> superset, int pageNumber, int pageSize)
        {
            var list = superset.ToList();
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalItemCount = list.Count;
            PageCount = TotalItemCount > 0
                ? (int)Math.Ceiling(TotalItemCount / (double)PageSize)
                : 1;

            if (PageNumber > PageCount)
            {
                PageNumber = PageCount;
            }

            if (TotalItemCount > 0)
            {
                AddRange(list.Skip((PageNumber - 1) * PageSize).Take(PageSize));
            }
        }
    }

    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> superset, int pageNumber, int pageSize)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }

        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> superset, int pageNumber, int pageSize)
        {
            return new PagedList<T>(superset, pageNumber, pageSize);
        }
    }
}
```

### File: FlowerShop/Services\AppHelpers.cs
```csharp
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FlowerShop.Services
{
    public class ChamPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public string Role { get; private set; }
        public int UserId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public bool IsVip { get; private set; }

        public ChamPrincipal(IIdentity identity, string role, int userId, string email, bool isVip)
        {
            Identity = identity;
            Role = role ?? "Customer";
            UserId = userId;
            Email = email ?? string.Empty;
            FullName = identity != null ? identity.Name : string.Empty;
            IsVip = isVip;
        }

        public bool IsInRole(string role)
        {
            return string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class AppHelpers
    {
        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

        public static int? GetCurrentUserId(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            if (cp != null && cp.UserId > 0)
                return cp.UserId;
            return null;
        }

        public static string GetCurrentEmail(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            return cp != null ? cp.Email : string.Empty;
        }

        public static string GetCurrentFullName(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            if (cp != null && !string.IsNullOrWhiteSpace(cp.FullName))
                return cp.FullName;
            return user != null && user.Identity != null && !string.IsNullOrWhiteSpace(user.Identity.Name)
                ? user.Identity.Name
                : string.Empty;
        }

        public static bool GetCurrentIsVip(IPrincipal user)
        {
            var cp = user as ChamPrincipal;
            return cp != null && cp.IsVip;
        }

        public static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return "KH";

            string[] parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                string first = parts[parts.Length - 2];
                string second = parts[parts.Length - 1];
                return (first.Substring(0, 1) + second.Substring(0, 1)).ToUpperInvariant();
            }
            if (parts.Length == 1 && parts[0].Length > 0)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }
            return "KH";
        }

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                hash = pbkdf2.GetBytes(32);
            }

            return string.Format("PBKDF2${0}${1}", Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            if (!storedHash.StartsWith("PBKDF2$", StringComparison.Ordinal))
            {
                return storedHash == password;
            }

            string[] parts = storedHash.Split('$');
            if (parts.Length != 3)
                return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expectedHash = Convert.FromBase64String(parts[2]);

            byte[] actualHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                actualHash = pbkdf2.GetBytes(32);
            }

            return FixedTimeEquals(actualHash, expectedHash);
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        // Kiểm tra định dạng file ảnh theo đúng Mục 3.1.2 & kịch bản kiểm thử SP04
        public static bool IsImageValid(HttpPostedFileBase imageFile)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
                return false;

            string ext = Path.GetExtension(imageFile.FileName);
            if (string.IsNullOrEmpty(ext))
                return false;

            ext = ext.ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
                return false;

            if (imageFile.ContentLength > 10 * 1024 * 1024) // 10 MB
                return false;

            return true;
        }

        public static string SaveUploadedImage(HttpPostedFileBase file, string webRootPath, string subFolder = "products")
        {
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string uploadsDir = Path.Combine(webRootPath, "uploads", subFolder);
            Directory.CreateDirectory(uploadsDir);

            string fileName = string.Format("{0:yyyyMMddHHmmss}_{1:N}{2}", DateTime.Now, Guid.NewGuid(), ext);
            string fullPath = Path.Combine(uploadsDir, fileName);

            file.SaveAs(fullPath);

            return string.Format("/uploads/{0}/{1}", subFolder, fileName);
        }

        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "san-pham-" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string normalized = input.Trim().ToLowerInvariant()
                .Replace('đ', 'd')
                .Replace('Đ', 'd');

            string formD = normalized.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char ch in formD)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            string clean = sb.ToString().Normalize(NormalizationForm.FormC);
            clean = Regex.Replace(clean, @"[^a-z0-9\s-]", "");
            clean = Regex.Replace(clean, @"\s+", "-").Trim('-');
            clean = Regex.Replace(clean, @"-+", "-");

            return string.IsNullOrEmpty(clean) ? "san-pham" : clean;
        }

        public static string FormatVnd(decimal amount)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N0} ₫", amount).Replace(",", ".");
        }
    }
}
```

### File: FlowerShop/Services\VnPayService.cs
```csharp
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using FlowerShop.Models;

namespace FlowerShop.Services
{
    public class VnPayService
    {
        private static readonly Random Rng = new Random();
        private static readonly object RngLock = new object();

        private static int NextRandom8Digits()
        {
            lock (RngLock)
            {
                return Rng.Next(10000000, 99999999);
            }
        }

        public string CreatePaymentUrl(Order order, HttpRequestBase request)
        {
            string tmnCode = ConfigurationManager.AppSettings["VnPay:TmnCode"] ?? "CHAMFL01";
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            bool useLocalGateway = !string.Equals(ConfigurationManager.AppSettings["VnPay:UseLocalSandboxSimulator"], "false", StringComparison.OrdinalIgnoreCase);

            string scheme = request.Url != null ? request.Url.Scheme : "http";
            string host = request.Url != null ? request.Url.Authority : "localhost:5188";
            string returnUrl = string.Format("{0}://{1}/Payment/VnPayReturn", scheme, host);
            string baseUrl = useLocalGateway
                ? string.Format("{0}://{1}/Payment/VnPayGateway", scheme, host)
                : (ConfigurationManager.AppSettings["VnPay:BaseUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");

            var vnpParams = new SortedList<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", tmnCode },
                { "vnp_Amount", ((long)(order.Total * 100)).ToString(CultureInfo.InvariantCulture) },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", !string.IsNullOrEmpty(request.UserHostAddress) ? request.UserHostAddress : "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", string.Format("Thanh toan don hang hoa tuoi CHAM Flowers #{0}", order.Id) },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", returnUrl },
                { "vnp_TxnRef", order.Id.ToString(CultureInfo.InvariantCulture) }
            };

            var queryBuilder = new StringBuilder();
            foreach (var kv in vnpParams)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    queryBuilder.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string signData = queryBuilder.ToString().TrimEnd('&');
            string secureHash = HmacSha512(hashSecret, signData);

            return string.Format("{0}?{1}&vnp_SecureHash={2}", baseUrl, signData, secureHash);
        }

        public string CreateSimulatedCallbackUrl(NameValueCollection gatewayQuery, bool success, HttpRequestBase request)
        {
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            string txnRef = gatewayQuery["vnp_TxnRef"] ?? string.Empty;
            string amount = gatewayQuery["vnp_Amount"] ?? string.Empty;
            string orderInfo = gatewayQuery["vnp_OrderInfo"] ?? string.Empty;
            string tmnCode = gatewayQuery["vnp_TmnCode"] ?? string.Empty;

            var callbackParams = new SortedList<string, string>(StringComparer.Ordinal)
            {
                { "vnp_Amount", amount },
                { "vnp_BankCode", "NCB" },
                { "vnp_BankTranNo", "VNP" + DateTime.Now.ToString("MMddHHmmss") },
                { "vnp_CardType", "ATM" },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_PayDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_ResponseCode", success ? "00" : "24" },
                { "vnp_TmnCode", string.IsNullOrEmpty(tmnCode) ? "CHAMFL01" : tmnCode },
                { "vnp_TransactionNo", NextRandom8Digits().ToString(CultureInfo.InvariantCulture) },
                { "vnp_TransactionStatus", success ? "00" : "02" },
                { "vnp_TxnRef", txnRef }
            };

            var sb = new StringBuilder();
            foreach (var kv in callbackParams)
            {
                sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
            }

            string signData = sb.ToString().TrimEnd('&');
            string secureHash = HmacSha512(hashSecret, signData);

            string scheme = request.Url != null ? request.Url.Scheme : "http";
            string host = request.Url != null ? request.Url.Authority : "localhost:5188";
            return string.Format("{0}://{1}/Payment/VnPayReturn?{2}&vnp_SecureHash={3}", scheme, host, signData, secureHash);
        }

        public bool ValidateSignature(NameValueCollection query)
        {
            string hashSecret = ConfigurationManager.AppSettings["VnPay:HashSecret"] ?? "CHAMFLOWERSVNPAYSECRETKEY2026SECURE";
            string inputHash = query["vnp_SecureHash"] ?? string.Empty;
            if (string.IsNullOrEmpty(inputHash))
                return false;

            var sorted = new SortedList<string, string>(StringComparer.Ordinal);
            foreach (string key in query.AllKeys)
            {
                if (!string.IsNullOrEmpty(key) &&
                    key.StartsWith("vnp_", StringComparison.Ordinal) &&
                    key != "vnp_SecureHash" &&
                    key != "vnp_SecureHashType")
                {
                    sorted[key] = query[key] ?? string.Empty;
                }
            }

            var sb = new StringBuilder();
            foreach (var kv in sorted)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    sb.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }

            string signData = sb.ToString().TrimEnd('&');
            string expectedHash = HmacSha512(hashSecret, signData);

            return string.Equals(inputHash, expectedHash, StringComparison.OrdinalIgnoreCase);
        }

        public static string HmacSha512(string key, string inputData)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hashValue).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
```

### File: FlowerShop/Hubs\ChatHub.cs
```csharp
using System.Linq;
using System.Threading.Tasks;
using FlowerShop.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace FlowerShop.Hubs
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {
        public async Task JoinCustomerRoom(string sessionKey)
        {
            if (string.IsNullOrWhiteSpace(sessionKey))
                return;

            await Groups.Add(Context.ConnectionId, "conv_" + sessionKey);

            using (var db = new FlowerShopDbContext())
            {
                var conv = db.ChatConversations.FirstOrDefault(c => c.SessionKey == sessionKey);
                if (conv != null)
                {
                    conv.IsCustomerOnline = true;
                    db.SaveChanges();
                    Clients.Group("Admins").customerStatusChanged(conv.Id, true);
                }
            }
        }

        public Task JoinAdminChannel()
        {
            return Groups.Add(Context.ConnectionId, "Admins");
        }
    }
}
```

### File: FlowerShop/Controllers\HomeController.cs
```csharp
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

            DateTime now = DateTime.Now;
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
```

### File: FlowerShop/Controllers\ProductController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // ==================== PHÍA KHÁCH HÀNG ====================

        public ActionResult Index(
            string search,
            int? categoryId,
            int? flowerCategoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string sortOrder,
            int page = 1)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.FlowerCategory)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    (p.Description != null && p.Description.ToLower().Contains(kw)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (flowerCategoryId.HasValue && flowerCategoryId.Value > 0)
            {
                query = query.Where(p => p.FlowerCategoryId == flowerCategoryId.Value);
            }

            if (minPrice.HasValue && minPrice.Value >= 0)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "featured":
                    query = query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }

            ViewBag.Categories = _context.Categories.Where(c => c.IsActive).ToList();
            ViewBag.FlowerCategories = _context.FlowerCategories.ToList();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.FlowerCategoryId = flowerCategoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortOrder = sortOrder;

            var paged = query.ToPagedList(page, 9);
            return View(paged);
        }

        public ActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.FlowerCategory)
                .Include(p => p.Supplier)
                .Include(p => p.Images)
                .Include("Reviews.User")
                .FirstOrDefault(p => p.Id == id && p.IsActive);

            if (product == null)
                return HttpNotFound();

            var approvedReviews = product.Reviews
                .Where(r => r.IsApproved && r.Status != ReviewStatus.Hidden)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var related = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsActive && p.Id != id && p.CategoryId == product.CategoryId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToList();

            bool canReview = false;
            bool hasReviewed = false;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                int? userId = AppHelpers.GetCurrentUserId(User);
                if (userId.HasValue)
                {
                    int uid = userId.Value;
                    hasReviewed = _context.Reviews.Any(r => r.ProductId == id && r.UserId == uid);

                    bool hasCompletedPurchase = _context.Orders
                        .Include(o => o.OrderItems)
                        .Any(o => o.UserId == uid &&
                                  o.Status == "Hoàn thành" &&
                                  o.OrderItems.Any(oi => oi.ProductId == id));

                    canReview = hasCompletedPurchase && !hasReviewed;
                }
            }

            var vm = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = related,
                ApprovedReviews = approvedReviews,
                CanReview = canReview,
                HasReviewed = hasReviewed
            };

            return View(vm);
        }

        // ==================== PHÍA QUẢN TRỊ VIÊN (ADMIN) ====================

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex(string search, int? categoryId, int page = 1)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.FlowerCategory)
                .Include(p => p.Supplier)
                .Include(p => p.Images)
                .Include(p => p.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(kw) || p.Slug.Contains(kw));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            query = query.OrderByDescending(p => p.CreatedAt);

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(query.ToPagedList(page, 10));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create()
        {
            PopulateDropdowns();
            return View(new Product { IsActive = true, StockQuantity = 20, ExpiryDate = DateTime.Now.AddDays(7) });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model, HttpPostedFileBase mainImageFile, IEnumerable<HttpPostedFileBase> galleryFiles, string customImageUrl)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Tên sản phẩm không được để trống.");
            }

            if (model.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá bán không hợp lệ. Giá sản phẩm phải lớn hơn 0.");
            }

            if (model.OriginalPrice.HasValue && model.OriginalPrice.Value < 0)
            {
                ModelState.AddModelError("OriginalPrice", "Giá gốc không được là số âm.");
            }

            if (model.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Số lượng tồn kho không được nhỏ hơn 0.");
            }

            if (mainImageFile != null && mainImageFile.ContentLength > 0 && !AppHelpers.IsImageValid(mainImageFile))
            {
                ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif, webp, svg.");
            }

            if (galleryFiles != null)
            {
                foreach (var gf in galleryFiles.Where(f => f != null && f.ContentLength > 0))
                {
                    if (!AppHelpers.IsImageValid(gf))
                    {
                        ModelState.AddModelError("", string.Format("File '{0}' không đúng định dạng ảnh hợp lệ.", gf.FileName));
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            string webRoot = Server.MapPath("~/");

            model.Name = model.Name.Trim();
            model.Slug = EnsureUniqueSlug(AppHelpers.GenerateSlug(model.Name));
            model.CreatedAt = DateTime.Now;

            _context.Products.Add(model);
            _context.SaveChanges();

            string mainUrl = "/images/samflora/b10-main.jpg";
            if (mainImageFile != null && mainImageFile.ContentLength > 0)
            {
                mainUrl = AppHelpers.SaveUploadedImage(mainImageFile, webRoot, "products");
            }
            else if (!string.IsNullOrWhiteSpace(customImageUrl))
            {
                mainUrl = customImageUrl.Trim();
            }

            _context.ProductImages.Add(new ProductImage
            {
                ProductId = model.Id,
                ImageUrl = mainUrl,
                IsMain = true
            });

            if (galleryFiles != null)
            {
                foreach (var gf in galleryFiles.Where(f => f != null && f.ContentLength > 0))
                {
                    string subUrl = AppHelpers.SaveUploadedImage(gf, webRoot, "products");
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = model.Id,
                        ImageUrl = subUrl,
                        IsMain = false
                    });
                }
            }

            _context.SaveChanges();
            TempData["Success"] = string.Format("Đã thêm sản phẩm \"{0}\" thành công!", model.Name);
            return RedirectToAction("AdminIndex");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var product = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return HttpNotFound();

            PopulateDropdowns(product);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            int id,
            Product model,
            HttpPostedFileBase mainImageFile,
            IEnumerable<HttpPostedFileBase> galleryFiles,
            List<int> deleteImageIds,
            string customImageUrl)
        {
            var existing = _context.Products
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (existing == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError("Name", "Vui lòng nhập tên sản phẩm.");
            }

            if (model.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá bán phải lớn hơn 0.");
            }

            if (model.StockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Tồn kho không được âm.");
            }

            if (mainImageFile != null && mainImageFile.ContentLength > 0 && !AppHelpers.IsImageValid(mainImageFile))
            {
                ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif, webp, svg.");
            }

            if (!ModelState.IsValid)
            {
                model.Images = existing.Images;
                PopulateDropdowns(model);
                return View(model);
            }

            string webRoot = Server.MapPath("~/");

            existing.Name = model.Name.Trim();
            existing.Slug = EnsureUniqueSlug(AppHelpers.GenerateSlug(existing.Name), existing.Id);
            existing.CategoryId = model.CategoryId;
            existing.FlowerCategoryId = model.FlowerCategoryId;
            existing.SupplierId = model.SupplierId;
            existing.Price = model.Price;
            existing.OriginalPrice = model.OriginalPrice;
            existing.StockQuantity = model.StockQuantity;
            existing.ExpiryDate = model.ExpiryDate;
            existing.Description = model.Description;
            existing.IsFeatured = model.IsFeatured;
            existing.IsHotDeal = model.IsHotDeal;
            existing.IsActive = model.IsActive;
            existing.UpdatedAt = DateTime.Now;

            if (deleteImageIds != null && deleteImageIds.Any())
            {
                var toRemove = existing.Images.Where(img => deleteImageIds.Contains(img.Id)).ToList();
                _context.ProductImages.RemoveRange(toRemove);
            }

            if (mainImageFile != null && mainImageFile.ContentLength > 0)
            {
                string newMainUrl = AppHelpers.SaveUploadedImage(mainImageFile, webRoot, "products");
                foreach (var img in existing.Images)
                {
                    img.IsMain = false;
                }
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = existing.Id,
                    ImageUrl = newMainUrl,
                    IsMain = true
                });
            }
            else if (!string.IsNullOrWhiteSpace(customImageUrl))
            {
                var currentMain = existing.Images.FirstOrDefault(i => i.IsMain);
                if (currentMain != null)
                {
                    currentMain.ImageUrl = customImageUrl.Trim();
                }
                else
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = existing.Id,
                        ImageUrl = customImageUrl.Trim(),
                        IsMain = true
                    });
                }
            }

            if (galleryFiles != null)
            {
                foreach (var gf in galleryFiles.Where(f => f != null && f.ContentLength > 0 && AppHelpers.IsImageValid(f)))
                {
                    string subUrl = AppHelpers.SaveUploadedImage(gf, webRoot, "products");
                    _context.ProductImages.Add(new ProductImage
                    {
                        ProductId = existing.Id,
                        ImageUrl = subUrl,
                        IsMain = false
                    });
                }
            }

            _context.SaveChanges();
            TempData["Success"] = string.Format("Cập nhật sản phẩm \"{0}\" thành công!", existing.Name);
            return RedirectToAction("AdminIndex");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm cần xóa.";
                return RedirectToAction("AdminIndex");
            }

            bool hasOrderConstraint = _context.OrderItems.Any(oi => oi.ProductId == id);
            if (hasOrderConstraint)
            {
                TempData["Error"] = string.Format("Không thể xóa sản phẩm \"{0}\" vì sản phẩm đang liên kết với đơn hàng trong hệ thống! Bạn có thể chuyển sang trạng thái 'Ngừng kinh doanh'.", product.Name);
                return RedirectToAction("AdminIndex");
            }

            foreach (var hp in _context.HotDealPopups.Where(h => h.ProductId == id).ToList())
            {
                hp.ProductId = null;
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã xóa sản phẩm \"{0}\" khỏi hệ thống.", product.Name);
            return RedirectToAction("AdminIndex");
        }

        // ==================== QUẢN LÝ DANH MỤC, LOẠI HOA & NHÀ CUNG CẤP ====================

        [Authorize(Roles = "Admin")]
        public ActionResult Categories()
        {
            ViewBag.Categories = _context.Categories.Include(c => c.Products).OrderBy(c => c.Id).ToList();
            ViewBag.FlowerCategories = _context.FlowerCategories.Include(fc => fc.Products).OrderBy(fc => fc.Id).ToList();
            ViewBag.Suppliers = _context.Suppliers.Include(s => s.Products).OrderBy(s => s.Id).ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategory(int id, string name, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Tên danh mục không được để trống.";
                return RedirectToAction("Categories");
            }

            if (id == 0)
            {
                _context.Categories.Add(new Category { Name = name.Trim(), IsActive = isActive, CreatedAt = DateTime.Now });
                TempData["Success"] = "Đã thêm danh mục mới thành công!";
            }
            else
            {
                var cat = _context.Categories.Find(id);
                if (cat != null)
                {
                    cat.Name = name.Trim();
                    cat.IsActive = isActive;
                    cat.UpdatedAt = DateTime.Now;
                    TempData["Success"] = "Đã cập nhật danh mục thành công!";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            var cat = _context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
            if (cat == null)
                return RedirectToAction("Categories");

            if (cat.Products.Any())
            {
                TempData["Error"] = string.Format("Không thể xóa danh mục \"{0}\" vì đang chứa {1} sản phẩm.", cat.Name, cat.Products.Count);
                return RedirectToAction("Categories");
            }

            _context.Categories.Remove(cat);
            _context.SaveChanges();
            TempData["Success"] = "Đã xóa danh mục thành công.";
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFlowerCategory(int id, string name, string description, string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Tên chủng loại hoa không được để trống.";
                return RedirectToAction("Categories");
            }

            if (id == 0)
            {
                _context.FlowerCategories.Add(new FlowerCategory
                {
                    Name = name.Trim(),
                    Description = description != null ? description.Trim() : null,
                    ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? "/images/samflora/b10-main.jpg" : imageUrl.Trim(),
                    CreatedAt = DateTime.Now
                });
                TempData["Success"] = "Đã thêm loại hoa / bộ sưu tập mới!";
            }
            else
            {
                var fc = _context.FlowerCategories.Find(id);
                if (fc != null)
                {
                    fc.Name = name.Trim();
                    fc.Description = description != null ? description.Trim() : null;
                    if (!string.IsNullOrWhiteSpace(imageUrl))
                        fc.ImageUrl = imageUrl.Trim();
                    TempData["Success"] = "Đã cập nhật loại hoa!";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFlowerCategory(int id)
        {
            var fc = _context.FlowerCategories.Find(id);
            if (fc != null)
            {
                foreach (var p in _context.Products.Where(x => x.FlowerCategoryId == id).ToList())
                {
                    p.FlowerCategoryId = null;
                }
                _context.FlowerCategories.Remove(fc);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa chủng loại hoa.";
            }
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSupplier(int id, string name, string phone, string email, string address, bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Tên nhà cung cấp không được để trống.";
                return RedirectToAction("Categories");
            }

            if (id == 0)
            {
                _context.Suppliers.Add(new Supplier
                {
                    Name = name.Trim(),
                    Phone = phone != null ? phone.Trim() : null,
                    Email = email != null ? email.Trim() : null,
                    Address = address != null ? address.Trim() : null,
                    IsActive = isActive,
                    CreatedAt = DateTime.Now
                });
                TempData["Success"] = "Đã thêm nhà cung cấp mới!";
            }
            else
            {
                var sup = _context.Suppliers.Find(id);
                if (sup != null)
                {
                    sup.Name = name.Trim();
                    sup.Phone = phone != null ? phone.Trim() : null;
                    sup.Email = email != null ? email.Trim() : null;
                    sup.Address = address != null ? address.Trim() : null;
                    sup.IsActive = isActive;
                    sup.UpdatedAt = DateTime.Now;
                    TempData["Success"] = "Đã cập nhật thông tin nhà cung cấp!";
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Categories");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSupplier(int id)
        {
            var sup = _context.Suppliers.Find(id);
            if (sup != null)
            {
                foreach (var p in _context.Products.Where(x => x.SupplierId == id).ToList())
                {
                    p.SupplierId = null;
                }
                _context.Suppliers.Remove(sup);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa nhà cung cấp.";
            }
            return RedirectToAction("Categories");
        }

        private void PopulateDropdowns(Product model = null)
        {
            ViewBag.CategoryList = new SelectList(_context.Categories.ToList(), "Id", "Name", model != null ? (object)model.CategoryId : null);
            ViewBag.FlowerCategoryList = new SelectList(_context.FlowerCategories.ToList(), "Id", "Name", model != null ? (object)model.FlowerCategoryId : null);
            ViewBag.SupplierList = new SelectList(_context.Suppliers.ToList(), "Id", "Name", model != null ? (object)model.SupplierId : null);
        }

        private string EnsureUniqueSlug(string baseSlug, int? excludeId = null)
        {
            string slug = baseSlug;
            int counter = 1;
            while (_context.Products.Any(p => p.Slug == slug && (!excludeId.HasValue || p.Id != excludeId.Value)))
            {
                slug = string.Format("{0}-{1}", baseSlug, counter++);
            }
            return slug;
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
```

### File: FlowerShop/Controllers\OrderController.cs
```csharp
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        // ==================== GIỎ HÀNG ====================

        public ActionResult Cart()
        {
            var cart = GetOrCreateCart();
            decimal subTotal = cart.CartItems.Sum(i => i.UnitPrice * i.Quantity);

            string appliedCode = Session["AppliedCoupon"] as string;
            decimal discount = CalculateCouponDiscount(appliedCode, subTotal);

            DateTime now = DateTime.Now;
            var coupons = _context.Coupons
                .Where(c => c.IsActive && (!c.ExpiryDate.HasValue || c.ExpiryDate >= now))
                .ToList();

            var vm = new CartViewModel
            {
                Cart = cart,
                SubTotal = subTotal,
                ShippingFee = cart.CartItems.Any() ? 30000 : 0,
                AppliedCoupon = discount > 0 ? appliedCode : null,
                DiscountAmount = discount,
                AvailableCoupons = coupons
            };

            return View(vm);
        }

        [HttpGet]
        public ActionResult GetCartCount()
        {
            var cart = GetOrCreateCart(createIfMissing: false);
            int count = cart != null && cart.CartItems != null ? cart.CartItems.Sum(i => i.Quantity) : 0;
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            var product = _context.Products.FirstOrDefault(p => p.Id == productId && p.IsActive);
            if (product == null)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã ngừng kinh doanh." });
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index", "Product");
            }

            var cart = GetOrCreateCart();
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            int targetQty = (existingItem != null ? existingItem.Quantity : 0) + quantity;

            if (targetQty > product.StockQuantity)
            {
                string msg = product.StockQuantity <= 0
                    ? string.Format("Sản phẩm \"{0}\" hiện đã hết hàng!", product.Name)
                    : string.Format("Rất tiếc, kho chỉ còn {0} sản phẩm \"{1}\".", product.StockQuantity, product.Name);

                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = msg });

                TempData["Error"] = msg;
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            if (existingItem != null)
            {
                existingItem.Quantity = targetQty;
                existingItem.UnitPrice = product.Price;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                _context.CartItems.Add(newItem);
            }

            cart.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            int totalCount = _context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .Select(ci => (int?)ci.Quantity)
                .Sum() ?? 0;

            if (Request.IsAjaxRequest())
            {
                return Json(new
                {
                    success = true,
                    message = string.Format("Đã thêm \"{0}\" vào giỏ hàng!", product.Name),
                    cartCount = totalCount
                });
            }

            TempData["Success"] = string.Format("Đã thêm \"{0}\" vào giỏ hàng!", product.Name);
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCartItem(int cartItemId, int quantity)
        {
            var cart = GetOrCreateCart();
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item == null)
                return RedirectToAction("Cart");

            if (quantity <= 0)
            {
                _context.CartItems.Remove(item);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
            else
            {
                var prod = _context.Products.Find(item.ProductId);
                if (prod != null && quantity > prod.StockQuantity)
                {
                    TempData["Error"] = string.Format("Số lượng vượt quá tồn kho! \"{0}\" hiện chỉ còn {1} sản phẩm.", prod.Name, prod.StockQuantity);
                    return RedirectToAction("Cart");
                }
                item.Quantity = quantity;
                if (prod != null) item.UnitPrice = prod.Price;
                TempData["Success"] = "Đã cập nhật số lượng sản phẩm.";
            }

            cart.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveCartItem(int cartItemId)
        {
            var cart = GetOrCreateCart();
            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                cart.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyCoupon(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                Session.Remove("AppliedCoupon");
                TempData["Success"] = "Đã gỡ mã giảm giá.";
                return RedirectToAction("Cart");
            }

            string code = couponCode.Trim().ToUpperInvariant();
            var cart = GetOrCreateCart();
            decimal subTotal = cart.CartItems.Sum(i => i.UnitPrice * i.Quantity);

            var coupon = _context.Coupons.FirstOrDefault(c => c.Code.ToUpper() == code && c.IsActive);
            if (coupon == null || (coupon.ExpiryDate.HasValue && coupon.ExpiryDate < DateTime.Now))
            {
                TempData["Error"] = "Mã giảm giá không tồn tại hoặc đã hết hạn.";
                return RedirectToAction("Cart");
            }

            if (subTotal < coupon.MinOrderAmount)
            {
                TempData["Error"] = string.Format("Mã {0} áp dụng cho đơn tối thiểu {1:N0} ₫.", coupon.Code, coupon.MinOrderAmount);
                return RedirectToAction("Cart");
            }

            if (coupon.VipOnly)
            {
                bool isVip = AppHelpers.GetCurrentIsVip(User);
                if (!isVip)
                {
                    TempData["Error"] = string.Format("Mã {0} chỉ dành riêng cho Khách hàng VIP.", coupon.Code);
                    return RedirectToAction("Cart");
                }
            }

            Session["AppliedCoupon"] = coupon.Code;
            TempData["Success"] = string.Format("Áp dụng mã {0} thành công! ({1})", coupon.Code, coupon.Description);
            return RedirectToAction("Cart");
        }

        // ==================== ĐƠN HÀNG CỦA KHÁCH HÀNG ====================

        [Authorize]
        public ActionResult Checkout()
        {
            return RedirectToAction("Checkout", "Payment");
        }

        [Authorize]
        public ActionResult MyOrders(string status)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            int uid = userId.Value;
            var query = _context.Orders
                .Include("OrderItems.Product.Images")
                .Where(o => o.UserId == uid)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            ViewBag.CurrentStatus = status;
            var orders = query.OrderByDescending(o => o.CreatedAt).ToList();
            return View(orders);
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            bool isAdmin = User.IsInRole("Admin");

            var order = _context.Orders
                .Include(o => o.User)
                .Include("OrderItems.Product.Images")
                .FirstOrDefault(o => o.Id == id && (isAdmin || o.UserId == userId));

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            var order = _context.Orders.FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return HttpNotFound();

            if (order.Status != "Chờ xác nhận")
            {
                TempData["Error"] = string.Format("Không thể hủy đơn hàng #{0} khi đơn đang ở trạng thái \"{1}\"!", order.Id, order.Status);
                return RedirectToAction("Details", new { id = id });
            }

            order.Status = "Đã hủy";
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã hủy đơn hàng #{0} thành công.", order.Id);
            return RedirectToAction("MyOrders");
        }

        // ==================== QUẢN LÝ ĐƠN HÀNG PHÍA ADMIN ====================

        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex(string status, string search, int page = 1)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(o => o.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                int parsedId;
                bool isId = int.TryParse(kw.TrimStart('#'), out parsedId);
                query = query.Where(o =>
                    o.RecipientName.ToLower().Contains(kw) ||
                    o.RecipientPhone.Contains(kw) ||
                    (isId && o.Id == parsedId));
            }

            ViewBag.Status = status;
            ViewBag.Search = search;

            var paged = query.OrderByDescending(o => o.CreatedAt).ToPagedList(page, 10);
            return View(paged);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminDetails(int id)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include("OrderItems.Product.Images")
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateShippingInfo(int id, string recipientName, string recipientPhone, string deliveryAddress, string note)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(recipientName) || string.IsNullOrWhiteSpace(recipientPhone) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                TempData["Error"] = "Tên người nhận, số điện thoại và địa chỉ giao hàng không được để trống.";
                return RedirectToAction("AdminDetails", new { id = id });
            }

            order.RecipientName = recipientName.Trim();
            order.RecipientPhone = recipientPhone.Trim();
            order.DeliveryAddress = deliveryAddress.Trim();
            order.Note = note != null ? note.Trim() : null;

            _context.SaveChanges();
            TempData["Success"] = "Đã cập nhật địa chỉ và thông tin giao hàng thành công!";
            return RedirectToAction("AdminDetails", new { id = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveOrderItem(int orderId, int orderItemId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
                return HttpNotFound();

            if (order.Status == "Hoàn thành")
            {
                TempData["Error"] = "Không thể xóa sản phẩm khỏi đơn hàng đã hoàn thành.";
                return RedirectToAction("AdminDetails", new { id = orderId });
            }

            if (order.OrderItems.Count <= 1)
            {
                TempData["Error"] = "Đơn hàng phải có ít nhất 1 sản phẩm. Nếu muốn hủy, vui lòng chuyển trạng thái sang 'Đã hủy'.";
                return RedirectToAction("AdminDetails", new { id = orderId });
            }

            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == orderItemId);
            if (item != null)
            {
                _context.OrderItems.Remove(item);
                order.OrderItems.Remove(item);
                decimal sub = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
                order.Total = Math.Max(0, sub + order.ShippingFee - order.DiscountAmount);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa sản phẩm khỏi đơn hàng và cập nhật tổng tiền.";
            }

            return RedirectToAction("AdminDetails", new { id = orderId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int id, string newStatus)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include("OrderItems.Product")
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return HttpNotFound();

            if (newStatus == "Đã hủy" && (order.Status == "Hoàn thành" || order.Status == "Đang giao"))
            {
                TempData["Error"] = string.Format("Không cho phép hủy đơn hàng đang ở trạng thái \"{0}\"!", order.Status);
                return RedirectToAction("AdminDetails", new { id = id });
            }

            if ((newStatus == "Đang xử lý" || newStatus == "Đang giao" || newStatus == "Hoàn thành") && !order.StockDeducted)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null && item.Quantity > item.Product.StockQuantity)
                    {
                        TempData["Error"] = string.Format("Không đủ tồn kho cho sản phẩm \"{0}\" (Cần: {1}, Tồn: {2}).", item.Product.Name, item.Quantity, item.Product.StockQuantity);
                        return RedirectToAction("AdminDetails", new { id = id });
                    }
                }
            }

            if (newStatus == "Hoàn thành" && !order.StockDeducted)
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.StockQuantity = Math.Max(0, item.Product.StockQuantity - item.Quantity);
                    }
                }

                order.StockDeducted = true;
                order.IsPaid = true;
                if (!order.PaidAmount.HasValue) order.PaidAmount = order.Total;
                if (!order.PaidDate.HasValue) order.PaidDate = DateTime.Now;

                if (order.User != null)
                {
                    order.User.TotalSpent += order.Total;
                    if (order.User.TotalSpent >= 2000000)
                    {
                        order.User.IsVip = true;
                    }
                }
            }

            order.Status = newStatus;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã cập nhật trạng thái đơn hàng #{0} sang \"{1}\"!", order.Id, newStatus);
            return RedirectToAction("AdminDetails", new { id = id });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AdminCreate()
        {
            ViewBag.Products = _context.Products.Where(p => p.IsActive).OrderBy(p => p.Name).ToList();
            ViewBag.Customers = _context.Users.Where(u => u.Role == "Customer" && u.IsActive).OrderBy(u => u.FullName).ToList();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminCreate(
            int? userId,
            string recipientName,
            string recipientPhone,
            string deliveryAddress,
            int productId,
            int quantity,
            string paymentMethod = "COD",
            string note = null)
        {
            if (string.IsNullOrWhiteSpace(recipientName) || string.IsNullOrWhiteSpace(recipientPhone) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ họ tên, số điện thoại và địa chỉ người nhận.";
                return RedirectToAction("AdminCreate");
            }

            var prod = _context.Products.Find(productId);
            if (prod == null || quantity <= 0)
            {
                TempData["Error"] = "Vui lòng chọn sản phẩm và số lượng hợp lệ.";
                return RedirectToAction("AdminCreate");
            }

            if (quantity > prod.StockQuantity)
            {
                TempData["Error"] = string.Format("Hết hàng / Vượt tồn kho! Sản phẩm \"{0}\" hiện chỉ còn {1} sản phẩm trong kho.", prod.Name, prod.StockQuantity);
                return RedirectToAction("AdminCreate");
            }

            decimal ship = 30000;
            decimal total = prod.Price * quantity + ship;

            var order = new Order
            {
                UserId = userId,
                RecipientName = recipientName.Trim(),
                RecipientPhone = recipientPhone.Trim(),
                DeliveryAddress = deliveryAddress.Trim(),
                DeliveryDate = DateTime.Now.AddHours(2),
                ShippingCarrier = "Giao Hoa Hỏa Tốc 2H (CHAM Express)",
                ShippingFee = ship,
                PaymentMethod = paymentMethod,
                Total = total,
                Status = "Chờ xác nhận",
                Note = note != null ? note.Trim() : null,
                IsPaid = false,
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = prod.Id,
                Quantity = quantity,
                UnitPrice = prod.Price
            });
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã tạo đơn hàng #{0} thành công!", order.Id);
            return RedirectToAction("AdminDetails", new { id = order.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
                return RedirectToAction("AdminIndex");

            if (order.Status == "Hoàn thành" || order.Status == "Đang giao")
            {
                TempData["Error"] = "Không thể xóa đơn hàng đang giao hoặc đã hoàn thành để đảm bảo toàn vẹn sổ sách.";
                return RedirectToAction("AdminDetails", new { id = id });
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            TempData["Success"] = string.Format("Đã xóa đơn hàng #{0} khỏi hệ thống.", id);
            return RedirectToAction("AdminIndex");
        }

        // ==================== HELPERS ====================

        private Cart GetOrCreateCart(bool createIfMissing = true)
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                int? userId = AppHelpers.GetCurrentUserId(User);
                if (userId.HasValue)
                {
                    int uid = userId.Value;
                    var userCart = _context.Carts
                        .Include("CartItems.Product.Images")
                        .FirstOrDefault(c => c.UserId == uid);

                    if (userCart == null && createIfMissing)
                    {
                        userCart = new Cart { UserId = uid, CreatedAt = DateTime.Now };
                        _context.Carts.Add(userCart);
                        _context.SaveChanges();
                    }

                    return userCart ?? new Cart();
                }
            }

            string sessionId = Session["CartSessionId"] as string;
            if (string.IsNullOrEmpty(sessionId))
            {
                if (!createIfMissing) return new Cart();
                sessionId = Guid.NewGuid().ToString("N");
                Session["CartSessionId"] = sessionId;
            }

            var guestCart = _context.Carts
                .Include("CartItems.Product.Images")
                .FirstOrDefault(c => c.SessionId == sessionId && c.UserId == null);

            if (guestCart == null && createIfMissing)
            {
                guestCart = new Cart { SessionId = sessionId, CreatedAt = DateTime.Now };
                _context.Carts.Add(guestCart);
                _context.SaveChanges();
            }

            return guestCart ?? new Cart();
        }

        private decimal CalculateCouponDiscount(string code, decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(code) || subTotal <= 0)
                return 0;

            string upper = code.ToUpperInvariant();
            DateTime now = DateTime.Now;
            var coupon = _context.Coupons.FirstOrDefault(c =>
                c.Code.ToUpper() == upper &&
                c.IsActive &&
                (!c.ExpiryDate.HasValue || c.ExpiryDate >= now));

            if (coupon == null || subTotal < coupon.MinOrderAmount)
                return 0;

            decimal raw = Math.Round(subTotal * coupon.DiscountPercent / 100m, 0);
            return Math.Min(raw, coupon.MaxDiscountAmount);
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
```

### File: FlowerShop/Controllers\PaymentController.cs
```csharp
using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    public class PaymentController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();
        private readonly VnPayService _vnPayService = new VnPayService();

        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            int uid = userId.Value;
            var user = _context.Users.Find(uid);
            var cart = _context.Carts
                .Include("CartItems.Product.Images")
                .FirstOrDefault(c => c.UserId == uid);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống. Vui lòng chọn mẫu hoa yêu thích trước khi thanh toán!";
                return RedirectToAction("Index", "Product");
            }

            foreach (var ci in cart.CartItems)
            {
                if (ci.Product != null)
                {
                    ci.UnitPrice = ci.Product.Price;
                }
            }
            _context.SaveChanges();

            decimal subTotal = cart.CartItems.Sum(i => i.UnitPrice * i.Quantity);
            string couponCode = Session["AppliedCoupon"] as string;
            decimal discount = CalculateDiscount(couponCode, subTotal);

            var vm = new CheckoutViewModel
            {
                RecipientName = user != null ? user.FullName : string.Empty,
                RecipientPhone = user != null && user.Phone != null ? user.Phone : string.Empty,
                DeliveryAddress = user != null ? user.Address : string.Empty,
                DeliveryDate = DateTime.Now.AddHours(3),
                ShippingCarrier = "Giao Hoa Hỏa Tốc 2H (CHAM Express)",
                PaymentMethod = "COD",
                CouponCode = discount > 0 ? couponCode : null,
                Items = cart.CartItems.ToList(),
                SubTotal = subTotal,
                ShippingFee = 30000,
                DiscountAmount = discount
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessPayment(CheckoutViewModel model)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            int uid = userId.Value;
            var cart = _context.Carts
                .Include("CartItems.Product.Images")
                .FirstOrDefault(c => c.UserId == uid);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, không thể thực hiện thanh toán.";
                return RedirectToAction("Cart", "Order");
            }

            decimal shippingFee;
            switch (model.ShippingCarrier)
            {
                case "Giao Tiêu Chuẩn Trong Ngày":
                    shippingFee = 20000;
                    break;
                case "Nhận Trực Tiếp Tại Cửa Hàng CHAM Flowers":
                case "Nhận Trực Tiếp Tại Cửa Hàng NT Flowers":
                    shippingFee = 0;
                    break;
                default:
                    shippingFee = 30000;
                    break;
            }

            decimal subTotal = cart.CartItems.Sum(i => (i.Product != null ? i.Product.Price : i.UnitPrice) * i.Quantity);
            string couponCode = !string.IsNullOrWhiteSpace(model.CouponCode) ? model.CouponCode : (Session["AppliedCoupon"] as string);
            decimal discount = CalculateDiscount(couponCode, subTotal);

            if (!ModelState.IsValid)
            {
                model.Items = cart.CartItems.ToList();
                model.SubTotal = subTotal;
                model.ShippingFee = shippingFee;
                model.DiscountAmount = discount;
                return View("Checkout", model);
            }

            foreach (var item in cart.CartItems)
            {
                if (item.Product == null || !item.Product.IsActive || item.Quantity > item.Product.StockQuantity)
                {
                    ModelState.AddModelError("", string.Format("Sản phẩm \"{0}\" hiện chỉ còn {1} sản phẩm trong kho.",
                        item.Product != null ? item.Product.Name : "",
                        item.Product != null ? item.Product.StockQuantity : 0));
                    model.Items = cart.CartItems.ToList();
                    model.SubTotal = subTotal;
                    model.ShippingFee = shippingFee;
                    model.DiscountAmount = discount;
                    return View("Checkout", model);
                }
            }

            decimal grandTotal = Math.Max(0, subTotal + shippingFee - discount);

            var order = new Order
            {
                UserId = uid,
                RecipientName = model.RecipientName.Trim(),
                RecipientPhone = model.RecipientPhone.Trim(),
                DeliveryAddress = model.DeliveryAddress.Trim(),
                DeliveryDate = model.DeliveryDate,
                ShippingCarrier = model.ShippingCarrier,
                ShippingFee = shippingFee,
                CouponCode = discount > 0 ? couponCode : null,
                DiscountAmount = discount,
                PaymentMethod = model.PaymentMethod,
                Total = grandTotal,
                Status = "Chờ xác nhận",
                Note = model.Note != null ? model.Note.Trim() : null,
                IsPaid = false,
                CreatedAt = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var ci in cart.CartItems.ToList())
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product != null ? ci.Product.Price : ci.UnitPrice
                });
            }

            _context.CartItems.RemoveRange(cart.CartItems.ToList());
            Session.Remove("AppliedCoupon");
            _context.SaveChanges();

            if (string.Equals(model.PaymentMethod, "VNPay", StringComparison.OrdinalIgnoreCase))
            {
                string vnPayUrl = _vnPayService.CreatePaymentUrl(order, Request);
                return Redirect(vnPayUrl);
            }

            TempData["Success"] = string.Format("Đặt hàng thành công! Mã đơn hàng của bạn là #{0}.", order.Id);
            return RedirectToAction("PaymentSuccess", new { id = order.Id });
        }

        [HttpGet]
        public ActionResult VnPayGateway()
        {
            if (!_vnPayService.ValidateSignature(Request.QueryString))
            {
                return Content("Chữ ký bảo mật VNPay (HMAC-SHA512) không hợp lệ!");
            }

            int orderId;
            int.TryParse(Request.QueryString["vnp_TxnRef"], out orderId);
            var order = _context.Orders.Find(orderId);
            ViewBag.Order = order;
            ViewBag.QueryString = Request.Url != null ? Request.Url.Query : ("?" + Request.QueryString.ToString());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VnPaySimulateCallback(string rawQuery, bool isSuccess)
        {
            NameValueCollection parsed = HttpUtility.ParseQueryString(rawQuery ?? string.Empty);
            string callbackUrl = _vnPayService.CreateSimulatedCallbackUrl(parsed, isSuccess, Request);
            return Redirect(callbackUrl);
        }

        [HttpGet]
        public ActionResult VnPayReturn()
        {
            bool validSig = _vnPayService.ValidateSignature(Request.QueryString);
            int orderId;
            int.TryParse(Request.QueryString["vnp_TxnRef"], out orderId);
            string responseCode = Request.QueryString["vnp_ResponseCode"] ?? string.Empty;
            string transactionNo = Request.QueryString["vnp_TransactionNo"] ?? string.Empty;

            var order = _context.Orders.Find(orderId);
            if (order == null)
                return HttpNotFound();

            if (validSig && responseCode == "00")
            {
                order.IsPaid = true;
                order.PaidAmount = order.Total;
                order.PaidDate = DateTime.Now;
                order.VnPayTransactionNo = transactionNo;
                if (order.Status == "Chờ xác nhận")
                {
                    order.Status = "Đang xử lý";
                }
                _context.SaveChanges();

                TempData["Success"] = string.Format("Thanh toán VNPay thành công! Mã giao dịch: {0}", transactionNo);
                return RedirectToAction("PaymentSuccess", new { id = order.Id });
            }

            TempData["Error"] = "Giao dịch thanh toán VNPay không thành công hoặc đã bị hủy bởi người dùng.";
            return RedirectToAction("PaymentFailed", new { id = order.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RetryPayment(int orderId, string paymentMethod)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId && o.UserId == userId);
            if (order == null)
                return HttpNotFound();

            order.PaymentMethod = paymentMethod;
            _context.SaveChanges();

            if (paymentMethod == "VNPay")
            {
                return Redirect(_vnPayService.CreatePaymentUrl(order, Request));
            }

            TempData["Success"] = string.Format("Đã chuyển đơn hàng #{0} sang thanh toán khi nhận hàng (COD)!", order.Id);
            return RedirectToAction("PaymentSuccess", new { id = order.Id });
        }

        [Authorize]
        public ActionResult PaymentSuccess(int id)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            bool isAdmin = User.IsInRole("Admin");

            var order = _context.Orders
                .Include("OrderItems.Product.Images")
                .FirstOrDefault(o => o.Id == id && (isAdmin || o.UserId == userId));

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        [Authorize]
        public ActionResult PaymentFailed(int id)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            var order = _context.Orders.FirstOrDefault(o => o.Id == id && o.UserId == userId);
            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        private decimal CalculateDiscount(string code, decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(code) || subTotal <= 0)
                return 0;

            string upper = code.ToUpperInvariant();
            DateTime now = DateTime.Now;
            var coupon = _context.Coupons.FirstOrDefault(c =>
                c.Code.ToUpper() == upper &&
                c.IsActive &&
                (!c.ExpiryDate.HasValue || c.ExpiryDate >= now));

            if (coupon == null || subTotal < coupon.MinOrderAmount)
                return 0;

            decimal raw = Math.Round(subTotal * coupon.DiscountPercent / 100m, 0);
            return Math.Min(raw, coupon.MaxDiscountAmount);
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
```

### File: FlowerShop/Controllers\AccountController.cs
```csharp
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
                return RedirectToAction("Index", "Report");
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

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin đổi mật khẩu chưa hợp lệ (mật khẩu mới tối thiểu 6 ký tự và phải khớp xác nhận).";
                return RedirectToAction("Profile");
            }

            if (!AppHelpers.VerifyPassword(model.CurrentPassword, user.Password))
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác.";
                return RedirectToAction("Profile");
            }

            user.Password = AppHelpers.HashPassword(model.NewPassword);
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Profile");
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
```

### File: FlowerShop/Controllers\CustomerController.cs
```csharp
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index(string search, string filter, int page = 1)
        {
            var query = _context.Users.AsQueryable();

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
                case "locked":
                    query = query.Where(u => !u.IsActive);
                    break;
                case "admin":
                    query = query.Where(u => u.Role == "Admin");
                    break;
                case "customer":
                    query = query.Where(u => u.Role == "Customer");
                    break;
            }

            ViewBag.Search = search;
            ViewBag.Filter = filter;

            var paged = query.OrderByDescending(u => u.CreatedAt).ToPagedList(page, 10);
            return View(paged);
        }

        public ActionResult Details(int id)
        {
            var user = _context.Users
                .Include("Orders.OrderItems")
                .Include("Reviews.Product")
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return HttpNotFound();

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
            var user = _context.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string fullName, string phone, string address, string role, bool isVip = false, bool isActive = true, string newPassword = null)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError("", "Họ tên, số điện thoại và địa chỉ không được để trống.");
                return View(user);
            }

            user.FullName = fullName.Trim();
            user.Full_name = fullName.Trim();
            user.Phone = phone.Trim();
            user.Address = address.Trim();
            user.Role = role == "Admin" ? "Admin" : "Customer";
            user.IsVip = isVip;
            user.IsActive = isActive;
            user.UpdatedAt = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length >= 6)
            {
                user.Password = AppHelpers.HashPassword(newPassword);
            }

            _context.SaveChanges();
            TempData["Success"] = string.Format("Cập nhật tài khoản \"{0}\" thành công!", user.FullName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleLock(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return RedirectToAction("Index");

            if (string.Equals(user.Email, "admin@chamflowers.vn", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(user.Email, "admin@ntflowers.vn", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Không thể khóa tài khoản Quản trị viên gốc của cửa hàng.";
                return RedirectToAction("Index");
            }

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = user.IsActive
                ? string.Format("Đã mở khóa tài khoản \"{0}\".", user.FullName)
                : string.Format("Đã khóa tài khoản \"{0}\".", user.FullName);

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
```

### File: FlowerShop/Controllers\BannerController.cs
```csharp
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;

namespace FlowerShop.Controllers
{
    [Authorize(Roles = "Admin")]
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
```

### File: FlowerShop/Controllers\HotDealController.cs
```csharp
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
            var popups = _context.HotDealPopups
                .Include(p => p.Product)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            var active = popups.FirstOrDefault(p => p.IsActiveNow);
            if (active == null)
            {
                return Json(new { hasPopup = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                hasPopup = true,
                id = active.Id,
                title = active.Title ?? "Ưu đãi Hot Deal Hoa Tươi!",
                subtitle = active.Subtitle,
                imageUrl = active.ImageUrl,
                couponCode = active.CouponCode,
                discountPercent = active.DiscountPercent,
                productUrl = active.ProductId.HasValue ? string.Format("/Product/Details/{0}", active.ProductId.Value) : "/HotDeal",
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

        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult CreateHotDealPopup()
        {
            ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name");
            return View(new HotDealPopup
            {
                IsActive = true,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10),
                DiscountPercent = 20,
                CouponCode = "CHAMFLOWERS15",
                ImageUrl = "/images/samflora/b12-main.jpg"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateHotDealPopup(HotDealPopup model, HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (!AppHelpers.IsImageValid(imageFile))
                {
                    ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif");
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

            model.CreatedAt = DateTime.Now;
            _context.HotDealPopups.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đã thêm chương trình Hot Deal Popup thành công!";
            return RedirectToAction("Manage");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EditHotDealPopup(int id)
        {
            var popup = _context.HotDealPopups.Find(id);
            if (popup == null)
                return HttpNotFound();

            ViewBag.Products = new SelectList(_context.Products.Where(p => p.IsActive).ToList(), "Id", "Name", popup.ProductId);
            return View(popup);
        }

        [Authorize(Roles = "Admin")]
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
                    ModelState.AddModelError("", "Chỉ cho phép file ảnh jpg, png, jpeg, gif");
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
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.IsActive = model.IsActive;

            _context.SaveChanges();
            TempData["Success"] = "Đã cập nhật Hot Deal Popup thành công!";
            return RedirectToAction("Manage");
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
```

### File: FlowerShop/Controllers\MenuController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    [Authorize(Roles = "Admin")]
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
```

### File: FlowerShop/Controllers\ReviewController.cs
```csharp
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Models;
using FlowerShop.Services;
using PagedList;

namespace FlowerShop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int productId, int rating, string content)
        {
            int? userId = AppHelpers.GetCurrentUserId(User);
            if (!userId.HasValue)
                return RedirectToAction("Login", "Account");

            int uid = userId.Value;

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Vui lòng nhập nội dung đánh giá sản phẩm.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            bool hasCompletedOrder = _context.Orders
                .Include(o => o.OrderItems)
                .Any(o => o.UserId == uid &&
                          o.Status == "Hoàn thành" &&
                          o.OrderItems.Any(oi => oi.ProductId == productId));

            if (!hasCompletedOrder && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "Bạn chỉ có thể đánh giá sản phẩm sau khi đơn hàng đã hoàn thành giao nhận.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            if (rating < 1) rating = 1;
            if (rating > 5) rating = 5;

            var review = new Review
            {
                ProductId = productId,
                UserId = uid,
                Rating = rating,
                Content = content.Trim(),
                Status = ReviewStatus.Pending,
                IsApproved = false,
                CreatedAt = DateTime.Now
            };

            var prodReview = new ProductReview
            {
                ProductId = productId,
                UserId = uid,
                Rating = rating,
                Comment = content.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.ProductReviews.Add(prodReview);
            _context.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã đánh giá! Phản hồi của bạn đã được ghi nhận và đang chờ Quản trị viên phê duyệt.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        // ==================== QUẢN LÝ ĐÁNH GIÁ PHÍA ADMIN ====================

        [Authorize(Roles = "Admin")]
        public ActionResult Index(string filter, int page = 1)
        {
            var query = _context.Reviews
                .Include("Product.Images")
                .Include(r => r.User)
                .AsQueryable();

            switch (filter)
            {
                case "pending":
                    query = query.Where(r => !r.IsApproved && r.Status == ReviewStatus.Pending);
                    break;
                case "approved":
                    query = query.Where(r => r.IsApproved);
                    break;
                case "hidden":
                    query = query.Where(r => r.Status == ReviewStatus.Hidden);
                    break;
            }

            ViewBag.Filter = filter;
            var paged = query.OrderByDescending(r => r.CreatedAt).ToPagedList(page, 10);
            return View(paged);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            var review = _context.Reviews
                .Include("Product.Images")
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
                return HttpNotFound();

            return View(review);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review == null)
            {
                TempData["Error"] = "Không tìm thấy đánh giá cần phê duyệt.";
                return RedirectToAction("Index");
            }

            review.IsApproved = true;
            review.Status = ReviewStatus.Approved;
            _context.SaveChanges();

            TempData["Success"] = string.Format("Đã phê duyệt đánh giá #{0} hiển thị công khai trên website!", id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Hide(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                review.IsApproved = false;
                review.Status = ReviewStatus.Hidden;
                _context.SaveChanges();
                TempData["Success"] = string.Format("Đã ẩn đánh giá #{0} khỏi trang sản phẩm.", id);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa đánh giá.";
            }
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
```

### File: FlowerShop/Controllers\ChatController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using FlowerShop.Hubs;
using FlowerShop.Models;
using FlowerShop.Services;
using Microsoft.AspNet.SignalR;

namespace FlowerShop.Controllers
{
    public class ChatController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        [HttpGet]
        public ActionResult InitSession()
        {
            string sessionKey = Session["ChatSessionKey"] as string;
            int? userId = null;
            string customerName = "Khách truy cập";

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = AppHelpers.GetCurrentUserId(User);
                customerName = !string.IsNullOrWhiteSpace(User.Identity.Name) ? User.Identity.Name : "Khách hàng";
                if (userId.HasValue)
                {
                    sessionKey = string.Format("user_{0}", userId.Value);
                    Session["ChatSessionKey"] = sessionKey;
                }
            }

            if (string.IsNullOrEmpty(sessionKey))
            {
                sessionKey = "guest_" + Guid.NewGuid().ToString("N").Substring(0, 10);
                Session["ChatSessionKey"] = sessionKey;
            }

            var conv = _context.ChatConversations
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.SessionKey == sessionKey);

            object messagesObj = new object[0];
            if (conv != null && conv.Messages != null)
            {
                messagesObj = conv.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new
                    {
                        id = m.Id,
                        senderRole = m.SenderRole,
                        senderName = m.SenderName,
                        content = m.Content,
                        messageType = m.MessageType,
                        attachmentUrl = m.AttachmentUrl,
                        createdAt = m.CreatedAt.ToString("HH:mm dd/MM")
                    })
                    .ToList();
            }

            return Json(new
            {
                sessionKey = sessionKey,
                customerName = customerName,
                userId = userId,
                conversationId = conv != null ? conv.Id : 0,
                messages = messagesObj
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendCustomerMessage(string sessionKey, string customerName, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Nội dung tin nhắn không được để trống." });
            }

            int? userId = null;
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                userId = AppHelpers.GetCurrentUserId(User);
                if (!string.IsNullOrWhiteSpace(User.Identity.Name))
                    customerName = User.Identity.Name;
                if (userId.HasValue)
                    sessionKey = string.Format("user_{0}", userId.Value);
            }

            if (string.IsNullOrWhiteSpace(sessionKey))
            {
                sessionKey = (Session["ChatSessionKey"] as string) ?? ("guest_" + Guid.NewGuid().ToString("N").Substring(0, 10));
            }
            Session["ChatSessionKey"] = sessionKey;

            var conv = _context.ChatConversations.FirstOrDefault(c => c.SessionKey == sessionKey);
            if (conv == null)
            {
                conv = new ChatConversation
                {
                    SessionKey = sessionKey,
                    UserId = userId,
                    CustomerName = string.IsNullOrWhiteSpace(customerName) ? "Khách truy cập" : customerName.Trim(),
                    Status = "Đang xử lý",
                    IsCustomerOnline = true,
                    UnreadAdminCount = 1,
                    CreatedAt = DateTime.Now,
                    LastMessageAt = DateTime.Now
                };
                _context.ChatConversations.Add(conv);
                _context.SaveChanges();
            }
            else
            {
                conv.IsCustomerOnline = true;
                conv.LastMessageAt = DateTime.Now;
                conv.UnreadAdminCount += 1;
                if (conv.Status == "Đã giải quyết")
                    conv.Status = "Đang xử lý";
            }

            var msg = new ChatMessage
            {
                ConversationId = conv.Id,
                SenderRole = "Customer",
                SenderName = conv.CustomerName,
                Content = content.Trim(),
                MessageType = "Text",
                CreatedAt = DateTime.Now
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            var payload = new
            {
                id = msg.Id,
                conversationId = conv.Id,
                sessionKey = conv.SessionKey,
                customerName = conv.CustomerName,
                senderRole = msg.SenderRole,
                senderName = msg.SenderName,
                content = msg.Content,
                messageType = msg.MessageType,
                attachmentUrl = msg.AttachmentUrl,
                status = conv.Status,
                unreadAdminCount = conv.UnreadAdminCount,
                createdAt = msg.CreatedAt.ToString("HH:mm dd/MM")
            };

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("conv_" + conv.SessionKey).receiveMessage(payload);
            hubContext.Clients.Group("Admins").receiveMessage(payload);

            return Json(new { success = true, message = payload });
        }

        // ==================== TRUNG TÂM CHĂM SÓC KHÁCH HÀNG ADMIN ====================

        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult Index(int? id, string statusFilter)
        {
            var query = _context.ChatConversations
                .Include(c => c.Messages)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(c => c.Status == statusFilter);
            }

            var conversations = query
                .OrderByDescending(c => c.LastMessageAt)
                .ToList();

            ChatConversation activeConv = null;
            if (conversations.Any())
            {
                activeConv = id.HasValue
                    ? (conversations.FirstOrDefault(c => c.Id == id.Value) ?? conversations.First())
                    : conversations.First();

                if (activeConv.UnreadAdminCount > 0)
                {
                    activeConv.UnreadAdminCount = 0;
                    _context.SaveChanges();
                }
            }

            ViewBag.Conversations = conversations;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.QuickProducts = _context.Products.Include(p => p.Images).Where(p => p.IsActive).Take(8).ToList();
            ViewBag.QuickCoupons = _context.Coupons.Where(c => c.IsActive).ToList();

            return View(activeConv);
        }

        [System.Web.Mvc.Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AdminReply(int conversationId, string content, string messageType = "Text", string attachmentUrl = null)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Vui lòng nhập nội dung phản hồi." });
            }

            var conv = _context.ChatConversations.Find(conversationId);
            if (conv == null)
                return Json(new { success = false, message = "Không tìm thấy hội thoại." });

            conv.LastMessageAt = DateTime.Now;
            conv.UnreadAdminCount = 0;
            conv.UnreadCustomerCount += 1;

            var msg = new ChatMessage
            {
                ConversationId = conv.Id,
                SenderRole = "Admin",
                SenderName = "CHAM Flowers CSKH",
                Content = content.Trim(),
                MessageType = messageType,
                AttachmentUrl = attachmentUrl,
                CreatedAt = DateTime.Now
            };

            _context.ChatMessages.Add(msg);
            _context.SaveChanges();

            var payload = new
            {
                id = msg.Id,
                conversationId = conv.Id,
                sessionKey = conv.SessionKey,
                customerName = conv.CustomerName,
                senderRole = msg.SenderRole,
                senderName = msg.SenderName,
                content = msg.Content,
                messageType = msg.MessageType,
                attachmentUrl = msg.AttachmentUrl,
                status = conv.Status,
                unreadAdminCount = 0,
                createdAt = msg.CreatedAt.ToString("HH:mm dd/MM")
            };

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.Group("conv_" + conv.SessionKey).receiveMessage(payload);
            hubContext.Clients.Group("Admins").receiveMessage(payload);

            return Json(new { success = true, message = payload });
        }

        [System.Web.Mvc.Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int conversationId, string status)
        {
            var conv = _context.ChatConversations.Find(conversationId);
            if (conv != null)
            {
                conv.Status = status;
                _context.SaveChanges();
                TempData["Success"] = string.Format("Đã chuyển trạng thái hội thoại sang \"{0}\".", status);
            }
            return RedirectToAction("Index", new { id = conversationId });
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
```

### File: FlowerShop/Controllers\ReportController.cs
```csharp
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using FlowerShop.Models;

namespace FlowerShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly FlowerShopDbContext _context = new FlowerShopDbContext();

        public ActionResult Index(DateTime? fromDate, DateTime? toDate, string preset = null)
        {
            DateTime end = (toDate ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);
            DateTime start = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            string label = "Tùy chọn thời gian";

            if (preset == "7d")
            {
                start = DateTime.Today.AddDays(-6);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "7 ngày gần nhất";
            }
            else if (preset == "30d" || (!fromDate.HasValue && !toDate.HasValue))
            {
                start = DateTime.Today.AddDays(-29);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "30 ngày gần nhất";
            }
            else if (preset == "90d")
            {
                start = DateTime.Today.AddDays(-89);
                end = DateTime.Today.AddDays(1).AddTicks(-1);
                label = "Quý này (90 ngày)";
            }

            var ordersInPeriod = _context.Orders
                .Include(o => o.User)
                .Include("OrderItems.Product.Category")
                .Include("OrderItems.Product.Images")
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            var revenueOrders = ordersInPeriod
                .Where(o => o.Status == "Hoàn thành" || o.IsPaid)
                .ToList();

            decimal totalRevenue = revenueOrders.Sum(o => o.Total);
            decimal grossProfit = Math.Round(totalRevenue * 0.42m, 0);

            var series = new List<RevenuePoint>();
            int totalDays = Math.Max(1, (end.Date - start.Date).Days + 1);
            if (totalDays <= 14)
            {
                for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
                {
                    var dayOrders = revenueOrders.Where(o => o.CreatedAt.Date == d).ToList();
                    decimal rev = dayOrders.Sum(o => o.Total);
                    series.Add(new RevenuePoint
                    {
                        Label = d.ToString("dd/MM"),
                        Revenue = rev,
                        GrossProfit = Math.Round(rev * 0.42m, 0),
                        OrderCount = dayOrders.Count
                    });
                }
            }
            else
            {
                DateTime cursor = start.Date;
                int weekIdx = 1;
                while (cursor <= end.Date)
                {
                    DateTime weekEnd = cursor.AddDays(6) > end.Date ? end.Date : cursor.AddDays(6);
                    var wkOrders = revenueOrders.Where(o => o.CreatedAt.Date >= cursor && o.CreatedAt.Date <= weekEnd).ToList();
                    decimal rev = wkOrders.Sum(o => o.Total);
                    series.Add(new RevenuePoint
                    {
                        Label = string.Format("Tuần {0} ({1:dd/MM}-{2:dd/MM})", weekIdx++, cursor, weekEnd),
                        Revenue = rev,
                        GrossProfit = Math.Round(rev * 0.42m, 0),
                        OrderCount = wkOrders.Count
                    });
                    cursor = weekEnd.AddDays(1);
                }
            }

            var topProducts = ordersInPeriod
                .Where(o => o.Status != "Đã hủy")
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.Product != null)
                .GroupBy(oi => oi.ProductId)
                .Select(g =>
                {
                    var firstProd = g.First().Product;
                    return new TopProductStat
                    {
                        ProductId = firstProd.Id,
                        ProductName = firstProd.Name,
                        CategoryName = firstProd.Category != null ? firstProd.Category.Name : "Hoa tươi",
                        ImageUrl = firstProd.MainImageUrl,
                        SoldQuantity = g.Sum(x => x.Quantity),
                        TotalRevenue = g.Sum(x => x.UnitPrice * x.Quantity),
                        StockQuantity = firstProd.StockQuantity
                    };
                })
                .OrderByDescending(x => x.SoldQuantity)
                .ThenByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToList();

            var allCustomers = _context.Users.Where(u => u.Role == "Customer").ToList();
            var allProducts = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .ToList();

            var lowStock = allProducts.Where(p => p.IsActive && p.StockQuantity <= 5).OrderBy(p => p.StockQuantity).ToList();
            var expiring = allProducts
                .Where(p => p.IsActive && p.ExpiryDate.HasValue && p.ExpiryDate.Value <= DateTime.Now.AddDays(4))
                .OrderBy(p => p.ExpiryDate)
                .ToList();

            var vm = new ReportDashboardViewModel
            {
                FromDate = start,
                ToDate = end.Date,
                PeriodLabel = label,
                TotalRevenue = totalRevenue,
                GrossProfit = grossProfit,
                TotalOrders = ordersInPeriod.Count,
                PendingOrders = ordersInPeriod.Count(o => o.Status == "Chờ xác nhận"),
                ProcessingOrders = ordersInPeriod.Count(o => o.Status == "Đang xử lý"),
                ShippingOrders = ordersInPeriod.Count(o => o.Status == "Đang giao"),
                CompletedOrders = ordersInPeriod.Count(o => o.Status == "Hoàn thành"),
                CancelledOrders = ordersInPeriod.Count(o => o.Status == "Đã hủy"),
                TotalCustomers = allCustomers.Count,
                VipCustomers = allCustomers.Count(u => u.IsVip),
                NewCustomersInPeriod = allCustomers.Count(u => u.CreatedAt >= start && u.CreatedAt <= end),
                TotalProducts = allProducts.Count,
                LowStockCount = lowStock.Count,
                ExpiringSoonCount = expiring.Count,
                RevenueSeries = series,
                BestSellingProducts = topProducts,
                TopVipCustomers = allCustomers.OrderByDescending(u => u.TotalSpent).Take(5).ToList(),
                LowStockProducts = lowStock,
                ExpiringProducts = expiring,
                RecentOrders = ordersInPeriod.Take(8).ToList()
            };

            if (!ordersInPeriod.Any())
            {
                ViewBag.NoDataMessage = "Không có dữ liệu để thống kê trong khoảng thời gian đã chọn.";
            }

            return View(vm);
        }

        public ActionResult ExportCsv(DateTime? fromDate, DateTime? toDate)
        {
            DateTime start = (fromDate ?? DateTime.Today.AddDays(-29)).Date;
            DateTime end = (toDate ?? DateTime.Today).Date.AddDays(1).AddTicks(-1);

            var orders = _context.Orders
                .Include("OrderItems.Product")
                .Where(o => o.CreatedAt >= start && o.CreatedAt <= end)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("BÁO CÁO KINH DOANH CỬA HÀNG HOA TƯƠI CHAM FLOWERS");
            sb.AppendLine(string.Format("Từ ngày:,{0:dd/MM/yyyy},Đến ngày:,{1:dd/MM/yyyy}", start, end));
            sb.AppendLine();
            sb.AppendLine("Mã Đơn,Ngày Đặt,Khách Nhận,Số Điện Thoại,Phương Thức,Đã Thanh Toán,Trạng Thái,Tổng Tiền (VNĐ),Chi Tiết Sản Phẩm");

            foreach (var o in orders)
            {
                string itemsDesc = string.Join(" | ", o.OrderItems.Select(i => string.Format("{0} x{1}", i.Product != null ? i.Product.Name : "", i.Quantity)));
                sb.AppendLine(string.Format("#{0},{1:dd/MM/yyyy HH:mm},\"{2}\",\"{3}\",{4},{5},\"{6}\",{7},\"{8}\"",
                    o.Id,
                    o.CreatedAt,
                    o.RecipientName,
                    o.RecipientPhone,
                    o.PaymentMethod,
                    o.IsPaid ? "Đã TT" : "Chưa TT",
                    o.Status,
                    o.Total,
                    itemsDesc));
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            byte[] body = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] finalBytes = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, finalBytes, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, finalBytes, preamble.Length, body.Length);

            string fileName = string.Format("BaoCao_CHAMFlowers_{0:yyyyMMdd}_{1:yyyyMMdd}.csv", start, end);
            return File(finalBytes, "text/csv; charset=utf-8", fileName);
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
```

### File: FlowerShop/Views\Shared\_LayoutUser.cshtml
```html
@{
    var currentCtrl = ViewContext.RouteData.Values["controller"] != null ? ViewContext.RouteData.Values["controller"].ToString() : "";
    var currentAct = ViewContext.RouteData.Values["action"] != null ? ViewContext.RouteData.Values["action"].ToString() : "";
}
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - CHAM Flowers | Tiệm Hoa Tươi Nghệ Thuật</title>
    <link rel="icon" type="image/svg+xml" href="~/images/logo-cham-icon.svg" />
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous">
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,600;0,700;1,600&family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap&subset=vietnamese" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="~/Content/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Content/site.css?v=20261104" />
</head>
<body>
    <!-- Top Promo Bar -->
    <div class="nt-topbar">
        <div class="container d-flex flex-wrap justify-content-between align-items-center">
            <div>
                <i class="bi bi-flower1 me-1"></i>
                <span>Giao hoa tươi hỏa tốc 2H nội thành TP.HCM • Nhập mã <strong>CHAMFLOWERS15</strong> giảm ngay 15%</span>
            </div>
            <div class="d-none d-md-flex align-items-center gap-3">
                <a href="javascript:void(0)" onclick="window['checkHotDealPopup'] &amp;&amp; window['checkHotDealPopup'](true)" class="text-white text-decoration-none fw-semibold">
                    <i class="bi bi-lightning-charge-fill text-warning"></i> Ưu đãi hôm nay
                </a>
                <span>|</span>
                <span><i class="bi bi-telephone-fill me-1"></i> Hotline: 1900 6868</span>
            </div>
        </div>
    </div>

    <!-- Main Navbar -->
    <header class="sticky-top nt-navbar">
        <nav class="navbar navbar-expand-xl py-2">
            <div class="container-xxl px-lg-4">
                <a class="navbar-brand nt-brand d-flex align-items-center py-0 me-2 flex-shrink-0" href="@Url.Action("Index", "Home")">
                    <img src="~/images/logo-cham.svg" alt="CHAM Flowers - Flowers For You" class="cham-logo-nav" />
                </a>

                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#ntMainNav">
                    <span class="navbar-toggler-icon"></span>
                </button>

                <div class="collapse navbar-collapse" id="ntMainNav">
                    @Html.Action("NavMenu", "Menu")

                    <div class="d-flex flex-wrap flex-xl-nowrap align-items-center gap-2 mt-2 mt-xl-0">
                        <form action="@Url.Action("Index", "Product")" method="get" class="nt-search-form flex-grow-1 flex-xl-grow-0">
                            <div class="nt-search-box">
                                <input type="text"
                                       name="search"
                                       value="@Request.QueryString["search"]"
                                       class="nt-search-input"
                                       placeholder="Tìm mẫu hoa tươi..."
                                       autocomplete="off" />
                                <button class="nt-search-btn" type="submit" title="Tìm kiếm hoa">
                                    <i class="bi bi-search"></i>
                                </button>
                            </div>
                        </form>

                        <a href="@Url.Action("Cart", "Order")" class="nt-cart-btn position-relative flex-shrink-0" title="Giỏ hàng">
                            <i class="bi bi-bag-heart-fill"></i>
                            <span id="cartCountBadge" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger d-none">0</span>
                        </a>

                        @if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                        {
                            if (User.IsInRole("Admin"))
                            {
                                <a href="@Url.Action("Index", "Report")" class="btn btn-sm btn-dark rounded-pill px-3 fw-semibold nt-nav-action-btn flex-shrink-0">
                                    <i class="bi bi-speedometer2 me-1"></i> Quản trị
                                </a>
                            }
                            <div class="dropdown flex-shrink-0">
                                <button class="btn btn-nt-outline btn-sm dropdown-toggle d-flex align-items-center gap-1 nt-nav-action-btn" type="button" data-bs-toggle="dropdown">
                                    <i class="bi bi-person-circle"></i>
                                    <span class="text-truncate" style="max-width:125px">@User.Identity.Name</span>
                                    @if (AppHelpers.GetCurrentIsVip(User))
                                    {
                                        <span class="badge bg-warning text-dark ms-1">VIP</span>
                                    }
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end shadow border-0 rounded-4 p-2">
                                    <li><a class="dropdown-item rounded-3 py-2" href="@Url.Action("Profile", "Account")"><i class="bi bi-person-gear me-2 text-danger"></i>Tài khoản cá nhân</a></li>
                                    <li><a class="dropdown-item rounded-3 py-2" href="@Url.Action("MyOrders", "Order")"><i class="bi bi-receipt-cutoff me-2 text-danger"></i>Đơn hàng của tôi</a></li>
                                    @if (User.IsInRole("Admin"))
                                    {
                                        <li><hr class="dropdown-divider" /></li>
                                        <li><a class="dropdown-item rounded-3 py-2 fw-bold text-primary" href="@Url.Action("Index", "Report")"><i class="bi bi-shield-lock me-2"></i>Hệ thống Quản trị</a></li>
                                    }
                                    <li><hr class="dropdown-divider" /></li>
                                    <li><a class="dropdown-item rounded-3 py-2 text-danger" href="@Url.Action("Logout", "Account")"><i class="bi bi-box-arrow-right me-2"></i>Đăng xuất</a></li>
                                </ul>
                            </div>
                        }
                        else
                        {
                            <a href="@Url.Action("Login", "Account")" class="btn btn-nt-outline btn-sm nt-nav-action-btn flex-shrink-0">Đăng nhập</a>
                            <a href="@Url.Action("Register", "Account")" class="btn btn-nt-primary btn-sm nt-nav-action-btn flex-shrink-0">Đăng ký</a>
                        }
                    </div>
                </div>
            </div>
        </nav>
    </header>

    <!-- Floating AJAX Toast Container -->
    <div id="ntToastContainer"></div>

    <!-- Server TempData Alerts -->
    <div class="container mt-3">
        @if (TempData["Success"] != null)
        {
            <div class="alert alert-success alert-dismissible fade show rounded-4 shadow-sm border-0" role="alert">
                <i class="bi bi-check-circle-fill me-2"></i> @TempData["Success"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }
        @if (TempData["Error"] != null)
        {
            <div class="alert alert-danger alert-dismissible fade show rounded-4 shadow-sm border-0" role="alert">
                <i class="bi bi-exclamation-octagon-fill me-2"></i> @TempData["Error"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }
    </div>

    <main role="main" class="pb-5">
        @RenderBody()
    </main>

    <!-- HotDealPopup Modal -->
    <div class="modal fade" id="hotDealPopupModal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 rounded-4 overflow-hidden shadow-lg">
                <div class="position-relative">
                    <button type="button"
                            class="btn position-absolute top-0 end-0 m-3 rounded-circle d-inline-flex align-items-center justify-content-center shadow"
                            style="width:36px;height:36px;z-index:10;background:rgba(15,23,42,0.75);color:#ffffff;border:2px solid rgba(255,255,255,0.9);padding:0;line-height:1;"
                            data-bs-dismiss="modal"
                            aria-label="Đóng">
                        <i class="bi bi-x-lg" style="font-size:1rem;-webkit-text-stroke:0.5px #fff;"></i>
                    </button>
                    <img id="hdPopupImg" src="/images/samflora/b12-main.jpg" class="w-100" style="height:265px;object-fit:cover;object-position:center 35%" alt="Hot Deal CHAM Flowers" />
                </div>
                <div class="p-4 text-center bg-white">
                    <h4 id="hdPopupTitle" class="serif-heading fw-bold text-danger mb-2">Ưu đãi Hoa Tươi CHAM Flowers</h4>
                    <p id="hdPopupSub" class="text-muted small mb-3"></p>
                    <div class="d-none mb-3">
                        <span class="badge bg-danger-subtle text-danger fs-6 px-3 py-2 rounded-pill">
                            Mã ưu đãi: <strong id="hdPopupCoupon">CHAMFLOWERS15</strong>
                        </span>
                    </div>
                    <a id="hdPopupLink" href="/HotDeal" class="btn btn-nt-primary w-100 py-2">Xem ngay chương trình khuyến mãi</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Floating Live Chat Widget -->
    <button id="ntChatLauncher" class="nt-chat-launcher" title="Chat tư vấn trực tuyến cùng CHAM Flowers">
        <i class="bi bi-chat-heart-fill"></i>
    </button>

    <div id="ntChatPanel" class="nt-chat-panel">
        <div class="nt-chat-header d-flex justify-content-between align-items-center">
            <div class="d-flex align-items-center gap-2">
                <img src="~/images/logo-cham-icon.svg" class="rounded-circle bg-white p-0 shadow-sm" style="width:38px;height:38px" alt="CHAM Flowers" />
                <div>
                    <div class="fw-bold small">CSKH CHAM Flowers</div>
                    <div style="font-size:0.74rem"><span class="badge bg-success rounded-circle p-1 me-1"> </span>Trực tuyến • Tư vấn mẫu hoa 24/7</div>
                </div>
            </div>
            <button id="ntChatClose" type="button" class="btn btn-sm text-white"><i class="bi bi-x-lg"></i></button>
        </div>
        <div id="ntChatMessages" class="nt-chat-messages">
            <div class="chat-bubble system">Xin chào! Bạn cần tư vấn hoa sinh nhật, khai trương hay thiết kế bó hoa theo ngân sách? Nhắn cho CHAM Flowers ngay nhé!</div>
        </div>
        <form id="ntChatForm" class="p-2 border-top bg-white d-flex gap-2">
            <input id="ntChatInput" type="text" class="form-control form-control-sm rounded-pill px-3" placeholder="Nhập tin nhắn cần tư vấn..." autocomplete="off" />
            <button type="submit" class="btn btn-nt-primary btn-sm px-3"><i class="bi bi-send-fill"></i></button>
        </form>
    </div>

    <!-- Footer -->
    <footer class="nt-footer pt-5 pb-4 mt-5">
        <div class="container">
            <div class="row g-4">
                <div class="col-lg-5">
                    <img src="~/images/logo-cham-white.svg" alt="CHAM Flowers" class="cham-logo-footer mb-3" />
                    <p class="small opacity-75">Thương hiệu hoa tươi nghệ thuật cao cấp chuyên cung cấp hoa sinh nhật, hoa khai trương, lan hồ điệp và hoa cưới thiết kế độc bản. Cam kết hoa tươi mới mỗi ngày từ Đà Lạt & nhập khẩu.</p>
                    <div class="small opacity-75">
                        <div><i class="bi bi-geo-alt-fill text-danger me-2"></i>128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh</div>
                        <div class="mt-1"><i class="bi bi-telephone-fill text-danger me-2"></i>Hotline: 1900 6868 • 0909 123 456</div>
                        <div class="mt-1"><i class="bi bi-envelope-fill text-danger me-2"></i>cskh@chamflowers.vn</div>
                    </div>
                </div>
                <div class="col-6 col-lg-3">
                    <h6 class="text-white fw-bold mb-3">Danh mục hoa</h6>
                    <ul class="list-unstyled small opacity-75 d-grid gap-2">
                        <li><a href="/Product" class="text-reset text-decoration-none">Tất cả mẫu hoa</a></li>
                        <li><a href="/HotDeal" class="text-reset text-decoration-none">Hot Deal giảm giá</a></li>
                        <li><a href="/Home/News" class="text-reset text-decoration-none">Cẩm nang chăm hoa</a></li>
                        <li><a href="/Home/About" class="text-reset text-decoration-none">Về CHAM Flowers</a></li>
                    </ul>
                </div>
                <div class="col-6 col-lg-4">
                    <h6 class="text-white fw-bold mb-3">Hỗ trợ khách hàng</h6>
                    <ul class="list-unstyled small opacity-75 d-grid gap-2">
                        <li><a href="/Order/Cart" class="text-reset text-decoration-none">Giỏ hàng & Mã giảm giá</a></li>
                        <li><a href="/Order/MyOrders" class="text-reset text-decoration-none">Tra cứu trạng thái đơn hàng</a></li>
                        <li><span>Thanh toán bảo mật COD & VNPay</span></li>
                        <li><span>Bảo hành độ tươi hoa 72 giờ</span></li>
                    </ul>
                </div>
            </div>
            <hr class="border-secondary my-4" />
            <div class="text-center small opacity-75">
                &copy; 2026 CHAM Flowers.
            </div>
        </div>
    </footer>

    <script src="~/Scripts/jquery-3.7.0.min.js"></script>
    <script src="~/Scripts/bootstrap.bundle.min.js"></script>
    <script src="~/Scripts/jquery.signalR-2.2.2.min.js"></script>
    <script src="~/signalr/hubs"></script>
    <script src="~/Scripts/site.js"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>
```

### File: FlowerShop/Views\Shared\_LayoutAdmin.cshtml
```html
@{
    var ctrl = ViewContext.RouteData.Values["controller"] != null ? ViewContext.RouteData.Values["controller"].ToString() : "";
    var act = ViewContext.RouteData.Values["action"] != null ? ViewContext.RouteData.Values["action"].ToString() : "";

    int pendingOrdersCount = 0;
    int pendingReviewsCount = 0;
    int unreadChatsCount = 0;
    int lowStockCount = 0;

    using (var dbNav = new FlowerShop.Models.FlowerShopDbContext())
    {
        pendingOrdersCount = dbNav.Orders.AsNoTracking().Count(o => o.Status == "Chờ xác nhận");
        pendingReviewsCount = dbNav.Reviews.AsNoTracking().Count(r => !r.IsApproved && r.Status == FlowerShop.Models.ReviewStatus.Pending);
        unreadChatsCount = dbNav.ChatConversations.AsNoTracking().Sum(c => (int?)c.UnreadAdminCount) ?? 0;
        lowStockCount = dbNav.Products.AsNoTracking().Count(p => p.IsActive && p.StockQuantity <= 5);
    }

    int totalAlerts = pendingOrdersCount + pendingReviewsCount + unreadChatsCount + lowStockCount;

    var adminName = User != null && User.Identity != null && !string.IsNullOrWhiteSpace(User.Identity.Name)
        ? User.Identity.Name
        : "Nguyễn Quang Hưởng";
    var initials = AppHelpers.GetInitials(adminName);

    string sectionLabel = "Hệ thống Quản trị";
    switch (ctrl)
    {
        case "Report":
            sectionLabel = "Thống kê & Báo cáo";
            break;
        case "Product":
            sectionLabel = "Sản phẩm & Kho hoa";
            break;
        case "Order":
            sectionLabel = "Quản lý Bán hàng";
            break;
        case "HotDeal":
            sectionLabel = "Chiến dịch Khuyến mãi";
            break;
        case "Customer":
            sectionLabel = "Hội viên & Khách hàng";
            break;
        case "Review":
            sectionLabel = "Phản hồi & Đánh giá";
            break;
        case "Chat":
            sectionLabel = "Chăm sóc Trực tuyến";
            break;
        case "Banner":
            sectionLabel = "Nội dung & Marketing";
            break;
    }
}
<!DOCTYPE html>
<html lang="vi">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Quản trị CHAM Flowers</title>
    <link rel="icon" type="image/svg+xml" href="~/images/logo-cham-icon.svg" />
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:ital,wght@0,600;0,700;1,600&family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap&subset=vietnamese" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">
    <link rel="stylesheet" href="~/Content/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Content/site.css?v=20261104" />
</head>
<body>
    <div class="admin-wrapper">
        <!-- Sidebar Quản trị viên -->
        <aside class="admin-sidebar d-none d-lg-flex">
            <div class="admin-brand-box">
                <a href="@Url.Action("Index", "Report")" class="text-decoration-none d-flex align-items-center gap-3">
                    <img src="~/images/logo-cham-icon.svg" class="admin-brand-emblem" alt="CHAM Flowers CMS" />
                    <div>
                        <div class="admin-brand-title">CHAM Flowers</div>
                        <div class="admin-brand-badge">Executive CMS</div>
                    </div>
                </a>
            </div>

            <div class="admin-nav-scroll">
                <div class="admin-nav-section">Tổng quan & Phân tích</div>
                <a class="nav-link @(ctrl == "Report" ? "active" : "")" href="@Url.Action("Index", "Report")">
                    <span class="admin-nav-icon"><i class="bi bi-grid-1x2-fill"></i></span>
                    <span>Dashboard & Báo cáo</span>
                </a>

                <div class="admin-nav-section">Sản phẩm & Kho hoa</div>
                <a class="nav-link @(ctrl == "Product" && act != "Categories" ? "active" : "")" href="@Url.Action("AdminIndex", "Product")">
                    <span class="admin-nav-icon"><i class="bi bi-flower1"></i></span>
                    <span>Quản lý Sản phẩm</span>
                    @if (lowStockCount > 0)
                    {
                        <span class="admin-nav-badge" title="Sản phẩm sắp hết hàng">@lowStockCount</span>
                    }
                </a>
                <a class="nav-link @(ctrl == "Product" && act == "Categories" ? "active" : "")" href="@Url.Action("Categories", "Product")">
                    <span class="admin-nav-icon"><i class="bi bi-tags-fill"></i></span>
                    <span>Danh mục • Loại hoa • NCC</span>
                </a>

                <div class="admin-nav-section">Bán hàng & Khuyến mãi</div>
                <a class="nav-link @(ctrl == "Order" ? "active" : "")" href="@Url.Action("AdminIndex", "Order")">
                    <span class="admin-nav-icon"><i class="bi bi-bag-check-fill"></i></span>
                    <span>Quản lý Đơn hàng</span>
                    @if (pendingOrdersCount > 0)
                    {
                        <span class="admin-nav-badge" title="Đơn chờ xác nhận">@pendingOrdersCount</span>
                    }
                </a>
                <a class="nav-link @(ctrl == "HotDeal" ? "active" : "")" href="@Url.Action("Manage", "HotDeal")">
                    <span class="admin-nav-icon"><i class="bi bi-fire"></i></span>
                    <span>Hot Deal & Popup</span>
                </a>

                <div class="admin-nav-section">Khách hàng & CSKH</div>
                <a class="nav-link @(ctrl == "Customer" ? "active" : "")" href="@Url.Action("Index", "Customer")">
                    <span class="admin-nav-icon"><i class="bi bi-people-fill"></i></span>
                    <span>Tài khoản Khách hàng</span>
                </a>
                <a class="nav-link @(ctrl == "Review" ? "active" : "")" href="@Url.Action("Index", "Review")">
                    <span class="admin-nav-icon"><i class="bi bi-star-half"></i></span>
                    <span>Kiểm duyệt Đánh giá</span>
                    @if (pendingReviewsCount > 0)
                    {
                        <span class="admin-nav-badge">@pendingReviewsCount</span>
                    }
                </a>
                <a class="nav-link @(ctrl == "Chat" ? "active" : "")" href="@Url.Action("Index", "Chat")">
                    <span class="admin-nav-icon"><i class="bi bi-chat-heart-fill"></i></span>
                    <span>Chat Chăm sóc KH</span>
                    @if (unreadChatsCount > 0)
                    {
                        <span class="admin-nav-badge">@unreadChatsCount</span>
                    }
                </a>

                <div class="admin-nav-section">Nội dung & Quảng bá</div>
                <a class="nav-link @(ctrl == "Menu" ? "active" : "")" href="@Url.Action("Index", "Menu")">
                    <span class="admin-nav-icon"><i class="bi bi-menu-button-wide"></i></span>
                    <span>Quản lý Menu điều hướng</span>
                </a>
                <a class="nav-link @(ctrl == "Banner" ? "active" : "")" href="@Url.Action("Index", "Banner")">
                    <span class="admin-nav-icon"><i class="bi bi-images"></i></span>
                    <span>Banner • Tin tức • Voucher</span>
                </a>
            </div>

            <div class="admin-sidebar-footer">
                <div class="admin-profile-mini">
                    <div class="admin-avatar-ring">@initials</div>
                    <div class="min-w-0 flex-grow-1">
                        <div class="fw-bold text-white text-truncate d-flex align-items-center gap-1" style="font-size:0.85rem">
                            <span class="text-truncate">@adminName</span>
                            <i class="bi bi-patch-check-fill text-warning flex-shrink-0" style="font-size:0.78rem" title="Quản trị viên xác thực"></i>
                        </div>
                        <div class="d-flex align-items-center gap-1" style="font-size:0.71rem;color:#fde68a;font-weight:600">
                            <span class="admin-online-dot"></span> Quản trị viên cao cấp
                        </div>
                    </div>
                </div>
                <div class="d-flex gap-2">
                    <a href="@Url.Action("Index", "Home")" class="btn btn-sm admin-sidebar-btn rounded-pill flex-grow-1">
                        <i class="bi bi-shop me-1"></i> Cửa hàng
                    </a>
                    <a href="@Url.Action("Logout", "Account")" class="btn btn-sm admin-sidebar-btn admin-sidebar-btn-logout rounded-pill px-3" title="Đăng xuất">
                        <i class="bi bi-box-arrow-right"></i>
                    </a>
                </div>
            </div>
        </aside>

        <!-- Main Content -->
        <div class="admin-main">
            <header class="admin-topbar d-flex justify-content-between align-items-center gap-3">
                <div class="min-w-0">
                    <div class="admin-breadcrumb d-none d-sm-block">
                        CHAM Flowers CMS <i class="bi bi-chevron-right mx-1" style="font-size:0.65rem"></i> <span class="text-danger">@sectionLabel</span>
                    </div>
                    <h1 class="admin-page-title text-truncate">@ViewData["Title"]</h1>
                </div>

                <div class="d-flex align-items-center gap-2 flex-shrink-0">
                    <form action="@Url.Action("AdminIndex", "Product")" method="get" class="admin-quick-search d-none d-xl-flex">
                        <i class="bi bi-search text-muted me-2" style="font-size:0.82rem"></i>
                        <input type="text" name="search" placeholder="Tìm nhanh mẫu hoa trong kho..." autocomplete="off" />
                    </form>

                    <div class="admin-date-chip d-none d-md-inline-flex">
                        <i class="bi bi-calendar3 text-danger"></i>
                        <span>@DateTime.Now.ToString("dd/MM/yyyy")</span>
                    </div>

                    <a href="@Url.Action("Create", "Product")" class="btn btn-sm btn-admin-primary d-none d-sm-inline-flex align-items-center gap-1">
                        <i class="bi bi-plus-lg"></i> <span>Thêm hoa mới</span>
                    </a>

                    <!-- Chuông thông báo vận hành trực tiếp -->
                    <div class="dropdown">
                        <button class="admin-icon-pill" type="button" data-bs-toggle="dropdown" title="Thông báo hệ thống">
                            <i class="bi bi-bell-fill"></i>
                            @if (totalAlerts > 0)
                            {
                                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger" style="font-size:0.65rem;padding:0.25em 0.5em">
                                    @totalAlerts
                                </span>
                            }
                        </button>
                        <div class="dropdown-menu dropdown-menu-end shadow-lg border-0 rounded-4 p-3" style="width:310px">
                            <div class="d-flex justify-content-between align-items-center mb-2 pb-2 border-bottom">
                                <span class="fw-bold small">Trung tâm nhắc việc</span>
                                <span class="badge bg-danger-subtle text-danger">@totalAlerts mục</span>
                            </div>
                            <div class="d-grid gap-2 small">
                                <a href="@Url.Action("AdminIndex", "Order", new { status = "Chờ xác nhận" })" class="d-flex justify-content-between align-items-center text-decoration-none text-dark p-2 rounded-3 bg-light">
                                    <span><i class="bi bi-bag-check-fill text-warning me-2"></i>Đơn hàng chờ xác nhận</span>
                                    <span class="badge bg-warning text-dark">@pendingOrdersCount</span>
                                </a>
                                <a href="@Url.Action("AdminIndex", "Product")" class="d-flex justify-content-between align-items-center text-decoration-none text-dark p-2 rounded-3 bg-light">
                                    <span><i class="bi bi-exclamation-triangle-fill text-danger me-2"></i>Mẫu hoa sắp hết kho (≤ 5)</span>
                                    <span class="badge bg-danger">@lowStockCount</span>
                                </a>
                                <a href="@Url.Action("Index", "Chat")" class="d-flex justify-content-between align-items-center text-decoration-none text-dark p-2 rounded-3 bg-light">
                                    <span><i class="bi bi-chat-heart-fill text-primary me-2"></i>Tin nhắn khách chưa đọc</span>
                                    <span class="badge bg-primary">@unreadChatsCount</span>
                                </a>
                                <a href="@Url.Action("Index", "Review", new { filter = "pending" })" class="d-flex justify-content-between align-items-center text-decoration-none text-dark p-2 rounded-3 bg-light">
                                    <span><i class="bi bi-star-half text-success me-2"></i>Đánh giá chờ duyệt</span>
                                    <span class="badge bg-success">@pendingReviewsCount</span>
                                </a>
                            </div>
                        </div>
                    </div>

                    <!-- User Dropdown -->
                    <div class="dropdown">
                        <button class="btn btn-light border rounded-pill d-flex align-items-center gap-2 py-1 ps-1 pe-3" type="button" data-bs-toggle="dropdown" style="height:38px">
                            <span class="admin-avatar-ring" style="width:30px;height:30px;font-size:0.74rem">@initials</span>
                            <span class="small fw-bold text-dark d-none d-md-inline text-truncate" style="max-width:145px">@adminName</span>
                            <i class="bi bi-chevron-down text-muted" style="font-size:0.7rem"></i>
                        </button>
                        <ul class="dropdown-menu dropdown-menu-end shadow-lg border-0 rounded-4 p-2 small">
                            <li class="px-3 py-2 border-bottom mb-1">
                                <div class="fw-bold text-dark">@adminName</div>
                                <div class="text-muted" style="font-size:0.74rem">Quản trị hệ thống CHAM Flowers</div>
                            </li>
                            <li><a class="dropdown-item rounded-3 py-2" href="@Url.Action("Index", "Home")"><i class="bi bi-shop me-2 text-danger"></i>Xem giao diện Cửa hàng</a></li>
                            <li><a class="dropdown-item rounded-3 py-2" href="@Url.Action("Profile", "Account")"><i class="bi bi-person-gear me-2 text-danger"></i>Hồ sơ Quản trị viên</a></li>
                            <li><hr class="dropdown-divider" /></li>
                            <li><a class="dropdown-item rounded-3 py-2 text-danger fw-semibold" href="@Url.Action("Logout", "Account")"><i class="bi bi-box-arrow-right me-2"></i>Đăng xuất</a></li>
                        </ul>
                    </div>
                </div>
            </header>

            <!-- Mobile Quick Nav -->
            <div class="px-3 py-2 d-flex d-lg-none gap-2 overflow-auto" style="background:#18181b">
                <a href="@Url.Action("Index", "Report")" class="btn btn-sm @(ctrl == "Report" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Dashboard</a>
                <a href="@Url.Action("AdminIndex", "Product")" class="btn btn-sm @(ctrl == "Product" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Sản phẩm</a>
                <a href="@Url.Action("AdminIndex", "Order")" class="btn btn-sm @(ctrl == "Order" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Đơn hàng</a>
                <a href="@Url.Action("Index", "Customer")" class="btn btn-sm @(ctrl == "Customer" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Khách hàng</a>
                <a href="@Url.Action("Manage", "HotDeal")" class="btn btn-sm @(ctrl == "HotDeal" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Hot Deal</a>
                <a href="@Url.Action("Index", "Review")" class="btn btn-sm @(ctrl == "Review" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Đánh giá</a>
                <a href="@Url.Action("Index", "Chat")" class="btn btn-sm @(ctrl == "Chat" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Chat</a>
                <a href="@Url.Action("Index", "Banner")" class="btn btn-sm @(ctrl == "Banner" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Banner</a>
                <a href="@Url.Action("Index", "Menu")" class="btn btn-sm @(ctrl == "Menu" ? "btn-danger" : "btn-outline-light") rounded-pill text-nowrap">Menu</a>
            </div>

            <div class="admin-content-area">
                @if (TempData["Success"] != null)
                {
                    <div class="alert alert-success alert-dismissible fade show rounded-4 border-0 shadow-sm mb-4" role="alert">
                        <i class="bi bi-check-circle-fill me-2"></i> @TempData["Success"]
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                }
                @if (TempData["Error"] != null)
                {
                    <div class="alert alert-danger alert-dismissible fade show rounded-4 border-0 shadow-sm mb-4" role="alert">
                        <i class="bi bi-exclamation-triangle-fill me-2"></i> @TempData["Error"]
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                }

                @RenderBody()
            </div>
        </div>
    </div>

    <script src="~/Scripts/jquery-3.7.0.min.js"></script>
    <script src="~/Scripts/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.3/dist/chart.umd.min.js"></script>
    <script src="~/Scripts/jquery.signalR-2.2.2.min.js"></script>
    <script src="~/signalr/hubs"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>
```

### File: FlowerShop/Views\Home\Index.cshtml
```html
@model FlowerShop.Models.HomeViewModel
@{
    ViewData["Title"] = "Trang chủ";
    DateTime countdownDate = Model.ActivePopup != null && Model.ActivePopup.EndDate.HasValue
        ? Model.ActivePopup.EndDate.Value
        : DateTime.Now.AddDays(5);
}

<div class="container mt-3">
    <!-- Hero Carousel -->
    @if (Model.Banners.Any())
    {
        <div id="ntHeroCarousel" class="carousel slide mb-5 shadow-sm rounded-4 overflow-hidden" data-bs-ride="carousel">
            <div class="carousel-indicators">
                @for (int i = 0; i < Model.Banners.Count; i++)
                {
                    <button type="button" data-bs-target="#ntHeroCarousel" data-bs-slide-to="@i" class="@(i == 0 ? "active" : "")"></button>
                }
            </div>
            <div class="carousel-inner">
                @for (int i = 0; i < Model.Banners.Count; i++)
                {
                    var b = Model.Banners[i];
                    <div class="carousel-item @(i == 0 ? "active" : "")">
                        <div class="hero-slide" style="background-image: url('@b.ImageUrl');">
                            <div class="hero-overlay">
                                <span class="badge bg-white text-danger rounded-pill px-3 py-2 mb-3 fw-bold">
                                    <i class="bi bi-stars me-1"></i> CHAM Flowers Boutique
                                </span>
                                <h1 class="display-5 fw-bold mb-3">@b.Title</h1>
                                <p class="lead opacity-90 mb-4">@b.Subtitle</p>
                                <div class="d-flex flex-wrap gap-3">
                                    <a href="@(string.IsNullOrEmpty(b.LinkUrl) ? "/Product" : b.LinkUrl)" class="btn btn-light text-danger fw-bold rounded-pill px-4 py-2 shadow-sm">
                                        @(string.IsNullOrEmpty(b.ButtonText) ? "Đặt hoa ngay" : b.ButtonText) <i class="bi bi-arrow-right ms-1"></i>
                                    </a>
                                    <a href="@Url.Action("Index", "HotDeal")" class="btn btn-outline-light rounded-pill px-4 py-2">
                                        <i class="bi bi-fire me-1"></i> Săn Hot Deal
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                }
            </div>
            <button class="carousel-control-prev" type="button" data-bs-target="#ntHeroCarousel" data-bs-slide="prev">
                <span class="carousel-control-prev-icon"></span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#ntHeroCarousel" data-bs-slide="next">
                <span class="carousel-control-next-icon"></span>
            </button>
        </div>
    }

    <!-- Cam kết dịch vụ -->
    <div class="row g-3 mb-5">
        <div class="col-6 col-lg-3">
            <div class="p-3 bg-white rounded-4 border h-100 d-flex align-items-center gap-3">
                <span class="fs-2 text-danger"><i class="bi bi-truck"></i></span>
                <div>
                    <div class="fw-bold small">Giao Hoa Hỏa Tốc 2H</div>
                    <div class="text-muted" style="font-size:0.78rem">Gửi ảnh thực tế trước khi giao</div>
                </div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="p-3 bg-white rounded-4 border h-100 d-flex align-items-center gap-3">
                <span class="fs-2 text-danger"><i class="bi bi-flower1"></i></span>
                <div>
                    <div class="fw-bold small">100% Hoa Tươi Tuyển Chọn</div>
                    <div class="text-muted" style="font-size:0.78rem">Nhập mới mỗi sáng từ Đà Lạt</div>
                </div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="p-3 bg-white rounded-4 border h-100 d-flex align-items-center gap-3">
                <span class="fs-2 text-danger"><i class="bi bi-gift"></i></span>
                <div>
                    <div class="fw-bold small">Miễn Phí Thiệp & Banner</div>
                    <div class="text-muted" style="font-size:0.78rem">Thiết kế thông điệp tinh tế</div>
                </div>
            </div>
        </div>
        <div class="col-6 col-lg-3">
            <div class="p-3 bg-white rounded-4 border h-100 d-flex align-items-center gap-3">
                <span class="fs-2 text-danger"><i class="bi bi-shield-check"></i></span>
                <div>
                    <div class="fw-bold small">Thanh Toán An Toàn</div>
                    <div class="text-muted" style="font-size:0.78rem">Hỗ trợ COD & Cổng VNPay</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Bộ sưu tập theo Chủng loại hoa -->
    <section class="mb-5">
        <div class="d-flex justify-content-between align-items-end mb-3">
            <div>
                <span class="text-danger fw-bold small text-uppercase">Bộ sưu tập</span>
                <h2 class="h3 fw-bold mb-0">Khám Phá Theo Loài Hoa</h2>
            </div>
            <a href="@Url.Action("Index", "Product")" class="text-danger text-decoration-none fw-semibold small">Xem tất cả <i class="bi bi-chevron-right"></i></a>
        </div>
        <div class="row g-3">
            @foreach (var fc in Model.FlowerCategories)
            {
                <div class="col-6 col-md-4 col-lg">
                    <a href="@Url.Action("Index", "Product", new { flowerCategoryId = fc.Id })" class="flower-cat-card">
                        <img src="@(fc.ImageUrl ?? "/images/samflora/b10-main.jpg")" alt="@fc.Name" />
                        <div class="fw-bold small">@fc.Name</div>
                        <div class="text-muted mt-1" style="font-size:0.74rem">@fc.Description</div>
                    </a>
                </div>
            }
        </div>
    </section>

    <!-- Hot Deal Khuyến Mãi kèm Countdown -->
    @if (Model.HotDealProducts.Any())
    {
        <section class="p-4 rounded-4 mb-5 border" style="background: linear-gradient(135deg, #fff1f2 0%, #ffe4e6 100%);">
            <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
                <div>
                    <span class="badge bg-danger rounded-pill px-3 py-1 mb-2"><i class="bi bi-lightning-charge-fill"></i> FLASH SALE HOA TƯƠI</span>
                    <h2 class="h3 fw-bold mb-0 text-dark">Ưu Đãi Hot Deal Trong Tuần</h2>
                </div>
                <div class="d-flex align-items-center gap-3">
                    <div class="countdown-box" data-countdown="@countdownDate.ToString("yyyy-MM-ddTHH:mm:ss")">
                        <div class="countdown-unit"><strong class="cd-days">05</strong><small>Ngày</small></div>
                        <div class="countdown-unit"><strong class="cd-hours">12</strong><small>Giờ</small></div>
                        <div class="countdown-unit"><strong class="cd-mins">45</strong><small>Phút</small></div>
                        <div class="countdown-unit"><strong class="cd-secs">30</strong><small>Giây</small></div>
                    </div>
                    <a href="@Url.Action("Index", "HotDeal")" class="btn btn-nt-primary d-none d-sm-inline-block">Xem tất cả</a>
                </div>
            </div>
            <div class="row g-4">
                @foreach (var p in Model.HotDealProducts)
                {
                    <div class="col-sm-6 col-lg-3">
                        @Html.Partial("_ProductCard", p)
                    </div>
                }
            </div>
        </section>
    }

    <!-- Sản phẩm nổi bật -->
    <section class="mb-5">
        <div class="d-flex flex-wrap justify-content-between align-items-end gap-2 mb-4">
            <div>
                <span class="text-danger fw-bold small text-uppercase">Best Sellers</span>
                <h2 class="h3 fw-bold mb-0">Mẫu Hoa Nổi Bật Được Yêu Thích</h2>
            </div>
            <div class="d-flex flex-wrap gap-2">
                @foreach (var cat in Model.Categories)
                {
                    <a href="@Url.Action("Index", "Product", new { categoryId = cat.Id })"
                       class="btn btn-sm btn-light border rounded-pill px-3">@cat.Name</a>
                }
            </div>
        </div>
        <div class="row g-4">
            @foreach (var p in Model.FeaturedProducts)
            {
                <div class="col-sm-6 col-md-4 col-lg-3">
                    @Html.Partial("_ProductCard", p)
                </div>
            }
        </div>
    </section>

    <!-- Mã giảm giá đang áp dụng -->
    @if (Model.ActiveCoupons.Any())
    {
        <section class="mb-5">
            <h3 class="h4 fw-bold mb-3"><i class="bi bi-ticket-perforated-fill text-danger me-2"></i>Mã Ưu Đãi Dành Cho Bạn</h3>
            <div class="row g-3">
                @foreach (var c in Model.ActiveCoupons)
                {
                    <div class="col-md-4">
                        <div class="bg-white border border-danger border-opacity-25 rounded-4 p-3 d-flex justify-content-between align-items-center shadow-sm">
                            <div>
                                <div class="d-flex align-items-center gap-2">
                                    <span class="badge bg-danger fs-6">@c.Code</span>
                                    @if (c.VipOnly)
                                    {
                                        <span class="badge bg-warning text-dark">Khách VIP</span>
                                    }
                                </div>
                                <div class="small fw-semibold mt-2">@c.Description</div>
                                <div class="text-muted" style="font-size:0.75rem">Đơn tối thiểu @AppHelpers.FormatVnd(c.MinOrderAmount)</div>
                            </div>
                            <button type="button" class="btn btn-sm btn-nt-outline"
                                    onclick="if(navigator.clipboard){navigator.clipboard.writeText('@c.Code');} showNtToast('Đã sao chép mã @c.Code!', true);">
                                Sao chép
                            </button>
                        </div>
                    </div>
                }
            </div>
        </section>
    }

    <!-- Cẩm nang & Tin tức -->
    @if (Model.LatestNews.Any())
    {
        <section>
            <div class="d-flex justify-content-between align-items-end mb-3">
                <div>
                    <span class="text-danger fw-bold small text-uppercase">Góc chia sẻ</span>
                    <h2 class="h3 fw-bold mb-0">Cẩm Nang & Ý Nghĩa Hoa Tươi</h2>
                </div>
                <a href="@Url.Action("News", "Home")" class="text-danger text-decoration-none fw-semibold small">Xem thêm bài viết <i class="bi bi-chevron-right"></i></a>
            </div>
            <div class="row g-4">
                @foreach (var art in Model.LatestNews)
                {
                    <div class="col-md-4">
                        <div class="flower-card">
                            <a href="@Url.Action("NewsDetail", "Home", new { id = art.Id })" class="text-decoration-none">
                                <img src="@(art.ImageUrl ?? "/images/samflora/editorial-2.jpg")" class="w-100" style="height:210px;object-fit:cover" alt="@art.Title" />
                            </a>
                            <div class="p-3 d-flex flex-column flex-grow-1">
                                <span class="badge bg-danger-subtle text-danger align-self-start mb-2">@art.CategoryName</span>
                                <h6 class="fw-bold">
                                    <a href="@Url.Action("NewsDetail", "Home", new { id = art.Id })" class="text-dark text-decoration-none">@art.Title</a>
                                </h6>
                                <p class="text-muted small mb-0 flex-grow-1">@art.Summary</p>
                            </div>
                        </div>
                    </div>
                }
            </div>
        </section>
    }
</div>

@section Scripts {
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            setTimeout(function () {
                if (window['checkHotDealPopup']) window['checkHotDealPopup'](false);
            }, 1200);
        });
    </script>
}
```

### File: FlowerShop/Views\Home\About.cshtml
```html
@{
    ViewData["Title"] = "Giới thiệu cửa hàng CHAM Flowers";
}

<div class="container py-4">
    <div class="row align-items-center g-5 mb-5">
        <div class="col-lg-6">
            <span class="badge bg-danger-subtle text-danger px-3 py-2 rounded-pill mb-3 fw-semibold">Câu chuyện thương hiệu</span>
            <h1 class="display-6 fw-bold mb-3">CHAM Flowers – Nơi Mỗi Đóa Hoa Kể Một Câu Chuyện Cảm Xúc</h1>
            <p class="text-muted">Khởi nguồn từ niềm đam mê nghệ thuật cắm hoa đương đại, <strong>CHAM Flowers</strong> mang sứ mệnh kết nối tâm hồn và truyền tải trọn vẹn thông điệp yêu thương thông qua những đóa hoa tươi tuyển chọn khắt khe nhất từ cao nguyên Đà Lạt và các nông trại danh tiếng tại Hà Lan, Ecuador.</p>
            <div class="row g-3 mt-2">
                <div class="col-sm-6">
                    <div class="p-3 bg-white border rounded-4">
                        <h4 class="fw-bold text-danger mb-1">10.000+</h4>
                        <div class="small text-muted">Đơn hoa trao gửi thành công</div>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="p-3 bg-white border rounded-4">
                        <h4 class="fw-bold text-danger mb-1">90 Phút</h4>
                        <div class="small text-muted">Thời gian giao hoa hỏa tốc trung bình</div>
                    </div>
                </div>
            </div>
            <div class="mt-4 d-flex gap-3">
                <a href="@Url.Action("Index", "Product")" class="btn btn-nt-primary">Xem bộ sưu tập hoa</a>
                <a href="@Url.Action("Index", "HotDeal")" class="btn btn-nt-outline">Chương trình ưu đãi</a>
            </div>
        </div>
        <div class="col-lg-6">
            <img src="/images/samflora/editorial-4.jpg" class="img-fluid rounded-4 shadow-sm border w-100" style="max-height:480px;object-fit:cover" alt="CHAM Flowers Boutique" />
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Home\News.cshtml
```html
@model List<FlowerShop.Models.NewsArticle>
@{
    ViewData["Title"] = "Tin tức & Cẩm nang hoa tươi";
}

<div class="container py-4">
    <div class="text-center mb-5">
        <span class="text-danger fw-bold small text-uppercase">Góc yêu hoa</span>
        <h1 class="h2 fw-bold">Tin Tức & Cẩm Nang Hoa Tươi CHAM Flowers</h1>
        <p class="text-muted">Bí quyết chăm sóc hoa tươi lâu và nghệ thuật tặng hoa tinh tế cho từng dịp đặc biệt.</p>
    </div>

    <div class="row g-4">
        @foreach (var item in Model)
        {
            <div class="col-md-4">
                <div class="flower-card">
                    <a href="@Url.Action("NewsDetail", "Home", new { id = item.Id })">
                        <img src="@(item.ImageUrl ?? "/images/samflora/editorial-2.jpg")" class="w-100" style="height:230px;object-fit:cover" alt="@item.Title" />
                    </a>
                    <div class="p-4 d-flex flex-column flex-grow-1">
                        <div class="d-flex justify-content-between align-items-center mb-2">
                            <span class="badge bg-danger-subtle text-danger">@item.CategoryName</span>
                            <small class="text-muted">@item.CreatedAt.ToString("dd/MM/yyyy")</small>
                        </div>
                        <h5 class="fw-bold mb-2">
                            <a href="@Url.Action("NewsDetail", "Home", new { id = item.Id })" class="text-dark text-decoration-none">@item.Title</a>
                        </h5>
                        <p class="text-muted small flex-grow-1">@item.Summary</p>
                        <a href="@Url.Action("NewsDetail", "Home", new { id = item.Id })" class="btn btn-sm btn-nt-outline align-self-start">Đọc chi tiết →</a>
                    </div>
                </div>
            </div>
        }
    </div>
</div>
```

### File: FlowerShop/Views\Product\Index.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.Product>
@{
    ViewData["Title"] = "Danh mục sản phẩm hoa tươi";
    var categories = ViewBag.Categories as List<FlowerShop.Models.Category> ?? new List<FlowerShop.Models.Category>();
    var flowerCategories = ViewBag.FlowerCategories as List<FlowerShop.Models.FlowerCategory> ?? new List<FlowerShop.Models.FlowerCategory>();
    int? selCat = ViewBag.CategoryId;
    int? selFlowerCat = ViewBag.FlowerCategoryId;
    string search = ViewBag.Search;
    decimal? minPrice = ViewBag.MinPrice;
    decimal? maxPrice = ViewBag.MaxPrice;
    string sortOrder = ViewBag.SortOrder;
}

<div class="container py-4">
    <div class="row g-4">
        <!-- Bộ lọc bên trái -->
        <div class="col-lg-3">
            <div class="bg-white p-4 rounded-4 border shadow-sm sticky-top" style="top:86px">
                <h5 class="fw-bold mb-3"><i class="bi bi-sliders text-danger me-2"></i>Bộ Lọc Tìm Hoa</h5>
                <form action="@Url.Action("Index", "Product")" method="get">
                    <div class="mb-3">
                        <label class="form-label small fw-semibold">Từ khóa tên hoa</label>
                        <input type="text" name="search" value="@search" class="form-control form-control-sm rounded-3" placeholder="VD: Hồng Trà, Lan..." />
                    </div>

                    <div class="mb-3">
                        <label class="form-label small fw-semibold">Chủ đề / Dịp tặng</label>
                        <select name="categoryId" class="form-select form-select-sm rounded-3">
                            <option value="">-- Tất cả danh mục --</option>
                            @foreach (var c in categories)
                            {
                                <option value="@c.Id" @Html.Raw(selCat == c.Id ? "selected=\"selected\"" : "")>@c.Name</option>
                            }
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label small fw-semibold">Chủng loại hoa</label>
                        <select name="flowerCategoryId" class="form-select form-select-sm rounded-3">
                            <option value="">-- Tất cả loài hoa --</option>
                            @foreach (var fc in flowerCategories)
                            {
                                <option value="@fc.Id" @Html.Raw(selFlowerCat == fc.Id ? "selected=\"selected\"" : "")>@fc.Name</option>
                            }
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label small fw-semibold">Khoảng giá (VNĐ)</label>
                        <div class="row g-2">
                            <div class="col-6">
                                <input type="number" name="minPrice" value="@minPrice" step="50000" min="0" class="form-control form-control-sm" placeholder="Từ" />
                            </div>
                            <div class="col-6">
                                <input type="number" name="maxPrice" value="@maxPrice" step="50000" min="0" class="form-control form-control-sm" placeholder="Đến" />
                            </div>
                        </div>
                        <div class="d-flex flex-wrap gap-1 mt-2">
                            <a href="@Url.Action("Index", "Product", new { maxPrice = 600000 })" class="badge bg-light text-dark border text-decoration-none">&lt; 600K</a>
                            <a href="@Url.Action("Index", "Product", new { minPrice = 600000, maxPrice = 1200000 })" class="badge bg-light text-dark border text-decoration-none">600K - 1.2M</a>
                            <a href="@Url.Action("Index", "Product", new { minPrice = 1200000 })" class="badge bg-light text-dark border text-decoration-none">&gt; 1.2M</a>
                        </div>
                    </div>

                    <div class="mb-3">
                        <label class="form-label small fw-semibold">Sắp xếp theo</label>
                        <select name="sortOrder" class="form-select form-select-sm rounded-3">
                            <option value="" @Html.Raw(string.IsNullOrEmpty(sortOrder) ? "selected=\"selected\"" : "")>Mới nhất</option>
                            <option value="featured" @Html.Raw(sortOrder == "featured" ? "selected=\"selected\"" : "")>Nổi bật nhất</option>
                            <option value="price_asc" @Html.Raw(sortOrder == "price_asc" ? "selected=\"selected\"" : "")>Giá tăng dần</option>
                            <option value="price_desc" @Html.Raw(sortOrder == "price_desc" ? "selected=\"selected\"" : "")>Giá giảm dần</option>
                            <option value="name_asc" @Html.Raw(sortOrder == "name_asc" ? "selected=\"selected\"" : "")>Tên A → Z</option>
                        </select>
                    </div>

                    <div class="d-grid gap-2">
                        <button type="submit" class="btn btn-nt-primary btn-sm"><i class="bi bi-funnel-fill me-1"></i> Áp dụng lọc</button>
                        <a href="@Url.Action("Index", "Product")" class="btn btn-light border btn-sm rounded-pill">Đặt lại bộ lọc</a>
                    </div>
                </form>
            </div>
        </div>

        <!-- Danh sách sản phẩm -->
        <div class="col-lg-9">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <div>
                    <h1 class="h4 fw-bold mb-0">Bộ Sưu Tập Hoa Tươi CHAM Flowers</h1>
                    <div class="text-muted small">Tìm thấy <strong>@Model.TotalItemCount</strong> mẫu hoa phù hợp</div>
                </div>
            </div>

            @if (!Model.Any())
            {
                <div class="bg-white p-5 rounded-4 border text-center">
                    <i class="bi bi-search text-muted display-4"></i>
                    <h5 class="fw-bold mt-3">Không tìm thấy sản phẩm phù hợp</h5>
                    <p class="text-muted small">Vui lòng thử từ khóa hoặc khoảng giá khác để xem thêm mẫu hoa tại CHAM Flowers.</p>
                    <a href="@Url.Action("Index", "Product")" class="btn btn-nt-outline btn-sm">Xem toàn bộ sản phẩm</a>
                </div>
            }
            else
            {
                <div class="row g-4">
                    @foreach (var p in Model)
                    {
                        <div class="col-sm-6 col-md-4">
                            @Html.Partial("_ProductCard", p)
                        </div>
                    }
                </div>

                if (Model.PageCount > 1)
                {
                    <nav class="mt-4 d-flex justify-content-center">
                        <ul class="pagination">
                            @for (int i = 1; i <= Model.PageCount; i++)
                            {
                                <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                                    <a class="page-link"
                                       href="@Url.Action("Index", "Product", new { page = i, search = search, categoryId = selCat, flowerCategoryId = selFlowerCat, minPrice = minPrice, maxPrice = maxPrice, sortOrder = sortOrder })">@i</a>
                                </li>
                            }
                        </ul>
                    </nav>
                }
            }
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Product\Details.cshtml
```html
@model FlowerShop.Models.ProductDetailViewModel
@{
    var p = Model.Product;
    ViewData["Title"] = p.Name;
}

<div class="container py-4">
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb small">
            <li class="breadcrumb-item"><a href="@Url.Action("Index", "Home")">Trang chủ</a></li>
            <li class="breadcrumb-item"><a href="@Url.Action("Index", "Product")">Sản phẩm</a></li>
            @if (p.Category != null)
            {
                <li class="breadcrumb-item"><a href="@Url.Action("Index", "Product", new { categoryId = p.CategoryId })">@p.Category.Name</a></li>
            }
            <li class="breadcrumb-item active">@p.Name</li>
        </ol>
    </nav>

    <div class="bg-white p-4 p-lg-5 rounded-4 border shadow-sm mb-5">
        <div class="row g-5">
            <!-- Gallery Hình ảnh sản phẩm -->
            <div class="col-lg-5">
                <div class="rounded-4 overflow-hidden border bg-light mb-3">
                    <img id="mainProductImg" src="@p.MainImageUrl" class="w-100" style="aspect-ratio:1/1.05;object-fit:cover" alt="@p.Name" />
                </div>
                @if (p.Images.Count > 1)
                {
                    <div class="d-flex gap-2 flex-wrap">
                        @foreach (var img in p.Images.OrderByDescending(i => i.IsMain))
                        {
                            <img src="@img.ImageUrl"
                                 onclick="document.getElementById('mainProductImg').src=this.src"
                                 class="rounded-3 border"
                                 style="width:74px;height:74px;object-fit:cover;cursor:pointer"
                                 alt="Gallery" />
                        }
                    </div>
                }
            </div>

            <!-- Thông tin sản phẩm -->
            <div class="col-lg-7">
                <div class="d-flex flex-wrap gap-2 mb-2">
                    @if (p.Category != null)
                    {
                        <span class="badge bg-danger-subtle text-danger rounded-pill px-3 py-1">@p.Category.Name</span>
                    }
                    @if (p.FlowerCategory != null)
                    {
                        <span class="badge bg-light text-dark border rounded-pill px-3 py-1">@p.FlowerCategory.Name</span>
                    }
                    @if (p.IsHotDeal)
                    {
                        <span class="badge bg-warning text-dark rounded-pill px-3 py-1"><i class="bi bi-fire"></i> Hot Deal</span>
                    }
                </div>

                <h1 class="h2 fw-bold mb-2">@p.Name</h1>

                <div class="d-flex align-items-center gap-3 mb-3 small">
                    <span class="text-warning fw-bold"><i class="bi bi-star-fill"></i> @p.AverageRating.ToString("0.0") / 5.0</span>
                    <span class="text-muted">(@Model.ApprovedReviews.Count đánh giá đã duyệt)</span>
                    <span>•</span>
                    @if (p.StockQuantity > 0)
                    {
                        <span class="text-success fw-semibold"><i class="bi bi-check-circle-fill me-1"></i>Còn hàng (@p.StockQuantity sản phẩm)</span>
                    }
                    else
                    {
                        <span class="text-danger fw-semibold"><i class="bi bi-x-circle-fill me-1"></i>Tạm hết hàng</span>
                    }
                </div>

                <div class="p-3 rounded-4 bg-danger-subtle bg-opacity-50 mb-4 d-flex align-items-baseline gap-3">
                    <span class="h2 fw-bold text-danger mb-0">@AppHelpers.FormatVnd(p.Price)</span>
                    @if (p.OriginalPrice.HasValue && p.OriginalPrice > p.Price)
                    {
                        <span class="text-muted text-decoration-line-through fs-5">@AppHelpers.FormatVnd(p.OriginalPrice.Value)</span>
                        <span class="badge bg-danger">Tiết kiệm @p.DiscountPercent%</span>
                    }
                </div>

                <p class="text-secondary lh-lg mb-4">@p.Description</p>

                @if (p.StockQuantity > 0)
                {
                    <form action="@Url.Action("AddToCart", "Order")" method="post" class="d-flex flex-wrap align-items-center gap-3 mb-4">
                        <input type="hidden" name="productId" value="@p.Id" />
                        <div class="input-group" style="width:135px">
                            <button class="btn btn-outline-secondary" type="button" onclick="var q=document.getElementById('buyQty'); if(q.value>1) q.value--;">-</button>
                            <input type="number" id="buyQty" name="quantity" value="1" min="1" max="@p.StockQuantity" class="form-control text-center fw-bold" />
                            <button class="btn btn-outline-secondary" type="button" onclick="var q=document.getElementById('buyQty'); if(parseInt(q.value)<@p.StockQuantity) q.value++;">+</button>
                        </div>

                        <button type="button" data-id="@p.Id" onclick="window['addToCartAjax'] &amp;&amp; window['addToCartAjax'](this.getAttribute('data-id'), parseInt(document.getElementById('buyQty').value || 1))" class="btn btn-nt-outline px-4 py-2">
                            <i class="bi bi-cart-plus me-1"></i> Thêm vào giỏ
                        </button>

                        <button type="submit" class="btn btn-nt-primary px-4 py-2">
                            <i class="bi bi-bag-check-fill me-1"></i> Mua ngay
                        </button>
                    </form>
                }

                <div class="row g-2 small text-muted border-top pt-3">
                    <div class="col-sm-6"><i class="bi bi-gift-fill text-danger me-2"></i>Tặng kèm thiệp hoặc banner chúc mừng</div>
                    <div class="col-sm-6"><i class="bi bi-camera-fill text-danger me-2"></i>Chụp ảnh sản phẩm thực tế duyệt trước khi giao</div>
                    <div class="col-sm-6"><i class="bi bi-clock-fill text-danger me-2"></i>Giao nhanh 90 - 120 phút nội thành</div>
                    @if (p.Supplier != null)
                    {
                        <div class="col-sm-6"><i class="bi bi-award-fill text-danger me-2"></i>Nguồn hoa: @p.Supplier.Name</div>
                    }
                </div>
            </div>
        </div>
    </div>

    <!-- Đánh giá sản phẩm -->
    <div class="bg-white p-4 rounded-4 border shadow-sm mb-5">
        <h4 class="fw-bold mb-4"><i class="bi bi-chat-square-quote-fill text-danger me-2"></i>Đánh Giá Từ Khách Hàng Đã Mua (@Model.ApprovedReviews.Count)</h4>

        @if (Model.CanReview)
        {
            <div class="p-4 rounded-4 bg-light border mb-4">
                <h6 class="fw-bold mb-2">Gửi đánh giá trải nghiệm của bạn</h6>
                <form action="@Url.Action("Submit", "Review")" method="post">
                    @Html.AntiForgeryToken()
                    <input type="hidden" name="productId" value="@p.Id" />
                    <div class="row g-3">
                        <div class="col-md-3">
                            <label class="form-label small fw-semibold">Mức độ hài lòng</label>
                            <select name="rating" class="form-select">
                                <option value="5">⭐⭐⭐⭐⭐ Tuyệt vời (5 sao)</option>
                                <option value="4">⭐⭐⭐⭐ Rất tốt (4 sao)</option>
                                <option value="3">⭐⭐⭐ Hài lòng (3 sao)</option>
                                <option value="2">⭐⭐ Cần cải thiện (2 sao)</option>
                                <option value="1">⭐ Không hài lòng (1 sao)</option>
                            </select>
                        </div>
                        <div class="col-md-9">
                            <label class="form-label small fw-semibold">Nhận xét chi tiết</label>
                            <textarea name="content" rows="2" class="form-control" required placeholder="Chia sẻ cảm nhận về độ tươi của hoa và dịch vụ giao hàng CHAM Flowers..."></textarea>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-nt-primary btn-sm mt-3">Gửi đánh giá</button>
                </form>
            </div>
        }
        else if (User != null && User.Identity != null && User.Identity.IsAuthenticated && !Model.HasReviewed)
        {
            <div class="alert alert-light border small text-muted">
                <i class="bi bi-info-circle me-1"></i> Theo chính sách đảm bảo đánh giá xác thực của CHAM Flowers, khách hàng có thể gửi đánh giá sau khi đơn hàng chứa sản phẩm này ở trạng thái <strong>Hoàn thành</strong>.
            </div>
        }

        @if (!Model.ApprovedReviews.Any())
        {
            <p class="text-muted small mb-0">Chưa có đánh giá công khai nào cho mẫu hoa này.</p>
        }
        else
        {
            <div class="d-grid gap-3">
                @foreach (var rev in Model.ApprovedReviews)
                {
                    <div class="p-3 rounded-4 border bg-light bg-opacity-50">
                        <div class="d-flex justify-content-between align-items-center mb-1">
                            <div class="fw-bold small">
                                <i class="bi bi-person-check-fill text-success me-1"></i>@(rev.User != null ? rev.User.FullName : "Khách hàng")
                                <span class="badge bg-success-subtle text-success ms-1">Đã mua hoa</span>
                            </div>
                            <span class="text-muted" style="font-size:0.75rem">@rev.CreatedAt.ToString("dd/MM/yyyy HH:mm")</span>
                        </div>
                        <div class="text-warning small mb-1">
                            @for (int s = 0; s < rev.Rating; s++)
                            {
                                <i class="bi bi-star-fill"></i>
                            }
                        </div>
                        <div class="small text-dark">@rev.Content</div>
                    </div>
                }
            </div>
        }
    </div>

    <!-- Sản phẩm liên quan -->
    @if (Model.RelatedProducts.Any())
    {
        <section>
            <h4 class="fw-bold mb-3">Mẫu Hoa Cùng Chủ Đề</h4>
            <div class="row g-4">
                @foreach (var rp in Model.RelatedProducts)
                {
                    <div class="col-sm-6 col-lg-3">
                        @Html.Partial("_ProductCard", rp)
                    </div>
                }
            </div>
        </section>
    }
</div>
```

### File: FlowerShop/Views\Order\Cart.cshtml
```html
@model FlowerShop.Models.CartViewModel
@using FlowerShop.Services
@{
    ViewBag.Title = "Giỏ hàng hoa tươi - CHAM Flowers";
}

<div class="container py-4">
    <h1 class="h3 fw-bold mb-4"><i class="bi bi-bag-heart-fill text-danger me-2"></i>Giỏ Hàng Của Bạn</h1>

    @if (!Model.Cart.CartItems.Any())
    {
        <div class="bg-white p-5 rounded-4 border text-center shadow-sm">
            <i class="bi bi-cart-x text-muted display-3"></i>
            <h4 class="fw-bold mt-3">Giỏ hàng chưa có sản phẩm nào</h4>
            <p class="text-muted">Hãy chọn những bó hoa tươi thắm nhất dành tặng người thân yêu nhé!</p>
            <a href="@Url.Action("Index", "Product")" class="btn btn-nt-primary px-4">Khám phá sản phẩm ngay</a>
        </div>
    }
    else
    {
        <div class="row g-4">
            <div class="col-lg-8">
                <div class="bg-white rounded-4 border shadow-sm overflow-hidden">
                    <div class="table-responsive">
                        <table class="table align-middle mb-0">
                            <thead class="table-light small text-muted">
                                <tr>
                                    <th class="ps-4">Sản phẩm</th>
                                    <th>Đơn giá</th>
                                    <th style="width:150px">Số lượng</th>
                                    <th>Thành tiền</th>
                                    <th class="pe-3"></th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var item in Model.Cart.CartItems)
                                {
                                    <tr>
                                        <td class="ps-4 py-3">
                                            <div class="d-flex align-items-center gap-3">
                                                <img src="@(item.Product != null && !string.IsNullOrEmpty(item.Product.MainImageUrl) ? item.Product.MainImageUrl : "/images/samflora/b10-main.jpg")"
                                                     class="rounded-3 border" style="width:68px;height:68px;object-fit:cover" alt="@(item.Product != null ? item.Product.Name : "")" />
                                                <div>
                                                    <a href="@Url.Action("Details", "Product", new { id = item.ProductId })"
                                                       class="fw-bold text-dark text-decoration-none">@(item.Product != null ? item.Product.Name : "Sản phẩm hoa")</a>
                                                    <div class="text-muted small">Kho còn: @(item.Product != null ? item.Product.StockQuantity : 0)</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="fw-semibold">@AppHelpers.FormatVnd(item.UnitPrice)</td>
                                        <td>
                                            <form action="@Url.Action("UpdateCartItem", "Order")" method="post" class="d-flex align-items-center gap-1">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="cartItemId" value="@item.Id" />
                                                <input type="number" name="quantity" value="@item.Quantity" min="1" max="@(item.Product != null ? item.Product.StockQuantity : 99)"
                                                       class="form-control form-control-sm text-center" style="width:68px" />
                                                <button type="submit" class="btn btn-sm btn-light border" title="Cập nhật"><i class="bi bi-arrow-repeat"></i></button>
                                            </form>
                                        </td>
                                        <td class="fw-bold text-danger">@AppHelpers.FormatVnd(item.UnitPrice * item.Quantity)</td>
                                        <td class="pe-3 text-end">
                                            <form action="@Url.Action("RemoveCartItem", "Order")" method="post">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="cartItemId" value="@item.Id" />
                                                <button type="submit" class="btn btn-sm btn-outline-danger rounded-circle"><i class="bi bi-trash"></i></button>
                                            </form>
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="col-lg-4">
                <div class="bg-white p-4 rounded-4 border shadow-sm">
                    <h5 class="fw-bold mb-3">Tóm Tắt Đơn Hàng</h5>

                    <!-- Áp dụng mã giảm giá -->
                    <form action="@Url.Action("ApplyCoupon", "Order")" method="post" class="mb-3">
                        @Html.AntiForgeryToken()
                        <label class="form-label small fw-semibold">Mã giảm giá ưu đãi</label>
                        <div class="input-group">
                            <input type="text" name="couponCode" value="@Model.AppliedCoupon" class="form-control form-control-sm text-uppercase" placeholder="VD: CHAMFLOWERS15" />
                            <button type="submit" class="btn btn-sm btn-dark">Áp dụng</button>
                        </div>
                    </form>

                    @if (Model.AvailableCoupons.Any())
                    {
                        <div class="mb-3 d-flex flex-wrap gap-1">
                            @foreach (var c in Model.AvailableCoupons)
                            {
                                <form action="@Url.Action("ApplyCoupon", "Order")" method="post" class="d-inline">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="couponCode" value="@c.Code" />
                                    <button type="submit" class="badge bg-danger-subtle text-danger border-0 py-1 px-2" title="@c.Description">
                                        + Dùng @c.Code (-@c.DiscountPercent%)
                                    </button>
                                </form>
                            }
                        </div>
                    }

                    <hr />
                    <div class="d-flex justify-content-between small mb-2">
                        <span class="text-muted">Tạm tính:</span>
                        <span class="fw-semibold">@AppHelpers.FormatVnd(Model.SubTotal)</span>
                    </div>
                    <div class="d-flex justify-content-between small mb-2">
                        <span class="text-muted">Phí giao hoa dự kiến:</span>
                        <span class="fw-semibold">@AppHelpers.FormatVnd(Model.ShippingFee)</span>
                    </div>
                    @if (Model.DiscountAmount > 0)
                    {
                        <div class="d-flex justify-content-between small mb-2 text-success">
                            <span>Giảm giá (@Model.AppliedCoupon):</span>
                            <span class="fw-bold">-@AppHelpers.FormatVnd(Model.DiscountAmount)</span>
                        </div>
                    }
                    <hr />
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <span class="fw-bold">Tổng thanh toán:</span>
                        <span class="h4 fw-bold text-danger mb-0">@AppHelpers.FormatVnd(Model.GrandTotal)</span>
                    </div>

                    <a href="@Url.Action("Checkout", "Payment")" class="btn btn-nt-primary w-100 py-2">
                        Tiến hành Đặt hàng & Thanh toán <i class="bi bi-arrow-right ms-1"></i>
                    </a>
                    <a href="@Url.Action("Index", "Product")" class="btn btn-light border w-100 rounded-pill mt-2 btn-sm">
                        Tiếp tục chọn thêm hoa
                    </a>
                </div>
            </div>
        </div>
    }
</div>
```

### File: FlowerShop/Views\Payment\Checkout.cshtml
```html
@model FlowerShop.Models.CheckoutViewModel
@using FlowerShop.Services
@{
    ViewBag.Title = "Thanh toán đơn hàng - CHAM Flowers";
}

<div class="container py-4">
    <h1 class="h3 fw-bold mb-4"><i class="bi bi-credit-card-2-front-fill text-danger me-2"></i>Xác Nhận Đặt Hoa & Thanh Toán</h1>

    <form action="@Url.Action("ProcessPayment", "Payment")" method="post">
        @Html.AntiForgeryToken()
        <input type="hidden" name="CouponCode" value="@Model.CouponCode" />
        @Html.ValidationSummary(true, "", new { @class = "alert alert-danger rounded-4 small" })

        <div class="row g-4">
            <div class="col-lg-7">
                <!-- Bước 1: Thông tin người nhận -->
                <div class="bg-white p-4 rounded-4 border shadow-sm mb-4">
                    <h5 class="fw-bold mb-3">1. Thông Tin Người Nhận Hoa</h5>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Họ và tên người nhận *</label>
                            <input type="text" name="RecipientName" value="@Model.RecipientName" class="form-control" placeholder="Nhập họ tên người nhận" required />
                            @Html.ValidationMessageFor(m => m.RecipientName, "", new { @class = "text-danger small" })
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Số điện thoại liên hệ *</label>
                            <input type="text" name="RecipientPhone" value="@Model.RecipientPhone" class="form-control" placeholder="VD: 0912345678" required />
                            @Html.ValidationMessageFor(m => m.RecipientPhone, "", new { @class = "text-danger small" })
                        </div>
                        <div class="col-12">
                            <label class="form-label small fw-semibold">Địa chỉ giao hoa chi tiết *</label>
                            <input type="text" name="DeliveryAddress" value="@Model.DeliveryAddress" class="form-control" placeholder="Số nhà, đường, phường/xã, quận/huyện..." required />
                            @Html.ValidationMessageFor(m => m.DeliveryAddress, "", new { @class = "text-danger small" })
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Thời gian giao hoa mong muốn *</label>
                            <input type="datetime-local" name="DeliveryDate" value="@Model.DeliveryDate.ToString("yyyy-MM-ddTHH:mm")" class="form-control" required />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Đơn vị vận chuyển *</label>
                            <select name="ShippingCarrier" class="form-select" id="carrierSelect" onchange="updateShipDisplay()">
                                <option value="Giao Hoa Hỏa Tốc 2H (CHAM Express)" data-fee="30000" @Html.Raw(Model.ShippingCarrier == "Giao Hoa Hỏa Tốc 2H (CHAM Express)" ? "selected=\"selected\"" : "")>Giao Hoa Hỏa Tốc 2H (30.000 ₫)</option>
                                <option value="Giao Tiêu Chuẩn Trong Ngày" data-fee="20000" @Html.Raw(Model.ShippingCarrier == "Giao Tiêu Chuẩn Trong Ngày" ? "selected=\"selected\"" : "")>Giao Tiêu Chuẩn Trong Ngày (20.000 ₫)</option>
                                <option value="Nhận Trực Tiếp Tại Cửa Hàng CHAM Flowers" data-fee="0" @Html.Raw(Model.ShippingCarrier == "Nhận Trực Tiếp Tại Cửa Hàng CHAM Flowers" ? "selected=\"selected\"" : "")>Nhận Trực Tiếp Tại Cửa Hàng (Miễn phí)</option>
                            </select>
                        </div>
                        <div class="col-12">
                            <label class="form-label small fw-semibold">Nội dung thiệp chúc mừng / Ghi chú giao hàng</label>
                            <textarea name="Note" rows="2" class="form-control" placeholder="VD: Chúc mừng sinh nhật em yêu! Giao giúp mình trước 15h chiều...">@Model.Note</textarea>
                        </div>
                    </div>
                </div>

                <!-- Bước 2: Phương thức thanh toán (COD & VNPay) -->
                <div class="bg-white p-4 rounded-4 border shadow-sm">
                    <h5 class="fw-bold mb-3">2. Chọn Hình Thức Thanh Toán</h5>
                    <div class="d-grid gap-3">
                        <label class="p-3 rounded-4 border d-flex align-items-center gap-3" style="cursor:pointer">
                            <input type="radio" name="PaymentMethod" value="COD" class="form-check-input mt-0" @Html.Raw(Model.PaymentMethod == "VNPay" ? "" : "checked=\"checked\"") />
                            <span class="fs-3 text-success"><i class="bi bi-cash-coin"></i></span>
                            <div>
                                <div class="fw-bold">Thanh toán tiền mặt khi nhận hoa (COD)</div>
                                <div class="text-muted small">Kiểm tra mẫu hoa tươi thực tế khi nhận rồi mới thanh toán.</div>
                            </div>
                        </label>

                        <label class="p-3 rounded-4 border d-flex align-items-center gap-3" style="cursor:pointer">
                            <input type="radio" name="PaymentMethod" value="VNPay" class="form-check-input mt-0" @Html.Raw(Model.PaymentMethod == "VNPay" ? "checked=\"checked\"" : "") />
                            <span class="fs-3 text-primary"><i class="bi bi-qr-code-scan"></i></span>
                            <div>
                                <div class="fw-bold">Thanh toán trực tuyến qua cổng VNPay <span class="badge bg-primary ms-1">Khuyên dùng</span></div>
                                <div class="text-muted small">Hỗ trợ Thẻ ATM nội địa (NCB, Vietcombank...), VNPay QR, Thẻ quốc tế bảo mật HMAC-SHA512.</div>
                            </div>
                        </label>
                    </div>
                </div>
            </div>

            <!-- Kiểm tra giỏ hàng lần cuối -->
            <div class="col-lg-5">
                <div class="bg-white p-4 rounded-4 border shadow-sm sticky-top" style="top:88px">
                    <h5 class="fw-bold mb-3">Đơn Hàng Của Bạn (@Model.Items.Sum(i => i.Quantity) sản phẩm)</h5>
                    <div class="d-grid gap-3 mb-3">
                        @foreach (var ci in Model.Items)
                        {
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="d-flex align-items-center gap-2">
                                    <img src="@(ci.Product != null && !string.IsNullOrEmpty(ci.Product.MainImageUrl) ? ci.Product.MainImageUrl : "/images/samflora/b10-main.jpg")"
                                         class="rounded-3 border" style="width:50px;height:50px;object-fit:cover" alt="" />
                                    <div class="small">
                                        <div class="fw-semibold">@(ci.Product != null ? ci.Product.Name : "Sản phẩm")</div>
                                        <div class="text-muted">SL: @ci.Quantity</div>
                                    </div>
                                </div>
                                <div class="fw-semibold small">@AppHelpers.FormatVnd(ci.UnitPrice * ci.Quantity)</div>
                            </div>
                        }
                    </div>
                    <hr />
                    <div class="d-flex justify-content-between small mb-2">
                        <span class="text-muted">Tạm tính:</span>
                        <span>@AppHelpers.FormatVnd(Model.SubTotal)</span>
                    </div>
                    <div class="d-flex justify-content-between small mb-2">
                        <span class="text-muted">Phí vận chuyển:</span>
                        <span id="shipFeeText">@AppHelpers.FormatVnd(Model.ShippingFee)</span>
                    </div>
                    @if (Model.DiscountAmount > 0)
                    {
                        <div class="d-flex justify-content-between small mb-2 text-success">
                            <span>Mã giảm giá (@Model.CouponCode):</span>
                            <span>-@AppHelpers.FormatVnd(Model.DiscountAmount)</span>
                        </div>
                    }
                    <hr />
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <span class="fw-bold">Tổng thanh toán:</span>
                        <span id="grandTotalText" class="h4 fw-bold text-danger mb-0">@AppHelpers.FormatVnd(Model.GrandTotal)</span>
                    </div>

                    <button type="submit" class="btn btn-nt-primary w-100 py-2 fs-6">
                        <i class="bi bi-lock-fill me-1"></i> Xác nhận Đặt hàng
                    </button>
                </div>
            </div>
        </div>
    </form>
</div>

@section Scripts {
    <script>
        var baseSub = @((long)Model.SubTotal);
        var baseDisc = @((long)Model.DiscountAmount);
        function updateShipDisplay() {
            var sel = document.getElementById('carrierSelect');
            var fee = parseInt(sel.options[sel.selectedIndex].getAttribute('data-fee') || '30000');
            var total = Math.max(0, baseSub + fee - baseDisc);
            document.getElementById('shipFeeText').textContent = fee.toLocaleString('vi-VN') + ' ₫';
            document.getElementById('grandTotalText').textContent = total.toLocaleString('vi-VN') + ' ₫';
        }
    </script>
}
```

### File: FlowerShop/Views\Account\Login.cshtml
```html
@model FlowerShop.Models.LoginViewModel
@{
    ViewData["Title"] = "Đăng nhập hệ thống";
}

<div class="container py-5" style="max-width:500px">
    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm">
        <div class="text-center mb-4">
            <img src="~/images/logo-cham-icon.svg" style="width:68px;height:68px" class="mb-2 shadow-sm rounded-circle" alt="CHAM Flowers" />
            <h2 class="h3 fw-bold mb-1">Đăng Nhập CHAM Flowers</h2>
            <p class="text-muted small">Đăng nhập để đặt hoa nhanh, lưu địa chỉ và tích điểm VIP</p>
        </div>

        <form action="@Url.Action("Login", "Account")" method="post" id="loginForm">
            @Html.AntiForgeryToken()
            <input type="hidden" name="ReturnUrl" value="@Model.ReturnUrl" />
            @if (!ViewData.ModelState.IsValid)
            {
                @Html.ValidationSummary(true, "", new { @class = "alert alert-danger small rounded-3 py-2" })
            }

            <div class="mb-3">
                <label class="form-label fw-semibold small">Email đăng nhập</label>
                <input type="email" name="Email" id="loginEmail" value="@Model.Email" class="form-control rounded-3" placeholder="example@chamflowers.vn" required />
                @Html.ValidationMessageFor(m => m.Email, "", new { @class = "text-danger small" })
            </div>

            <div class="mb-3">
                <div class="d-flex justify-content-between">
                    <label class="form-label fw-semibold small">Mật khẩu</label>
                    <a href="@Url.Action("ForgotPassword", "Account")" class="small text-danger text-decoration-none">Quên mật khẩu?</a>
                </div>
                <input type="password" name="Password" id="loginPassword" class="form-control rounded-3" placeholder="••••••••" required />
                @Html.ValidationMessageFor(m => m.Password, "", new { @class = "text-danger small" })
            </div>

            <div class="form-check mb-4">
                <input type="checkbox" name="RememberMe" id="RememberMe" value="true" class="form-check-input" @Html.Raw(Model.RememberMe ? "checked=\"checked\"" : "") />
                <input type="hidden" name="RememberMe" value="false" />
                <label for="RememberMe" class="form-check-label small">Ghi nhớ phiên đăng nhập</label>
            </div>

            <button type="submit" class="btn btn-nt-primary w-100 py-2 mb-3">Đăng nhập ngay</button>
        </form>

        <div class="p-3 rounded-4 bg-light border mb-3">
            <div class="small fw-bold text-secondary mb-2"><i class="bi bi-lightning-charge-fill text-warning"></i> Đăng nhập nhanh tài khoản mẫu:</div>
            <div class="d-flex gap-2">
                <button type="button" class="btn btn-sm btn-dark rounded-pill flex-grow-1"
                        onclick="document.getElementById('loginEmail').value='admin@chamflowers.vn'; document.getElementById('loginPassword').value='Admin@123'; document.getElementById('loginForm').submit();">
                    Admin Chủ Shop
                </button>
                <button type="button" class="btn btn-sm btn-outline-danger rounded-pill flex-grow-1"
                        onclick="document.getElementById('loginEmail').value='khachvip@chamflowers.vn'; document.getElementById('loginPassword').value='Khach@123'; document.getElementById('loginForm').submit();">
                    Khách hàng VIP
                </button>
            </div>
        </div>

        <div class="text-center small text-muted">
            Chưa có tài khoản? <a href="@Url.Action("Register", "Account")" class="text-danger fw-bold text-decoration-none">Đăng ký thành viên mới</a>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Account\Register.cshtml
```html
@model FlowerShop.Models.RegisterViewModel
@{
    ViewData["Title"] = "Đăng ký tài khoản";
}

<div class="container py-5" style="max-width:600px">
    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm">
        <div class="text-center mb-4">
            <h2 class="h3 fw-bold mb-1">Đăng Ký Thành Viên CHAM Flowers</h2>
            <p class="text-muted small">Nhận ưu đãi hậu mãi sinh nhật và nâng hạng VIP tự động</p>
        </div>

        <form action="@Url.Action("Register", "Account")" method="post">
            @Html.AntiForgeryToken()
            @if (!ViewData.ModelState.IsValid)
            {
                @Html.ValidationSummary(true, "", new { @class = "alert alert-danger small rounded-3 py-2" })
            }

            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label fw-semibold small">Họ và tên *</label>
                    <input type="text" name="FullName" value="@Model.FullName" class="form-control rounded-3" placeholder="Nguyễn Văn A" required />
                    @Html.ValidationMessageFor(m => m.FullName, "", new { @class = "text-danger small" })
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold small">Số điện thoại *</label>
                    <input type="text" name="Phone" value="@Model.Phone" class="form-control rounded-3" placeholder="0909123456" required />
                    @Html.ValidationMessageFor(m => m.Phone, "", new { @class = "text-danger small" })
                </div>
                <div class="col-md-7">
                    <label class="form-label fw-semibold small">Email đăng nhập *</label>
                    <input type="email" name="Email" value="@Model.Email" class="form-control rounded-3" placeholder="email@domain.com" required />
                    @Html.ValidationMessageFor(m => m.Email, "", new { @class = "text-danger small" })
                </div>
                <div class="col-md-5">
                    <label class="form-label fw-semibold small">Ngày sinh (Nhận quà SN)</label>
                    <input type="date" name="BirthDate" value="@(Model.BirthDate.HasValue ? Model.BirthDate.Value.ToString("yyyy-MM-dd") : "")" class="form-control rounded-3" />
                </div>
                <div class="col-12">
                    <label class="form-label fw-semibold small">Địa chỉ giao nhận mặc định *</label>
                    <input type="text" name="Address" value="@Model.Address" class="form-control rounded-3" placeholder="Số nhà, tên đường, phường/xã, quận/huyện..." required />
                    @Html.ValidationMessageFor(m => m.Address, "", new { @class = "text-danger small" })
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold small">Mật khẩu *</label>
                    <input type="password" name="Password" class="form-control rounded-3" placeholder="Tối thiểu 6 ký tự" required />
                    @Html.ValidationMessageFor(m => m.Password, "", new { @class = "text-danger small" })
                </div>
                <div class="col-md-6">
                    <label class="form-label fw-semibold small">Xác nhận mật khẩu *</label>
                    <input type="password" name="ConfirmPassword" class="form-control rounded-3" placeholder="Nhập lại mật khẩu" required />
                    @Html.ValidationMessageFor(m => m.ConfirmPassword, "", new { @class = "text-danger small" })
                </div>
            </div>

            <button type="submit" class="btn btn-nt-primary w-100 py-2 mt-4 mb-3">Tạo tài khoản ngay</button>
            <div class="text-center small text-muted">
                Đã có tài khoản? <a href="@Url.Action("Login", "Account")" class="text-danger fw-bold text-decoration-none">Đăng nhập</a>
            </div>
        </form>
    </div>
</div>
```

### File: FlowerShop/Views\Account\Profile.cshtml
```html
@model FlowerShop.Models.ProfileViewModel
@{
    ViewData["Title"] = "Quản lý tài khoản cá nhân";
    var recentOrders = ViewBag.RecentOrders as List<FlowerShop.Models.Order> ?? new List<FlowerShop.Models.Order>();
}

<div class="container py-4">
    <div class="row g-4">
        <div class="col-lg-4">
            <div class="bg-white p-4 rounded-4 border shadow-sm text-center mb-4">
                <div class="rounded-circle bg-danger-subtle text-danger d-inline-flex align-items-center justify-content-center mb-3" style="width:76px;height:76px">
                    <i class="bi bi-person-heart fs-1"></i>
                </div>
                <h5 class="fw-bold mb-1">@Model.FullName</h5>
                <div class="text-muted small mb-2">@Model.Email</div>
                @if (Model.IsVip)
                {
                    <span class="badge bg-warning text-dark px-3 py-2 rounded-pill"><i class="bi bi-gem me-1"></i>Khách hàng VIP (Áp mã VIP20)</span>
                }
                else
                {
                    <span class="badge bg-secondary-subtle text-secondary px-3 py-2 rounded-pill">Thành viên Thân thiết</span>
                }
                <hr />
                <div class="d-flex justify-content-between small">
                    <span class="text-muted">Tổng chi tiêu tích lũy:</span>
                    <strong class="text-danger">@AppHelpers.FormatVnd(Model.TotalSpent)</strong>
                </div>
                @if (!Model.IsVip)
                {
                    <div class="small text-muted mt-2" style="font-size:0.76rem">
                        Chi tiêu từ 2.000.000 ₫ tự động nâng hạng VIP!
                    </div>
                }
            </div>

            <!-- Đổi mật khẩu -->
            <div class="bg-white p-4 rounded-4 border shadow-sm">
                <h6 class="fw-bold mb-3"><i class="bi bi-key-fill text-danger me-2"></i>Đổi mật khẩu bảo mật</h6>
                <form action="@Url.Action("ChangePassword", "Account")" method="post">
                    @Html.AntiForgeryToken()
                    <div class="mb-2">
                        <input type="password" name="CurrentPassword" class="form-control form-control-sm" placeholder="Mật khẩu hiện tại" required />
                    </div>
                    <div class="mb-2">
                        <input type="password" name="NewPassword" class="form-control form-control-sm" placeholder="Mật khẩu mới (>= 6 ký tự)" required minlength="6" />
                    </div>
                    <div class="mb-3">
                        <input type="password" name="ConfirmNewPassword" class="form-control form-control-sm" placeholder="Xác nhận mật khẩu mới" required />
                    </div>
                    <button type="submit" class="btn btn-sm btn-nt-outline w-100">Cập nhật mật khẩu</button>
                </form>
            </div>
        </div>

        <div class="col-lg-8">
            <div class="bg-white p-4 rounded-4 border shadow-sm mb-4">
                <h5 class="fw-bold mb-3">Thông Tin Cá Nhân & Địa Chỉ Giao Hoa Mặc Định</h5>
                <form action="@Url.Action("Profile", "Account")" method="post">
                    @Html.AntiForgeryToken()
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Họ và tên</label>
                            <input type="text" name="FullName" value="@Model.FullName" class="form-control" required />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Email (Cố định)</label>
                            <input value="@Model.Email" class="form-control bg-light" readonly />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Số điện thoại nhận hàng</label>
                            <input type="text" name="Phone" value="@Model.Phone" class="form-control" required />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label small fw-semibold">Ngày sinh</label>
                            <input type="date" name="BirthDate" value="@(Model.BirthDate.HasValue ? Model.BirthDate.Value.ToString("yyyy-MM-dd") : "")" class="form-control" />
                        </div>
                        <div class="col-12">
                            <label class="form-label small fw-semibold">Địa chỉ nhận hàng mặc định</label>
                            <input type="text" name="Address" value="@Model.Address" class="form-control" required />
                        </div>
                    </div>
                    <button type="submit" class="btn btn-nt-primary mt-3">Lưu thay đổi</button>
                </form>
            </div>

            <div class="bg-white p-4 rounded-4 border shadow-sm">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h6 class="fw-bold mb-0">Đơn Hàng Gần Đây</h6>
                    <a href="@Url.Action("MyOrders", "Order")" class="small text-danger text-decoration-none fw-semibold">Xem tất cả →</a>
                </div>
                @if (!recentOrders.Any())
                {
                    <p class="text-muted small mb-0">Bạn chưa có đơn đặt hoa nào.</p>
                }
                else
                {
                    <div class="table-responsive">
                        <table class="table table-sm align-middle mb-0">
                            <thead>
                                <tr class="text-muted small">
                                    <th>Mã đơn</th>
                                    <th>Ngày đặt</th>
                                    <th>Tổng tiền</th>
                                    <th>Trạng thái</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var o in recentOrders)
                                {
                                    <tr>
                                        <td class="fw-bold">#@o.Id</td>
                                        <td class="small">@o.CreatedAt.ToString("dd/MM/yyyy HH:mm")</td>
                                        <td class="fw-semibold text-danger">@AppHelpers.FormatVnd(o.Total)</td>
                                        <td><span class="badge bg-light text-dark border">@o.Status</span></td>
                                        <td class="text-end">
                                            <a href="@Url.Action("Details", "Order", new { id = o.Id })" class="btn btn-sm btn-light border">Chi tiết</a>
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>
                    </div>
                }
            </div>
        </div>
    </div>
</div>
```

### File: FlowerShop/Content\site.css
```css
:root {
    --nt-primary: #be123c;
    --nt-primary-dark: #881337;
    --nt-primary-light: #ffe4e6;
    --nt-accent: #ec4899;
    --nt-gold: #d97706;
    --nt-sage: #15803d;
    --nt-bg: #fffbfc;
    --nt-surface: #ffffff;
    --nt-text: #1f2937;
    --nt-muted: #6b7280;
    --nt-border: #f3e8ee;
    --nt-radius: 16px;
}

body {
    font-family: 'Plus Jakarta Sans', 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
    background-color: var(--nt-bg);
    color: var(--nt-text);
    overflow-x: hidden;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
}

/* Mở rộng không gian hiển thị trên màn hình rộng - Không để trống 2 bên */
@media (min-width: 1200px) {
    .container, .container-lg, .container-xl, .container-xxl {
        max-width: 95% !important;
        padding-left: 2rem !important;
        padding-right: 2rem !important;
    }
}

@media (min-width: 1400px) {
    .container, .container-lg, .container-xl, .container-xxl {
        max-width: 94% !important;
        padding-left: 2.5rem !important;
        padding-right: 2.5rem !important;
    }
}

@media (min-width: 1600px) {
    .container, .container-lg, .container-xl, .container-xxl {
        max-width: 1680px !important;
        padding-left: 3rem !important;
        padding-right: 3rem !important;
    }
}

@media (min-width: 1900px) {
    .container, .container-lg, .container-xl, .container-xxl {
        max-width: 1840px !important;
        padding-left: 3.5rem !important;
        padding-right: 3.5rem !important;
    }
}

h1, h2, h3, .serif-heading {
    font-family: 'Playfair Display', 'Times New Roman', serif;
}

.nt-topbar {
    background: linear-gradient(90deg, #881337 0%, #be123c 50%, #9d174d 100%);
    color: #fff;
    font-size: 0.85rem;
    padding: 0.45rem 0;
}

.nt-navbar {
    background: rgba(255, 255, 255, 0.96);
    backdrop-filter: blur(12px);
    border-bottom: 1px solid var(--nt-border);
    box-shadow: 0 4px 20px rgba(190, 18, 60, 0.05);
}

.nt-brand {
    font-family: 'Playfair Display', 'Times New Roman', serif;
    font-weight: 700;
    font-size: 1.65rem;
    color: var(--nt-primary) !important;
    letter-spacing: -0.5px;
}

.cham-logo-nav {
    height: 46px;
    width: auto;
    display: block;
    transition: transform 0.2s ease;
}

.nt-brand:hover .cham-logo-nav {
    transform: scale(1.02);
}

.cham-logo-footer {
    height: 54px;
    width: auto;
    display: block;
}

.nt-nav-link {
    font-size: 0.92rem;
    font-weight: 600;
    white-space: nowrap;
    color: #374151 !important;
    padding: 0.45rem 0.82rem !important;
    border-radius: 99px;
    transition: all 0.2s ease;
}

.nt-nav-link:hover, .nt-nav-link.active {
    color: var(--nt-primary) !important;
    background-color: var(--nt-primary-light);
}

/* Boutique Navbar Search Pill & Actions */
.nt-search-box {
    display: flex;
    align-items: center;
    height: 40px;
    width: 225px;
    padding: 4px 5px 4px 15px;
    background-color: #fdf7f9;
    border: 1.5px solid #f0d6df;
    border-radius: 999px;
    transition: border-color 0.2s ease, box-shadow 0.2s ease, background-color 0.2s ease, width 0.22s ease;
}

.nt-search-box:hover {
    background-color: #ffffff;
    border-color: #e8a4b8;
}

.nt-search-box:focus-within {
    background-color: #ffffff;
    border-color: var(--nt-primary);
    box-shadow: 0 4px 16px rgba(190, 18, 60, 0.12), 0 0 0 3px rgba(190, 18, 60, 0.08);
    width: 250px;
}

.nt-search-input {
    flex: 1 1 auto;
    min-width: 0;
    border: none !important;
    outline: none !important;
    box-shadow: none !important;
    background: transparent !important;
    font-size: 0.875rem;
    font-weight: 500;
    color: var(--nt-text);
    padding: 0 8px 0 0;
}

.nt-search-input::placeholder {
    color: #9ca3af;
    font-weight: 400;
}

.nt-search-btn {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    border: none;
    background: linear-gradient(135deg, #e11d48 0%, #be123c 100%);
    color: #ffffff;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    font-size: 0.8rem;
    cursor: pointer;
    box-shadow: 0 2px 6px rgba(190, 18, 60, 0.24);
    transition: transform 0.18s ease, box-shadow 0.18s ease;
}

.nt-search-btn:hover {
    transform: scale(1.06);
    box-shadow: 0 4px 10px rgba(190, 18, 60, 0.35);
}

.nt-cart-btn {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background-color: #fdf7f9;
    border: 1.5px solid #f0d6df;
    color: var(--nt-primary);
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: 1.1rem;
    text-decoration: none;
    transition: all 0.2s ease;
}

.nt-cart-btn:hover {
    background-color: var(--nt-primary-light);
    border-color: var(--nt-primary);
    color: var(--nt-primary-dark);
    transform: translateY(-1px);
}

.nt-nav-action-btn {
    height: 40px;
    display: inline-flex;
    align-items: center;
}

@media (max-width: 1199.98px) {
    .nt-search-box,
    .nt-search-box:focus-within {
        width: 100%;
    }
}

.btn-nt-primary {
    background: linear-gradient(135deg, #e11d48 0%, #be123c 100%);
    color: #fff;
    border: none;
    border-radius: 99px;
    padding: 0.6rem 1.45rem;
    font-weight: 600;
    transition: transform 0.18s ease, box-shadow 0.18s ease;
}

.btn-nt-primary:hover {
    color: #fff;
    transform: translateY(-1px);
    box-shadow: 0 8px 20px rgba(190, 18, 60, 0.28);
}

.btn-nt-outline {
    border: 1.5px solid var(--nt-primary);
    color: var(--nt-primary);
    background: transparent;
    border-radius: 99px;
    padding: 0.55rem 1.35rem;
    font-weight: 600;
    transition: all 0.2s ease;
}

.btn-nt-outline:hover {
    background: var(--nt-primary);
    color: #fff;
}

/* Hero Banner */
.hero-slide {
    position: relative;
    border-radius: 24px;
    overflow: hidden;
    min-height: 480px;
    background-size: cover;
    background-position: center;
    display: flex;
    align-items: center;
}

.hero-slide::before {
    content: "";
    position: absolute;
    inset: 0;
    background: linear-gradient(90deg, rgba(36, 6, 17, 0.88) 0%, rgba(136, 19, 55, 0.65) 52%, rgba(15, 23, 42, 0.22) 100%);
    z-index: 1;
}

.hero-overlay {
    position: relative;
    padding: 3.5rem 4rem;
    max-width: 780px;
    color: #fff;
    z-index: 2;
}

/* Product Cards */
.flower-card {
    background: #fff;
    border-radius: var(--nt-radius);
    border: 1px solid var(--nt-border);
    overflow: hidden;
    transition: transform 0.25s ease, box-shadow 0.25s ease;
    height: 100%;
    display: flex;
    flex-direction: column;
}

.flower-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 14px 32px rgba(136, 19, 55, 0.10);
}

.flower-img-wrap {
    position: relative;
    padding-top: 112%;
    background: #fff5f7;
    overflow: hidden;
}

.flower-img-wrap img {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
    object-position: center 35%;
    transition: transform 0.4s ease;
}

.flower-card:hover .flower-img-wrap img {
    transform: scale(1.05);
}

.badge-discount {
    position: absolute;
    top: 12px;
    left: 12px;
    background: #e11d48;
    color: #fff;
    font-weight: 700;
    font-size: 0.78rem;
    padding: 0.3rem 0.65rem;
    border-radius: 99px;
    z-index: 2;
}

.badge-hot {
    position: absolute;
    top: 12px;
    right: 12px;
    background: #fef3c7;
    color: #b45309;
    font-weight: 700;
    font-size: 0.75rem;
    padding: 0.28rem 0.6rem;
    border-radius: 99px;
    z-index: 2;
}

.flower-price {
    color: var(--nt-primary);
    font-weight: 800;
    font-size: 1.15rem;
}

.flower-old-price {
    color: #9ca3af;
    text-decoration: line-through;
    font-size: 0.88rem;
    margin-left: 0.4rem;
}

/* Category Pills */
.flower-cat-card {
    background: #fff;
    border: 1px solid var(--nt-border);
    border-radius: 18px;
    padding: 1.1rem;
    text-align: center;
    text-decoration: none;
    color: var(--nt-text);
    transition: all 0.22s ease;
    display: block;
    height: 100%;
}

.flower-cat-card:hover {
    border-color: var(--nt-primary);
    transform: translateY(-3px);
    box-shadow: 0 10px 24px rgba(190, 18, 60, 0.08);
    color: var(--nt-primary);
}

.flower-cat-card img {
    width: 78px;
    height: 78px;
    border-radius: 50%;
    object-fit: cover;
    margin-bottom: 0.65rem;
    border: 3px solid var(--nt-primary-light);
}

/* Countdown Box */
.countdown-box {
    display: inline-flex;
    gap: 0.6rem;
}

.countdown-unit {
    background: #881337;
    color: #fff;
    border-radius: 12px;
    min-width: 58px;
    padding: 0.45rem 0.6rem;
    text-align: center;
}

.countdown-unit strong {
    display: block;
    font-size: 1.25rem;
    line-height: 1.1;
}

.countdown-unit small {
    font-size: 0.7rem;
    opacity: 0.85;
}

/* Floating Chat Widget */
.nt-chat-launcher {
    position: fixed;
    bottom: 24px;
    right: 24px;
    width: 60px;
    height: 60px;
    border-radius: 50%;
    background: linear-gradient(135deg, #e11d48, #9d174d);
    color: #fff;
    border: none;
    box-shadow: 0 10px 28px rgba(190, 18, 60, 0.4);
    z-index: 1055;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.5rem;
    transition: transform 0.2s ease;
}

.nt-chat-launcher:hover {
    transform: scale(1.07);
}

.nt-chat-panel {
    position: fixed;
    bottom: 96px;
    right: 24px;
    width: 365px;
    max-width: calc(100vw - 32px);
    height: 485px;
    background: #fff;
    border-radius: 20px;
    box-shadow: 0 18px 48px rgba(15, 23, 42, 0.22);
    border: 1px solid var(--nt-border);
    z-index: 1055;
    display: none;
    flex-direction: column;
    overflow: hidden;
}

.nt-chat-panel.open {
    display: flex;
}

.nt-chat-header {
    background: linear-gradient(135deg, #9f1239, #e11d48);
    color: #fff;
    padding: 0.9rem 1.1rem;
}

.nt-chat-messages {
    flex: 1;
    overflow-y: auto;
    padding: 1rem;
    background: #fff9fb;
    display: flex;
    flex-direction: column;
    gap: 0.65rem;
}

.chat-bubble {
    max-width: 82%;
    padding: 0.6rem 0.9rem;
    border-radius: 16px;
    font-size: 0.9rem;
    line-height: 1.45;
}

.chat-bubble.customer {
    align-self: flex-end;
    background: var(--nt-primary);
    color: #fff;
    border-bottom-right-radius: 4px;
}

.chat-bubble.admin {
    align-self: flex-start;
    background: #fff;
    color: var(--nt-text);
    border: 1px solid #f3d6df;
    border-bottom-left-radius: 4px;
}

.chat-bubble.system {
    align-self: center;
    background: #fef3c7;
    color: #92400e;
    font-size: 0.78rem;
    max-width: 95%;
    text-align: center;
}

/* ==========================================================================
   CHAM Flowers Executive Admin CMS Design System
   ========================================================================== */
.admin-wrapper {
    display: flex;
    min-height: 100vh;
    background-color: #f4f5f9;
    background-image: radial-gradient(ellipse 80% 320px at 50% 0%, rgba(225, 29, 72, 0.055), transparent 70%);
    color: #1e293b;
}

/* Minimalist Toned-Down Dark Slate Sidebar */
.admin-sidebar {
    width: 278px;
    background: #18181b;
    color: #f4f4f5;
    flex-shrink: 0;
    display: flex;
    flex-direction: column;
    position: sticky;
    top: 0;
    height: 100vh;
    border-right: 1px solid #27272a;
    z-index: 1035;
}

.admin-brand-box {
    padding: 1.15rem 1.25rem 1.05rem;
    border-bottom: 1px solid rgba(255, 255, 255, 0.07);
}

.admin-brand-emblem {
    width: 42px;
    height: 42px;
    border-radius: 12px;
    padding: 2px;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.12);
    flex-shrink: 0;
}

.admin-brand-title {
    font-family: 'Playfair Display', Georgia, serif;
    font-size: 1.14rem;
    font-weight: 700;
    letter-spacing: 0.2px;
    color: #fafafa;
    line-height: 1.2;
}

.admin-brand-badge {
    display: inline-flex;
    align-items: center;
    gap: 4px;
    font-size: 0.66rem;
    font-weight: 600;
    letter-spacing: 0.08em;
    text-transform: uppercase;
    color: #a1a1aa;
    margin-top: 2px;
}

.admin-nav-scroll {
    padding: 0.65rem 0.8rem 0.75rem;
    flex-grow: 1;
    overflow-y: auto;
}

.admin-nav-scroll::-webkit-scrollbar {
    width: 4px;
}

.admin-nav-scroll::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.12);
    border-radius: 99px;
}

.admin-nav-section {
    padding: 0.72rem 0.65rem 0.3rem;
    font-size: 0.65rem;
    font-weight: 700;
    letter-spacing: 0.09em;
    text-transform: uppercase;
    color: #71717a;
}

.admin-sidebar .nav-link {
    color: #a1a1aa;
    padding: 0.5rem 0.7rem;
    border-radius: 11px;
    margin: 0.12rem 0;
    font-size: 0.86rem;
    font-weight: 500;
    display: flex;
    align-items: center;
    gap: 0.65rem;
    position: relative;
    white-space: nowrap;
    border: 1px solid transparent;
    transition: all 0.18s ease;
}

.admin-nav-icon {
    width: 30px;
    height: 30px;
    border-radius: 8px;
    background: rgba(255, 255, 255, 0.04);
    color: #a1a1aa;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: 0.92rem;
    flex-shrink: 0;
    transition: all 0.18s ease;
}

.admin-sidebar .nav-link:hover {
    background: rgba(255, 255, 255, 0.05);
    color: #f4f4f5;
}

.admin-sidebar .nav-link:hover .admin-nav-icon {
    background: rgba(255, 255, 255, 0.08);
    color: #f4f4f5;
}

.admin-sidebar .nav-link.active {
    background: #27272a;
    border-color: rgba(255, 255, 255, 0.10);
    color: #ffffff;
    font-weight: 600;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.admin-sidebar .nav-link.active::before {
    content: "";
    position: absolute;
    left: -1px;
    top: 22%;
    bottom: 22%;
    width: 3px;
    border-radius: 0 4px 4px 0;
    background: #e4e4e7;
}

.admin-sidebar .nav-link.active .admin-nav-icon {
    background: rgba(255, 255, 255, 0.10);
    color: #ffffff;
}

.admin-nav-badge {
    margin-left: auto;
    font-size: 0.69rem;
    font-weight: 600;
    padding: 0.12rem 0.5rem;
    border-radius: 999px;
    background: #27272a;
    color: #d4d4d8;
    border: 1px solid #3f3f46;
}

.admin-sidebar .nav-link.active .admin-nav-badge {
    background: #3f3f46;
    color: #ffffff;
    border-color: #52525b;
}

.admin-sidebar-footer {
    padding: 0.85rem 0.9rem;
    border-top: 1px solid rgba(255, 255, 255, 0.07);
    background: #141417;
}

.admin-profile-mini {
    display: flex;
    align-items: center;
    gap: 0.7rem;
    padding: 0.65rem 0.78rem;
    border-radius: 13px;
    background: linear-gradient(135deg, rgba(225, 29, 72, 0.20) 0%, rgba(136, 19, 55, 0.24) 55%, rgba(245, 158, 11, 0.12) 100%);
    border: 1px solid rgba(251, 113, 133, 0.34);
    box-shadow: 0 6px 18px rgba(225, 29, 72, 0.14);
    margin-bottom: 0.65rem;
}

.admin-avatar-ring {
    width: 38px;
    height: 38px;
    border-radius: 50%;
    background: linear-gradient(135deg, #fb7185 0%, #e11d48 55%, #be123c 100%);
    color: #ffffff;
    border: 1.5px solid rgba(253, 230, 138, 0.65);
    font-weight: 700;
    font-size: 0.84rem;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    box-shadow: 0 4px 12px rgba(225, 29, 72, 0.42);
}

.admin-online-dot {
    width: 7px;
    height: 7px;
    border-radius: 50%;
    background: #22c55e;
    box-shadow: 0 0 8px rgba(34, 197, 94, 0.85);
    display: inline-block;
    flex-shrink: 0;
}

.admin-sidebar-btn {
    background: #202024;
    border: 1px solid rgba(255, 255, 255, 0.12);
    color: #e4e4e7;
    font-size: 0.8rem;
    font-weight: 500;
    transition: all 0.18s ease;
}

.admin-sidebar-btn:hover {
    background: #2c2c31;
    border-color: rgba(255, 255, 255, 0.22);
    color: #ffffff;
}

.admin-sidebar-btn-logout:hover {
    background: rgba(225, 29, 72, 0.16);
    border-color: rgba(244, 63, 94, 0.4);
    color: #fda4af;
}

/* Main Content & Glassmorphic Topbar */
.admin-main {
    flex: 1;
    min-width: 0;
    display: flex;
    flex-direction: column;
}

.admin-topbar {
    background: rgba(255, 255, 255, 0.88);
    backdrop-filter: blur(14px);
    -webkit-backdrop-filter: blur(14px);
    border-bottom: 1px solid rgba(226, 232, 240, 0.85);
    padding: 0.85rem 1.75rem;
    position: sticky;
    top: 0;
    z-index: 1025;
}

.admin-breadcrumb {
    font-size: 0.73rem;
    font-weight: 600;
    letter-spacing: 0.03em;
    color: #94a3b8;
    margin-bottom: 0.1rem;
}

.admin-page-title {
    font-size: 1.16rem;
    font-weight: 700;
    color: #0f172a;
    letter-spacing: -0.01em;
    margin: 0;
}

.admin-quick-search {
    display: flex;
    align-items: center;
    height: 38px;
    width: 240px;
    padding: 0 12px;
    background: #f8fafc;
    border: 1.5px solid #e2e8f0;
    border-radius: 999px;
    transition: all 0.2s ease;
}

.admin-quick-search:focus-within {
    background: #fff;
    border-color: var(--nt-primary);
    box-shadow: 0 0 0 3px rgba(225, 29, 72, 0.1);
    width: 270px;
}

.admin-quick-search input {
    border: none !important;
    outline: none !important;
    box-shadow: none !important;
    background: transparent !important;
    font-size: 0.84rem;
    width: 100%;
    color: #1e293b;
}

.admin-icon-pill {
    width: 38px;
    height: 38px;
    border-radius: 50%;
    background: #f8fafc;
    border: 1.5px solid #e2e8f0;
    color: #475569;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    position: relative;
    transition: all 0.2s ease;
}

.admin-icon-pill:hover {
    background: #fff1f2;
    border-color: #fda4af;
    color: var(--nt-primary);
}

.admin-date-chip {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    height: 38px;
    padding: 0 14px;
    border-radius: 999px;
    background: #f8fafc;
    border: 1.5px solid #e2e8f0;
    font-size: 0.82rem;
    font-weight: 600;
    color: #475569;
}

.admin-content-area {
    padding: 1.65rem 1.85rem 2.4rem;
    flex-grow: 1;
}

@media (max-width: 991.98px) {
    .admin-topbar {
        padding: 0.75rem 1rem;
    }
    .admin-content-area {
        padding: 1.15rem 1rem 2rem;
    }
}

/* Elevated Admin Cards & Welcome Banner */
.admin-card {
    background: #ffffff;
    border-radius: 20px;
    border: 1px solid rgba(226, 232, 240, 0.9);
    box-shadow: 0 4px 22px rgba(15, 23, 42, 0.035), 0 1px 2px rgba(15, 23, 42, 0.02);
    transition: box-shadow 0.22s ease;
}

.admin-card:hover {
    box-shadow: 0 10px 28px rgba(15, 23, 42, 0.055);
}

.admin-hero-banner {
    background: linear-gradient(125deg, #2a0a18 0%, #881337 55%, #be123c 100%);
    border-radius: 22px;
    padding: 1.55rem 1.85rem;
    color: #ffffff;
    position: relative;
    overflow: hidden;
    box-shadow: 0 14px 32px rgba(136, 19, 55, 0.20);
}

.admin-hero-banner::after {
    content: "";
    position: absolute;
    right: -45px;
    top: -55px;
    width: 240px;
    height: 240px;
    border-radius: 50%;
    background: radial-gradient(circle, rgba(253, 164, 175, 0.26) 0%, transparent 70%);
    pointer-events: none;
}

/* Modern Executive KPI Cards */
.stat-kpi {
    position: relative;
    border-radius: 20px;
    padding: 1.35rem 1.4rem;
    background: #ffffff;
    border: 1px solid rgba(226, 232, 240, 0.9);
    box-shadow: 0 4px 20px rgba(15, 23, 42, 0.035);
    height: 100%;
    overflow: hidden;
    transition: transform 0.22s ease, box-shadow 0.22s ease;
}

.stat-kpi:hover {
    transform: translateY(-3px);
    box-shadow: 0 14px 30px rgba(15, 23, 42, 0.075);
}

.stat-kpi::before {
    content: "";
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 4px;
    background: linear-gradient(90deg, #e11d48, #fb7185);
}

.stat-kpi.kpi-orders::before {
    background: linear-gradient(90deg, #4f46e5, #38bdf8);
}

.stat-kpi.kpi-vip::before {
    background: linear-gradient(90deg, #d97706, #fbbf24);
}

.stat-kpi.kpi-stock::before {
    background: linear-gradient(90deg, #059669, #34d399);
}

.kpi-icon-orb {
    width: 46px;
    height: 46px;
    border-radius: 14px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: 1.25rem;
    flex-shrink: 0;
}

.kpi-orb-rose {
    background: linear-gradient(135deg, #ffe4e6, #fecdd3);
    color: #be123c;
}

.kpi-orb-indigo {
    background: linear-gradient(135deg, #e0e7ff, #dbeafe);
    color: #4338ca;
}

.kpi-orb-amber {
    background: linear-gradient(135deg, #fef3c7, #fde68a);
    color: #b45309;
}

.kpi-orb-emerald {
    background: linear-gradient(135deg, #d1fae5, #a7f3d0);
    color: #047857;
}

.kpi-value {
    font-size: 1.55rem;
    font-weight: 800;
    letter-spacing: -0.02em;
    color: #0f172a;
    line-height: 1.2;
}

/* Global Admin Table & Form Refinements */
.admin-wrapper .table {
    margin-bottom: 0;
}

.admin-wrapper .table > thead > tr > th {
    background-color: #f8fafc;
    color: #64748b;
    font-size: 0.72rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.06em;
    padding: 0.9rem 1rem;
    border-bottom: 1px solid #e2e8f0;
    white-space: nowrap;
}

.admin-wrapper .table > tbody > tr > td {
    padding: 0.9rem 1rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: middle;
    color: #334155;
}

.admin-wrapper .table > tbody > tr:last-child > td {
    border-bottom: none;
}

.admin-wrapper .table > tbody > tr {
    transition: background-color 0.15s ease;
}

.admin-wrapper .table > tbody > tr:hover > td {
    background-color: #fff9fb;
}

.admin-wrapper .badge {
    border-radius: 999px;
    padding: 0.38em 0.78em;
    font-weight: 600;
    letter-spacing: 0.01em;
}

.admin-wrapper .form-control,
.admin-wrapper .form-select {
    border-radius: 11px;
    border: 1.5px solid #e2e8f0;
    transition: border-color 0.18s ease, box-shadow 0.18s ease;
}

.admin-wrapper .form-control:focus,
.admin-wrapper .form-select:focus {
    border-color: var(--nt-primary);
    box-shadow: 0 0 0 3px rgba(225, 29, 72, 0.10);
}

.btn-admin-primary {
    background: linear-gradient(135deg, #e11d48 0%, #be123c 100%);
    color: #fff !important;
    border: none;
    border-radius: 999px;
    font-weight: 600;
    padding: 0.46rem 1.15rem;
    box-shadow: 0 4px 12px rgba(225, 29, 72, 0.24);
    transition: transform 0.18s ease, box-shadow 0.18s ease;
}

.btn-admin-primary:hover {
    transform: translateY(-1px);
    box-shadow: 0 7px 18px rgba(225, 29, 72, 0.34);
}

.admin-filter-pill {
    border-radius: 999px;
    padding: 0.38rem 0.95rem;
    font-size: 0.83rem;
    font-weight: 600;
    border: 1.5px solid #e2e8f0;
    background: #f8fafc;
    color: #475569;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    gap: 5px;
    transition: all 0.18s ease;
}

.admin-filter-pill:hover {
    background: #fff1f2;
    border-color: #fda4af;
    color: var(--nt-primary);
}

.admin-filter-pill.active {
    background: linear-gradient(135deg, #e11d48 0%, #be123c 100%);
    border-color: transparent;
    color: #ffffff;
    box-shadow: 0 4px 12px rgba(225, 29, 72, 0.25);
}

.rank-medal {
    width: 26px;
    height: 26px;
    border-radius: 50%;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    font-size: 0.74rem;
    font-weight: 800;
    flex-shrink: 0;
}

.rank-1 { background: #fef3c7; color: #b45309; border: 1px solid #fcd34d; }
.rank-2 { background: #f1f5f9; color: #475569; border: 1px solid #cbd5e1; }
.rank-3 { background: #ffedd5; color: #c2410c; border: 1px solid #fdba74; }
.rank-n { background: #f8fafc; color: #94a3b8; border: 1px solid #e2e8f0; }

.customer-avatar-circle {
    width: 38px;
    height: 38px;
    border-radius: 50%;
    background: linear-gradient(135deg, #ffe4e6, #fecdd3);
    color: #be123c;
    font-weight: 700;
    font-size: 0.84rem;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    border: 1px solid #fda4af;
}

/* Floating Toast */
#ntToastContainer {
    position: fixed;
    top: 84px;
    right: 24px;
    z-index: 1080;
}

footer.nt-footer {
    background: #2b0916;
    color: #fce7f3;
}
```

### File: FlowerShop/Views\_ViewStart.cshtml
```html
@{
    Layout = "~/Views/Shared/_LayoutUser.cshtml";
}
```

### File: FlowerShop/Views\Account\AccessDenied.cshtml
```html
@{
    ViewData["Title"] = "Từ chối truy cập";
}

<div class="container py-5 text-center" style="max-width:520px">
    <div class="bg-white p-5 rounded-4 border shadow-sm">
        <i class="bi bi-shield-lock-fill text-danger display-3"></i>
        <h3 class="fw-bold mt-3">Khu Vực Dành Cho Quản Trị Viên</h3>
        <p class="text-muted">Tài khoản hiện tại của bạn không có quyền truy cập chức năng quản trị này.</p>
        <a href="@Url.Action("Index", "Home")" class="btn btn-nt-primary">Quay về trang chủ</a>
    </div>
</div>
```

### File: FlowerShop/Views\Account\ForgotPassword.cshtml
```html
@{
    ViewData["Title"] = "Quên mật khẩu";
}

<div class="container py-5" style="max-width:480px">
    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm">
        <h3 class="fw-bold mb-2 text-center">Khôi Phục Mật Khẩu</h3>
        <p class="text-muted small text-center mb-4">Xác minh Email và Số điện thoại đã đăng ký để thiết lập mật khẩu mới.</p>

        @if (ViewBag.Error != null)
        {
            <div class="alert alert-danger small rounded-3">@ViewBag.Error</div>
        }

        <form action="@Url.Action("ForgotPassword", "Account")" method="post">
            @Html.AntiForgeryToken()
            <div class="mb-3">
                <label class="form-label small fw-semibold">Email tài khoản</label>
                <input type="email" name="email" class="form-control" required placeholder="khachvip@chamflowers.vn" />
            </div>
            <div class="mb-3">
                <label class="form-label small fw-semibold">Số điện thoại đăng ký</label>
                <input type="text" name="phone" class="form-control" required placeholder="0912345678" />
            </div>
            <div class="mb-4">
                <label class="form-label small fw-semibold">Mật khẩu mới</label>
                <input type="password" name="newPassword" class="form-control" required minlength="6" placeholder="Nhập mật khẩu mới (>= 6 ký tự)" />
            </div>
            <button type="submit" class="btn btn-nt-primary w-100">Đặt lại mật khẩu</button>
        </form>
    </div>
</div>
```

### File: FlowerShop/Views\Banner\Index.cshtml
```html
@model List<FlowerShop.Models.Banner>
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Quản lý Nội dung Website: Banner • Tin tức • Mã giảm giá - CHAM Flowers Executive CMS";
    var newsList = ViewBag.NewsList as List<FlowerShop.Models.NewsArticle> ?? new List<FlowerShop.Models.NewsArticle>();
    var coupons = ViewBag.Coupons as List<FlowerShop.Models.Coupon> ?? new List<FlowerShop.Models.Coupon>();
}

<!-- 1. Quản lý Banner Quảng Cáo -->
<div class="admin-card p-4 mb-4">
    <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
        <h6 class="fw-bold mb-0"><i class="bi bi-images text-danger me-2"></i>Quản Lý Banner Trang Chủ</h6>
        <form action="@Url.Action("Index", "Banner")" method="get" class="d-flex gap-1">
            <input type="text" name="search" value="@ViewBag.Search" class="form-control form-control-sm" placeholder="Tìm tiêu đề banner..." />
            <button type="submit" class="btn btn-sm btn-dark">Tìm</button>
        </form>
    </div>

    <form action="@Url.Action("SaveBanner", "Banner")" method="post" enctype="multipart/form-data" class="row g-2 bg-light p-3 rounded-3 border mb-3">
        @Html.AntiForgeryToken()
        <input type="hidden" name="Id" value="0" />
        <div class="col-md-4"><input type="text" name="Title" class="form-control form-control-sm" placeholder="Tiêu đề Banner mới *" required /></div>
        <div class="col-md-4"><input type="text" name="Subtitle" class="form-control form-control-sm" placeholder="Mô tả phụ..." /></div>
        <div class="col-md-4"><input type="text" name="LinkUrl" class="form-control form-control-sm" placeholder="Link đích (VD: /HotDeal)" /></div>
        <div class="col-md-5"><input type="file" name="imageFile" class="form-control form-control-sm" /></div>
        <div class="col-md-5"><input type="text" name="ImageUrl" class="form-control form-control-sm" placeholder="Hoặc URL ảnh (/images/samflora/editorial-2.jpg)" /></div>
        <input type="hidden" name="IsActive" value="true" />
        <div class="col-md-2 d-grid"><button type="submit" class="btn btn-sm btn-danger">+ Thêm Banner</button></div>
    </form>

    <div class="table-responsive">
        <table class="table table-sm align-middle mb-0">
            <thead class="small text-muted">
                <tr><th>Ảnh</th><th>Tiêu đề</th><th>Đường dẫn</th><th>Ngày tạo</th><th></th></tr>
            </thead>
            <tbody>
                @foreach (var b in Model)
                {
                    <tr>
                        <td><img src="@b.ImageUrl" class="rounded-2 border" style="width:96px;height:42px;object-fit:cover" alt="" /></td>
                        <td>
                            <div class="fw-bold small">@b.Title</div>
                            <div class="text-muted" style="font-size:0.74rem">@b.Subtitle</div>
                        </td>
                        <td class="small"><code>@b.LinkUrl</code></td>
                        <td class="small">@b.CreatedAt.ToString("dd/MM/yyyy")</td>
                        <td class="text-end">
                            <form action="@Url.Action("DeleteBanner", "Banner")" method="post" class="d-inline" onsubmit="return confirm('Xóa banner này?');">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="id" value="@b.Id" />
                                <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                            </form>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

<div class="row g-4">
    <!-- 2. Quản lý Mã giảm giá / Tri ân -->
    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <h6 class="fw-bold mb-3"><i class="bi bi-ticket-perforated-fill text-danger me-2"></i>Mã Giảm Giá Khuyến Mãi</h6>
            <form action="@Url.Action("SaveCoupon", "Banner")" method="post" class="row g-2 bg-light p-3 rounded-3 border mb-3">
                @Html.AntiForgeryToken()
                <input type="hidden" name="Id" value="0" />
                <input type="hidden" name="IsActive" value="true" />
                <div class="col-4"><input type="text" name="Code" class="form-control form-control-sm text-uppercase" placeholder="MÃ (VD: SINHNHAT)" required /></div>
                <div class="col-4"><input type="number" name="DiscountPercent" value="15" class="form-control form-control-sm" placeholder="% giảm" /></div>
                <div class="col-4"><input type="number" name="MinOrderAmount" value="300000" class="form-control form-control-sm" placeholder="Đơn tối thiểu" /></div>
                <div class="col-8"><input type="text" name="Description" class="form-control form-control-sm" placeholder="Mô tả ưu đãi..." required /></div>
                <div class="col-4 d-grid"><button type="submit" class="btn btn-sm btn-danger">+ Tạo mã</button></div>
            </form>

            <table class="table table-sm align-middle mb-0">
                <thead class="small text-muted"><tr><th>Mã</th><th>Ưu đãi</th><th></th></tr></thead>
                <tbody>
                    @foreach (var c in coupons)
                    {
                        <tr>
                            <td><span class="badge bg-danger">@c.Code</span></td>
                            <td class="small">@c.Description</td>
                            <td class="text-end">
                                <form action="@Url.Action("DeleteCoupon", "Banner")" method="post" class="d-inline">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="id" value="@c.Id" />
                                    <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                                </form>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>

    <!-- 3. Quản lý Tin tức & Cẩm nang -->
    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <h6 class="fw-bold mb-3"><i class="bi bi-journal-richtext text-danger me-2"></i>Bài Viết Tin Tức & Cẩm Nang</h6>
            <form action="@Url.Action("SaveNews", "Banner")" method="post" class="row g-2 bg-light p-3 rounded-3 border mb-3">
                @Html.AntiForgeryToken()
                <input type="hidden" name="Id" value="0" />
                <input type="hidden" name="IsPublished" value="true" />
                <div class="col-8"><input type="text" name="Title" class="form-control form-control-sm" placeholder="Tiêu đề bài viết *" required /></div>
                <div class="col-4"><input type="text" name="CategoryName" value="Cẩm nang hoa tươi" class="form-control form-control-sm" /></div>
                <div class="col-12"><input type="text" name="Summary" class="form-control form-control-sm" placeholder="Tóm tắt ngắn..." /></div>
                <div class="col-12"><textarea name="Content" rows="2" class="form-control form-control-sm" placeholder="Nội dung bài viết..." required></textarea></div>
                <div class="col-12 text-end"><button type="submit" class="btn btn-sm btn-danger">Đăng bài viết</button></div>
            </form>

            <ul class="list-group list-group-flush small">
                @foreach (var n in newsList)
                {
                    <li class="list-group-item px-0 d-flex justify-content-between align-items-center">
                        <div>
                            <div class="fw-semibold">@n.Title</div>
                            <span class="text-muted" style="font-size:0.72rem">@n.CreatedAt.ToString("dd/MM/yyyy")</span>
                        </div>
                        <form action="@Url.Action("DeleteNews", "Banner")" method="post">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="id" value="@n.Id" />
                            <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                        </form>
                    </li>
                }
            </ul>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Chat\Index.cshtml
```html
@model FlowerShop.Models.ChatConversation
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Trung tâm Chăm sóc Khách hàng Trực tuyến (SignalR) - CHAM Flowers Executive CMS";
    var conversations = ViewBag.Conversations as List<FlowerShop.Models.ChatConversation> ?? new List<FlowerShop.Models.ChatConversation>();
    var quickProducts = ViewBag.QuickProducts as List<FlowerShop.Models.Product> ?? new List<FlowerShop.Models.Product>();
    var quickCoupons = ViewBag.QuickCoupons as List<FlowerShop.Models.Coupon> ?? new List<FlowerShop.Models.Coupon>();
    string curFilter = ViewBag.StatusFilter as string;
}

<div class="row g-3">
    <!-- Sidebar: Danh sách khách hàng đang hoạt động & Lọc trạng thái -->
    <div class="col-lg-4">
        <div class="admin-card p-3 h-100 d-flex flex-column" style="min-height:560px">
            <div class="d-flex justify-content-between align-items-center mb-2">
                <h6 class="fw-bold mb-0"><i class="bi bi-people-fill text-danger me-1"></i>Danh Sách Hội Thoại</h6>
                <span class="badge bg-success-subtle text-success">SignalR Live</span>
            </div>

            <div class="d-flex flex-wrap gap-1 mb-3">
                <a href="@Url.Action("Index", "Chat")" class="badge text-decoration-none @(string.IsNullOrEmpty(curFilter) ? "bg-danger" : "bg-light text-dark border")">Tất cả</a>
                <a href="@Url.Action("Index", "Chat", new { statusFilter = "Đang xử lý" })" class="badge text-decoration-none @(curFilter == "Đang xử lý" ? "bg-danger" : "bg-light text-dark border")">Đang xử lý</a>
                <a href="@Url.Action("Index", "Chat", new { statusFilter = "Đã giải quyết" })" class="badge text-decoration-none @(curFilter == "Đã giải quyết" ? "bg-danger" : "bg-light text-dark border")">Đã giải quyết</a>
                <a href="@Url.Action("Index", "Chat", new { statusFilter = "Cần chuyển tiếp" })" class="badge text-decoration-none @(curFilter == "Cần chuyển tiếp" ? "bg-danger" : "bg-light text-dark border")">Cần chuyển tiếp</a>
            </div>

            @if (!conversations.Any())
            {
                <div class="text-center text-muted my-auto py-5 small">
                    <i class="bi bi-chat-square-dots display-5 d-block mb-2"></i>
                    Chưa có hội thoại
                </div>
            }
            else
            {
                <div class="list-group list-group-flush overflow-auto">
                    @foreach (var c in conversations)
                    {
                        bool active = Model != null && Model.Id == c.Id;
                        var lastMsg = c.Messages != null ? c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault() : null;
                        <a href="@Url.Action("Index", "Chat", new { id = c.Id, statusFilter = curFilter })"
                           class="list-group-item list-group-item-action rounded-3 mb-1 border @(active ? "bg-danger-subtle border-danger" : "")">
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="fw-bold small text-dark">
                                    <span class="d-inline-block rounded-circle me-1 @(c.IsCustomerOnline ? "bg-success" : "bg-secondary")" style="width:8px;height:8px"></span>
                                    @c.CustomerName
                                </div>
                                @if (c.UnreadAdminCount > 0)
                                {
                                    <span class="badge rounded-pill bg-danger">@c.UnreadAdminCount</span>
                                }
                            </div>
                            <div class="text-muted text-truncate mt-1" style="font-size:0.76rem">@(lastMsg != null ? lastMsg.Content : "Bắt đầu trò chuyện")</div>
                            <div class="d-flex justify-content-between align-items-center mt-1">
                                <span class="badge bg-white text-dark border" style="font-size:0.68rem">@c.Status</span>
                                <span class="text-muted" style="font-size:0.68rem">@c.LastMessageAt.ToString("HH:mm dd/MM")</span>
                            </div>
                        </a>
                    }
                </div>
            }
        </div>
    </div>

    <!-- Cửa sổ hội thoại tập trung + Trạng thái xử lý + Công cụ gửi nhanh -->
    <div class="col-lg-8">
        <div class="admin-card h-100 d-flex flex-column" style="min-height:560px">
            @if (Model == null)
            {
                <div class="m-auto text-center text-muted p-5">
                    <h5>Chưa có hội thoại nào được chọn</h5>
                    <p class="small mb-0">Khi khách hàng nhắn tin tư vấn trên website, hội thoại sẽ hiển thị tức thì tại đây.</p>
                </div>
            }
            else
            {
                <!-- Header hội thoại & Gán trạng thái xử lý -->
                <div class="p-3 border-bottom d-flex flex-wrap justify-content-between align-items-center gap-2">
                    <div>
                        <div class="fw-bold">
                            @Model.CustomerName
                            <span class="badge @(Model.IsCustomerOnline ? "bg-success" : "bg-secondary") ms-1" style="font-size:0.7rem">
                                @(Model.IsCustomerOnline ? "Đang trực tuyến" : "Ngoại tuyến")
                            </span>
                        </div>
                        <div class="text-muted small">Mã phiên: <code>@Model.SessionKey</code></div>
                    </div>

                    <form action="@Url.Action("UpdateStatus", "Chat")" method="post" class="d-flex align-items-center gap-2">
                        @Html.AntiForgeryToken()
                        <input type="hidden" name="conversationId" value="@Model.Id" />
                        <select name="status" class="form-select form-select-sm" onchange="this.form.submit()">
                            <option value="Đang xử lý" @Html.Raw(Model.Status == "Đang xử lý" ? "selected=\"selected\"" : "")>🟡 Đang xử lý</option>
                            <option value="Đã giải quyết" @Html.Raw(Model.Status == "Đã giải quyết" ? "selected=\"selected\"" : "")>🟢 Đã giải quyết</option>
                            <option value="Cần chuyển tiếp" @Html.Raw(Model.Status == "Cần chuyển tiếp" ? "selected=\"selected\"" : "")>🔴 Cần chuyển tiếp</option>
                        </select>
                    </form>
                </div>

                <!-- Khung bong bóng chat -->
                <div id="adminChatBox" class="p-3 flex-grow-1 overflow-auto d-flex flex-column gap-2" style="max-height:370px;background:#fff9fb">
                    @if (Model.Messages != null)
                    {
                        foreach (var m in Model.Messages.OrderBy(x => x.CreatedAt))
                        {
                            string cls = m.SenderRole == "Admin" ? "customer" : (m.SenderRole == "System" ? "system" : "admin");
                            <div class="chat-bubble @cls">
                                <div class="fw-bold mb-1" style="font-size:0.72rem">@m.SenderName</div>
                                <div>@m.Content</div>
                                @if (m.MessageType == "ProductLink" && !string.IsNullOrEmpty(m.AttachmentUrl))
                                {
                                    <div class="mt-1"><a href="@m.AttachmentUrl" target="_blank" class="badge bg-white text-danger text-decoration-none">Link mẫu hoa: @m.AttachmentUrl</a></div>
                                }
                                else if (m.MessageType == "Coupon" && !string.IsNullOrEmpty(m.AttachmentUrl))
                                {
                                    <div class="mt-1"><span class="badge bg-warning text-dark">Voucher: @m.AttachmentUrl</span></div>
                                }
                                else if (m.MessageType == "Image" && !string.IsNullOrEmpty(m.AttachmentUrl))
                                {
                                    <div class="mt-2"><img src="@m.AttachmentUrl" class="rounded-2" style="max-height:110px" alt="" /></div>
                                }
                                <div class="opacity-75 mt-1" style="font-size:0.68rem">@m.CreatedAt.ToString("HH:mm dd/MM")</div>
                            </div>
                        }
                    }
                </div>

                <!-- Thanh công cụ gửi nhanh -->
                <div class="px-3 py-2 border-top bg-light d-flex flex-wrap gap-2 align-items-center">
                    <span class="small fw-semibold text-muted"><i class="bi bi-lightning-charge-fill text-warning"></i> Gửi nhanh:</span>

                    <div class="dropdown">
                        <button class="btn btn-sm btn-white border bg-white dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            <i class="bi bi-flower1 text-danger"></i> Link sản phẩm
                        </button>
                        <ul class="dropdown-menu small shadow">
                            @foreach (var qp in quickProducts)
                            {
                                <li>
                                    <a class="dropdown-item" href="javascript:void(0)"
                                       onclick="sendQuickTool('Mời bạn tham khảo mẫu @qp.Name (@AppHelpers.FormatVnd(qp.Price)) đang rất được ưa chuộng tại shop ạ!', 'ProductLink', '/Product/Details/@qp.Id')">
                                        @qp.Name
                                    </a>
                                </li>
                            }
                        </ul>
                    </div>

                    <div class="dropdown">
                        <button class="btn btn-sm btn-white border bg-white dropdown-toggle" type="button" data-bs-toggle="dropdown">
                            <i class="bi bi-ticket-perforated text-success"></i> Mã giảm giá
                        </button>
                        <ul class="dropdown-menu small shadow">
                            @foreach (var qc in quickCoupons)
                            {
                                <li>
                                    <a class="dropdown-item" href="javascript:void(0)"
                                       onclick="sendQuickTool('CHAM Flowers gửi tặng bạn mã ưu đãi @qc.Code (@qc.Description) nhé!', 'Coupon', '@qc.Code')">
                                        @qc.Code (-@qc.DiscountPercent%)
                                    </a>
                                </li>
                            }
                        </ul>
                    </div>

                    <button type="button" class="btn btn-sm btn-white border bg-white"
                            onclick="sendQuickTool('Em gửi anh/chị ảnh mẫu hoa cắm thực tế tại cửa hàng CHAM Flowers ạ!', 'Image', '/images/samflora/b10-main.jpg')">
                        <i class="bi bi-camera text-primary"></i> Gửi ảnh hoa thực tế
                    </button>
                </div>

                <!-- Form nhập phản hồi -->
                <form id="adminReplyForm" class="p-3 border-top d-flex gap-2">
                    <input type="hidden" id="activeConvId" value="@Model.Id" />
                    <input type="text" id="adminReplyInput" class="form-control" placeholder="Nhập tin nhắn tư vấn phản hồi cho @Model.CustomerName..." autocomplete="off" />
                    <button type="submit" class="btn btn-danger rounded-pill px-4 text-nowrap"><i class="bi bi-send-fill me-1"></i> Trả lời</button>
                </form>
            }
        </div>
    </div>
</div>

@section Scripts {
    <script>
        var activeConvId = parseInt(document.getElementById('activeConvId') ? document.getElementById('activeConvId').value : '0');
        var adminBox = document.getElementById('adminChatBox');
        var seenIds = {};

        if (adminBox) adminBox.scrollTop = adminBox.scrollHeight;

        function appendAdminViewMsg(m) {
            if (!adminBox || (m.id && seenIds[m.id])) return;
            if (m.id) seenIds[m.id] = true;

            var cls = m.senderRole === 'Admin' ? 'customer' : 'admin';
            var extra = '';
            if (m.messageType === 'ProductLink' && m.attachmentUrl) {
                extra = '<div class="mt-1"><a href="' + m.attachmentUrl + '" target="_blank" class="badge bg-white text-danger text-decoration-none">Link mẫu hoa: ' + m.attachmentUrl + '</a></div>';
            } else if (m.messageType === 'Coupon' && m.attachmentUrl) {
                extra = '<div class="mt-1"><span class="badge bg-warning text-dark">Voucher: ' + m.attachmentUrl + '</span></div>';
            } else if (m.messageType === 'Image' && m.attachmentUrl) {
                extra = '<div class="mt-2"><img src="' + m.attachmentUrl + '" class="rounded-2" style="max-height:110px"/></div>';
            }

            var div = document.createElement('div');
            div.className = 'chat-bubble ' + cls;
            div.innerHTML = '<div class="fw-bold mb-1" style="font-size:0.72rem">' + m.senderName + '</div><div>' + m.content + '</div>' + extra + '<div class="opacity-75 mt-1" style="font-size:0.68rem">' + m.createdAt + '</div>';
            adminBox.appendChild(div);
            adminBox.scrollTop = adminBox.scrollHeight;
        }

        function sendQuickTool(content, type, url) {
            if (!activeConvId) return;
            $.post('/Chat/AdminReply', {
                conversationId: activeConvId,
                content: content,
                messageType: type,
                attachmentUrl: url
            }, function (res) {
                if (res && res.success) appendAdminViewMsg(res.message);
            });
        }

        var replyForm = document.getElementById('adminReplyForm');
        if (replyForm) {
            replyForm.addEventListener('submit', function (e) {
                e.preventDefault();
                var inp = document.getElementById('adminReplyInput');
                var text = inp.value ? inp.value.trim() : '';
                if (!text || !activeConvId) return;
                inp.value = '';
                sendQuickTool(text, 'Text', null);
            });
        }

        if (window.$ && $.connection && $.connection.chatHub) {
            var hub = $.connection.chatHub;
            hub.client.receiveMessage = function (payload) {
                if (payload.conversationId === activeConvId) {
                    appendAdminViewMsg(payload);
                } else {
                    setTimeout(function () { location.reload(); }, 800);
                }
            };
            $.connection.hub.start().done(function () {
                hub.server.joinAdminChannel();
            });
        }
    </script>
}
```

### File: FlowerShop/Views\Customer\Create.cshtml
```html
@model FlowerShop.Models.User
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Thêm Tài khoản Khách hàng mới - CHAM Flowers Executive CMS";
}

<div class="admin-card p-4" style="max-width:680px">
    <h6 class="fw-bold mb-3">Thêm Tài Khoản Mới</h6>
    <form action="@Url.Action("Create", "Customer")" method="post">
        @Html.AntiForgeryToken()
        @Html.ValidationSummary(true, "", new { @class = "alert alert-danger small rounded-3" })

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label small fw-bold">Họ và tên *</label>
                <input type="text" name="FullName" value="@Model.FullName" class="form-control" placeholder="Nhập họ tên khách hàng" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Số điện thoại *</label>
                <input type="text" name="Phone" value="@Model.Phone" class="form-control" placeholder="0909xxxxxx" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Email đăng nhập *</label>
                <input type="email" name="Email" value="@Model.Email" class="form-control" placeholder="khachhang@domain.com" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Mật khẩu khởi tạo *</label>
                <input type="password" name="Password" class="form-control" placeholder="Tối thiểu 6 ký tự" required />
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Địa chỉ nhận hàng *</label>
                <input type="text" name="Address" value="@Model.Address" class="form-control" placeholder="Địa chỉ giao hoa..." required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Phân quyền</label>
                <select name="Role" class="form-select">
                    <option value="Customer" @Html.Raw(Model.Role == "Admin" ? "" : "selected=\"selected\"")>Khách hàng (Customer)</option>
                    <option value="Admin" @Html.Raw(Model.Role == "Admin" ? "selected=\"selected\"" : "")>Quản trị viên (Admin)</option>
                </select>
            </div>
            <div class="col-md-6 d-flex align-items-end gap-4">
                <div class="form-check">
                    <input type="checkbox" name="IsVip" value="true" @Html.Raw(Model.IsVip ? "checked=\"checked\"" : "") class="form-check-input" id="chkVip" />
                    <label for="chkVip" class="form-check-label small fw-semibold">Khách hàng VIP</label>
                </div>
                <div class="form-check">
                    <input type="checkbox" name="IsActive" value="true" @Html.Raw(Model.IsActive ? "checked=\"checked\"" : "") class="form-check-input" id="chkActive" />
                    <label for="chkActive" class="form-check-label small fw-semibold">Kích hoạt</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Lưu tài khoản</button>
            <a href="@Url.Action("Index", "Customer")" class="btn btn-light border rounded-pill px-4">Hủy</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\Customer\Details.cshtml
```html
@model FlowerShop.Models.User
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = string.Format("Hồ sơ Khách hàng: {0} - CHAM Flowers Executive CMS", Model.FullName);
}

<div class="mb-3">
    <a href="@Url.Action("Index", "Customer")" class="btn btn-sm btn-light border rounded-pill"><i class="bi bi-arrow-left me-1"></i>Danh sách khách hàng</a>
</div>

<div class="row g-4">
    <div class="col-lg-4">
        <div class="admin-card p-4">
            <h5 class="fw-bold mb-1">@Model.FullName</h5>
            <div class="text-muted small mb-3">@Model.Email</div>
            <div class="small mb-2"><strong>Số điện thoại:</strong> @(Model.Phone ?? "Chưa cập nhật")</div>
            <div class="small mb-2"><strong>Địa chỉ:</strong> @Model.Address</div>
            <div class="small mb-2"><strong>Ngày đăng ký:</strong> @Model.CreatedAt.ToString("dd/MM/yyyy")</div>
            <div class="small mb-2"><strong>Tổng chi tiêu:</strong> <span class="text-danger fw-bold">@AppHelpers.FormatVnd(Model.TotalSpent)</span></div>
            <div class="mt-3 d-flex gap-2">
                <a href="@Url.Action("Edit", "Customer", new { id = Model.Id })" class="btn btn-sm btn-primary rounded-pill px-3">Chỉnh sửa</a>
            </div>
        </div>
    </div>

    <div class="col-lg-8">
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3">Lịch Sử Mua Hoa (@(Model.Orders != null ? Model.Orders.Count : 0) đơn)</h6>
            @if (Model.Orders == null || !Model.Orders.Any())
            {
                <p class="text-muted small mb-0">Khách hàng chưa có đơn hàng nào.</p>
            }
            else
            {
                <table class="table table-sm align-middle mb-0">
                    <thead class="small text-muted">
                        <tr><th>Mã đơn</th><th>Ngày đặt</th><th>Tổng tiền</th><th>Trạng thái</th><th></th></tr>
                    </thead>
                    <tbody>
                        @foreach (var o in Model.Orders.OrderByDescending(x => x.CreatedAt))
                        {
                            <tr>
                                <td class="fw-bold">#@o.Id</td>
                                <td class="small">@o.CreatedAt.ToString("dd/MM/yyyy HH:mm")</td>
                                <td class="fw-bold text-danger small">@AppHelpers.FormatVnd(o.Total)</td>
                                <td><span class="badge bg-light text-dark border">@o.Status</span></td>
                                <td class="text-end"><a href="@Url.Action("AdminDetails", "Order", new { id = o.Id })" class="btn btn-sm btn-light border">Xem đơn</a></td>
                            </tr>
                        }
                    </tbody>
                </table>
            }
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Customer\Edit.cshtml
```html
@model FlowerShop.Models.User
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = string.Format("Sửa thông tin Tài khoản #{0} - CHAM Flowers Executive CMS", Model.Id);
}

<div class="admin-card p-4" style="max-width:680px">
    <h6 class="fw-bold mb-3">Cập Nhật Thông Tin & Phân Quyền Tài Khoản</h6>
    <form action="@Url.Action("Edit", "Customer", new { id = Model.Id })" method="post">
        @Html.AntiForgeryToken()
        @Html.ValidationSummary(true, "", new { @class = "alert alert-danger small rounded-3" })

        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label small fw-bold">Họ và tên *</label>
                <input type="text" name="fullName" value="@Model.FullName" class="form-control" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Số điện thoại *</label>
                <input type="text" name="phone" value="@Model.Phone" class="form-control" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Email (Không thay đổi)</label>
                <input type="text" value="@Model.Email" class="form-control bg-light" readonly />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Đặt mật khẩu mới (Để trống nếu giữ nguyên)</label>
                <input type="password" name="newPassword" class="form-control" placeholder="Mật khẩu mới..." />
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Địa chỉ *</label>
                <input type="text" name="address" value="@Model.Address" class="form-control" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Vai trò hệ thống</label>
                <select name="role" class="form-select">
                    <option value="Customer" @Html.Raw(Model.Role == "Customer" ? "selected=\"selected\"" : "")>Customer (Khách hàng)</option>
                    <option value="Admin" @Html.Raw(Model.Role == "Admin" ? "selected=\"selected\"" : "")>Admin (Quản trị viên)</option>
                </select>
            </div>
            <div class="col-md-6 d-flex align-items-end gap-4">
                <div class="form-check">
                    <input type="checkbox" name="isVip" value="true" @Html.Raw(Model.IsVip ? "checked=\"checked\"" : "") class="form-check-input" id="chkVip" />
                    <label for="chkVip" class="form-check-label small fw-semibold">Khách VIP</label>
                </div>
                <div class="form-check">
                    <input type="checkbox" name="isActive" value="true" @Html.Raw(Model.IsActive ? "checked=\"checked\"" : "") class="form-check-input" id="chkAct" />
                    <label for="chkAct" class="form-check-label small fw-semibold">Cho phép hoạt động</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Lưu thay đổi</button>
            <a href="@Url.Action("Index", "Customer")" class="btn btn-light border rounded-pill px-4">Quay lại</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\Customer\Index.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.User>
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Quản lý Tài khoản Khách hàng - CHAM Flowers Executive CMS";
    string curFilter = ViewBag.Filter as string;
}

<div class="admin-card p-3 px-4 mb-4 d-flex flex-wrap justify-content-between align-items-center gap-3">
    <div class="d-flex flex-wrap gap-2">
        <a href="@Url.Action("Index", "Customer")" class="admin-filter-pill @(string.IsNullOrEmpty(curFilter) ? "active" : "")">Tất cả</a>
        <a href="@Url.Action("Index", "Customer", new { filter = "vip" })" class="admin-filter-pill @(curFilter == "vip" ? "active" : "")"><i class="bi bi-gem"></i> Khách VIP</a>
        <a href="@Url.Action("Index", "Customer", new { filter = "locked" })" class="admin-filter-pill @(curFilter == "locked" ? "active" : "")">Đã khóa</a>
        <a href="@Url.Action("Index", "Customer", new { filter = "admin" })" class="admin-filter-pill @(curFilter == "admin" ? "active" : "")">Quản trị viên</a>
    </div>

    <div class="d-flex flex-wrap align-items-center gap-2">
        <form action="@Url.Action("Index", "Customer")" method="get" class="d-flex align-items-center gap-2">
            <div class="admin-quick-search" style="width:225px">
                <i class="bi bi-search text-muted me-2" style="font-size:0.82rem"></i>
                <input type="text" name="search" value="@ViewBag.Search" placeholder="Tên, Email, SĐT..." />
            </div>
            <button type="submit" class="btn btn-sm btn-dark rounded-pill px-3" style="height:38px">Tìm</button>
        </form>
        <a href="@Url.Action("Create", "Customer")" class="btn btn-sm btn-admin-primary text-nowrap">
            <i class="bi bi-person-plus-fill me-1"></i>Thêm khách hàng
        </a>
    </div>
</div>

<div class="admin-card overflow-hidden">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead>
                <tr>
                    <th class="ps-4">ID</th>
                    <th>Hội viên & Liên hệ</th>
                    <th>Số điện thoại</th>
                    <th>Vai trò / Hạng thẻ</th>
                    <th>Tổng tích lũy</th>
                    <th>Trạng thái</th>
                    <th class="text-end pe-4">Thao tác</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var u in Model)
                {
                    string uInit = AppHelpers.GetInitials(u.FullName);
                    <tr>
                        <td class="ps-4 text-muted fw-semibold small">#@u.Id</td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <span class="customer-avatar-circle">@uInit</span>
                                <div>
                                    <div class="fw-bold small text-dark">@u.FullName</div>
                                    <div class="text-muted" style="font-size:0.75rem">@u.Email</div>
                                </div>
                            </div>
                        </td>
                        <td class="small">@u.Phone</td>
                        <td>
                            <span class="badge @(u.Role == "Admin" ? "bg-dark" : "bg-light text-secondary border")">@u.Role</span>
                            @if (u.IsVip)
                            {
                                <span class="badge bg-warning-subtle text-warning-emphasis border border-warning-subtle ms-1"><i class="bi bi-gem me-1"></i>VIP</span>
                            }
                        </td>
                        <td class="fw-bold text-danger small">@AppHelpers.FormatVnd(u.TotalSpent)</td>
                        <td>
                            <span class="badge @(u.IsActive ? "bg-success-subtle text-success" : "bg-danger-subtle text-danger")">
                                @(u.IsActive ? "● Hoạt động" : "● Đã khóa")
                            </span>
                        </td>
                        <td class="text-end pe-4">
                            <div class="d-inline-flex gap-1">
                                <a href="@Url.Action("Details", "Customer", new { id = u.Id })" class="btn btn-sm btn-light border rounded-pill px-3">Chi tiết</a>
                                <a href="@Url.Action("Edit", "Customer", new { id = u.Id })" class="btn btn-sm btn-outline-primary rounded-pill px-3">Sửa</a>
                                @if (u.Email != "admin@chamflowers.vn" && u.Email != "admin@ntflowers.vn")
                                {
                                    <form action="@Url.Action("ToggleLock", "Customer", new { id = u.Id })" method="post"
                                          onsubmit="return confirm('Bạn có chắc chắn muốn @(u.IsActive ? "khóa" : "mở khóa") tài khoản @u.FullName?');">
                                        @Html.AntiForgeryToken()
                                        <button type="submit" class="btn btn-sm rounded-pill px-3 @(u.IsActive ? "btn-outline-danger" : "btn-outline-success")">
                                            @(u.IsActive ? "Khóa" : "Mở khóa")
                                        </button>
                                    </form>
                                }
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

@if (Model.PageCount > 1)
{
    <nav class="mt-4 d-flex justify-content-center">
        <ul class="pagination pagination-sm">
            @for (int i = 1; i <= Model.PageCount; i++)
            {
                <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                    <a class="page-link" href="@Url.Action("Index", "Customer", new { page = i, filter = curFilter, search = ViewBag.Search })">@i</a>
                </li>
            }
        </ul>
    </nav>
}
```

### File: FlowerShop/Views\Home\NewsDetail.cshtml
```html
@model FlowerShop.Models.NewsArticle
@{
    ViewData["Title"] = Model.Title;
}

<div class="container py-4" style="max-width:860px">
    <nav aria-label="breadcrumb" class="mb-3">
        <ol class="breadcrumb small">
            <li class="breadcrumb-item"><a href="@Url.Action("Index", "Home")">Trang chủ</a></li>
            <li class="breadcrumb-item"><a href="@Url.Action("News", "Home")">Tin tức</a></li>
            <li class="breadcrumb-item active">@Model.Title</li>
        </ol>
    </nav>

    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm">
        <span class="badge bg-danger-subtle text-danger mb-2">@Model.CategoryName</span>
        <h1 class="h2 fw-bold mb-2">@Model.Title</h1>
        <div class="text-muted small mb-4"><i class="bi bi-calendar3 me-1"></i> Đăng ngày @Model.CreatedAt.ToString("dd/MM/yyyy") • Bởi CHAM Flowers Editorial</div>
        <img src="@(Model.ImageUrl ?? "/images/samflora/editorial-2.jpg")" class="w-100 rounded-4 mb-4" style="max-height:380px;object-fit:cover" alt="@Model.Title" />
        <div class="lh-lg">
            @Html.Raw(Model.Content)
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\HotDeal\CreateHotDealPopup.cshtml
```html
@model FlowerShop.Models.HotDealPopup
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = "Thêm Chương trình Hot Deal Popup";
}

<div class="admin-card p-4" style="max-width:700px">
    <form action="@Url.Action("CreateHotDealPopup", "HotDeal")" method="post" enctype="multipart/form-data">
        @Html.AntiForgeryToken()
        @if (!ViewData.ModelState.IsValid)
        {
            @Html.ValidationSummary(false, "", new { @class = "alert alert-danger small rounded-3" })
        }

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label small fw-bold">Tiêu đề chương trình</label>
                @Html.TextBoxFor(m => m.Title, new { @class = "form-control", placeholder = "VD: Flash Sale Hoa Sinh Nhật Giảm 25%" })
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Mã giảm giá đính kèm</label>
                @Html.TextBoxFor(m => m.CouponCode, new { @class = "form-control", placeholder = "CHAMFLOWERS15" })
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Mô tả ngắn</label>
                @Html.TextBoxFor(m => m.Subtitle, new { @class = "form-control", placeholder = "Áp dụng cho tất cả đơn hàng đặt trực tuyến..." })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Sản phẩm tiêu điểm (Tùy chọn)</label>
                @Html.DropDownListFor(m => m.ProductId, ViewBag.Products as SelectList, "-- Dẫn tới trang Hot Deal chung --", new { @class = "form-select" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Tải ảnh Popup (.jpg, .png, .gif)</label>
                <input type="file" name="imageFile" class="form-control" />
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Hoặc đường dẫn ảnh có sẵn</label>
                @Html.TextBoxFor(m => m.ImageUrl, new { @class = "form-control" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Thời gian bắt đầu (StartDate)</label>
                <input type="datetime-local" name="StartDate" value="@(Model.StartDate.HasValue ? Model.StartDate.Value.ToString("yyyy-MM-ddTHH:mm") : "")" class="form-control" />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Thời gian kết thúc (EndDate)</label>
                <input type="datetime-local" name="EndDate" value="@(Model.EndDate.HasValue ? Model.EndDate.Value.ToString("yyyy-MM-ddTHH:mm") : "")" class="form-control" />
            </div>
            <div class="col-12">
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input" })
                    <label for="IsActive" class="form-check-label small fw-semibold">Kích hoạt hiển thị Popup</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Lưu Hot Deal</button>
            <a href="@Url.Action("Manage", "HotDeal")" class="btn btn-light border rounded-pill px-4">Hủy</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\HotDeal\EditHotDealPopup.cshtml
```html
@model FlowerShop.Models.HotDealPopup
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = string.Format("Sửa Hot Deal Popup #{0}", Model.Id);
}

<div class="admin-card p-4" style="max-width:700px">
    <form action="@Url.Action("EditHotDealPopup", "HotDeal", new { id = Model.Id })" method="post" enctype="multipart/form-data">
        @Html.AntiForgeryToken()
        @Html.HiddenFor(m => m.Id)
        @if (!ViewData.ModelState.IsValid)
        {
            @Html.ValidationSummary(false, "", new { @class = "alert alert-danger small rounded-3" })
        }

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label small fw-bold">Tiêu đề chương trình</label>
                @Html.TextBoxFor(m => m.Title, new { @class = "form-control" })
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Mã ưu đãi</label>
                @Html.TextBoxFor(m => m.CouponCode, new { @class = "form-control" })
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Mô tả ngắn</label>
                @Html.TextBoxFor(m => m.Subtitle, new { @class = "form-control" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Sản phẩm tiêu điểm</label>
                @Html.DropDownListFor(m => m.ProductId, ViewBag.Products as SelectList, "-- Trang Hot Deal chung --", new { @class = "form-select" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Đổi ảnh mới</label>
                <input type="file" name="imageFile" class="form-control" />
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Đường dẫn ảnh hiện tại</label>
                @Html.TextBoxFor(m => m.ImageUrl, new { @class = "form-control" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Ngày bắt đầu</label>
                <input type="datetime-local" name="StartDate" value="@(Model.StartDate.HasValue ? Model.StartDate.Value.ToString("yyyy-MM-ddTHH:mm") : "")" class="form-control" />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Ngày kết thúc</label>
                <input type="datetime-local" name="EndDate" value="@(Model.EndDate.HasValue ? Model.EndDate.Value.ToString("yyyy-MM-ddTHH:mm") : "")" class="form-control" />
            </div>
            <div class="col-12">
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input" })
                    <label for="IsActive" class="form-check-label small fw-semibold">Kích hoạt hiển thị</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Lưu thay đổi</button>
            <a href="@Url.Action("Manage", "HotDeal")" class="btn btn-light border rounded-pill px-4">Quay lại</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\HotDeal\Index.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.Product>
@{
    ViewData["Title"] = "Hot Deal - Khuyến mãi hoa tươi";
    var categories = ViewBag.Categories as List<FlowerShop.Models.Category> ?? new List<FlowerShop.Models.Category>();
    DateTime endDate = ViewBag.EndDate != null ? (DateTime)ViewBag.EndDate : DateTime.Now.AddDays(5);
    int? selCat = ViewBag.CategoryId;
    string sortOrder = ViewBag.SortOrder;
}

<div class="container py-4">
    <!-- Banner Khuyến mãi kèm Bộ đếm thời gian Countdown -->
    <div class="rounded-4 p-4 p-lg-5 mb-4 text-white shadow-sm" style="background: linear-gradient(125deg, #881337 0%, #e11d48 60%, #db2777 100%);">
        <div class="row align-items-center g-4">
            <div class="col-lg-7">
                <span class="badge bg-warning text-dark fw-bold px-3 py-2 rounded-pill mb-2">
                    <i class="bi bi-fire"></i> CHƯƠNG TRÌNH HOT DEAL ĐẶC BIỆT
                </span>
                <h1 class="display-6 fw-bold mb-2">Săn Hoa Tươi Giá Ưu Đãi Tới 25%</h1>
                <p class="opacity-90 mb-0">Áp dụng thêm mã <strong>CHAMFLOWERS15</strong> tại giỏ hàng để giảm thêm 15% cho đơn từ 300.000 ₫!</p>
            </div>
            <div class="col-lg-5 text-lg-end">
                <div class="small text-uppercase fw-semibold mb-2 opacity-90">Kết thúc chương trình sau:</div>
                <div class="countdown-box" data-countdown="@endDate.ToString("yyyy-MM-ddTHH:mm:ss")">
                    <div class="countdown-unit bg-white text-danger"><strong class="cd-days">05</strong><small>Ngày</small></div>
                    <div class="countdown-unit bg-white text-danger"><strong class="cd-hours">12</strong><small>Giờ</small></div>
                    <div class="countdown-unit bg-white text-danger"><strong class="cd-mins">30</strong><small>Phút</small></div>
                    <div class="countdown-unit bg-white text-danger"><strong class="cd-secs">00</strong><small>Giây</small></div>
                </div>
            </div>
        </div>
    </div>

    <!-- Form tìm kiếm, lọc danh mục & sắp xếp -->
    <form action="@Url.Action("Index", "HotDeal")" method="get" class="bg-white p-3 rounded-4 border shadow-sm mb-4">
        <div class="row g-2 align-items-center">
            <div class="col-md-4">
                <input type="text" name="search" value="@ViewBag.Search" class="form-control" placeholder="Tìm tên mẫu hoa khuyến mãi..." />
            </div>
            <div class="col-md-3">
                <select name="categoryId" class="form-select">
                    <option value="">-- Tất cả danh mục --</option>
                    @foreach (var c in categories)
                    {
                        <option value="@c.Id" @Html.Raw(selCat == c.Id ? "selected=\"selected\"" : "")>@c.Name</option>
                    }
                </select>
            </div>
            <div class="col-md-3">
                <select name="sortOrder" class="form-select">
                    <option value="">Mới cập nhật</option>
                    <option value="price_asc" @Html.Raw(sortOrder == "price_asc" ? "selected=\"selected\"" : "")>Giá thấp đến cao</option>
                    <option value="price_desc" @Html.Raw(sortOrder == "price_desc" ? "selected=\"selected\"" : "")>Giá cao đến thấp</option>
                    <option value="name_asc" @Html.Raw(sortOrder == "name_asc" ? "selected=\"selected\"" : "")>Tên A → Z</option>
                </select>
            </div>
            <div class="col-md-2 d-grid">
                <button type="submit" class="btn btn-nt-primary"><i class="bi bi-search me-1"></i> Lọc</button>
            </div>
        </div>
    </form>

    <!-- Lưới sản phẩm Hot Deal -->
    @if (!Model.Any())
    {
        <div class="bg-white p-5 rounded-4 border text-center">
            <p class="text-muted mb-0">Hiện chưa có sản phẩm khuyến mãi phù hợp điều kiện lọc.</p>
        </div>
    }
    else
    {
        <div class="row g-4">
            @foreach (var item in Model)
            {
                <div class="col-sm-6 col-md-4 col-lg-3">
                    @Html.Partial("_ProductCard", item)
                </div>
            }
        </div>

        if (Model.PageCount > 1)
        {
            <nav class="mt-4 d-flex justify-content-center">
                <ul class="pagination">
                    @for (int i = 1; i <= Model.PageCount; i++)
                    {
                        <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                            <a class="page-link"
                               href="@Url.Action("Index", "HotDeal", new { page = i, search = ViewBag.Search, categoryId = ViewBag.CategoryId, sortOrder = ViewBag.SortOrder })">@i</a>
                        </li>
                    }
                </ul>
            </nav>
        }
    }
</div>
```

### File: FlowerShop/Views\HotDeal\Manage.cshtml
```html
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = "Quản lý Chương trình Hot Deal & Popup Khuyến mãi";
    var popups = ViewBag.Popups as List<FlowerShop.Models.HotDealPopup> ?? new List<FlowerShop.Models.HotDealPopup>();
    var hotProducts = ViewBag.HotDealProducts as List<FlowerShop.Models.Product> ?? new List<FlowerShop.Models.Product>();
    var allProducts = ViewBag.AllProducts as List<FlowerShop.Models.Product> ?? new List<FlowerShop.Models.Product>();
}

<div class="row g-4">
    <!-- 1. Quản lý HotDealPopup -->
    <div class="col-12">
        <div class="admin-card p-4">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <div>
                    <h6 class="fw-bold mb-0"><i class="bi bi-window-stack text-danger me-2"></i>Danh Sách Chiến Dịch Hot Deal Popup</h6>
                    <div class="text-muted small">Hiển thị tự động trên trang chủ theo thuộc tính <code>IsActiveNow</code> (StartDate → EndDate)</div>
                </div>
                <a href="@Url.Action("CreateHotDealPopup", "HotDeal")" class="btn btn-sm btn-danger rounded-pill px-3">+ Thêm Popup Hot Deal</a>
            </div>

            <div class="table-responsive">
                <table class="table align-middle mb-0">
                    <thead class="table-light small text-muted">
                        <tr>
                            <th>Banner</th>
                            <th>Tiêu đề & Mã ưu đãi</th>
                            <th>Thời gian hiệu lực</th>
                            <th>Trạng thái IsActiveNow</th>
                            <th class="text-end">Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var p in popups)
                        {
                            <tr>
                                <td><img src="@p.ImageUrl" class="rounded-3 border" style="width:96px;height:54px;object-fit:cover" alt="" /></td>
                                <td>
                                    <div class="fw-bold small">@(p.Title ?? "Hot Deal CHAM Flowers")</div>
                                    <div class="text-muted small">Mã: <strong class="text-danger">@(p.CouponCode ?? "-")</strong> • SP liên kết: @(p.Product != null ? p.Product.Name : "Tất cả")</div>
                                </td>
                                <td class="small">
                                    @(p.StartDate.HasValue ? p.StartDate.Value.ToString("dd/MM/yyyy") : "Ngay lập tức") → @(p.EndDate.HasValue ? p.EndDate.Value.ToString("dd/MM/yyyy") : "Không giới hạn")
                                </td>
                                <td>
                                    @if (p.IsActiveNow)
                                    {
                                        <span class="badge bg-success">Đang hiển thị (ActiveNow)</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-secondary">Đang tạm dừng / Hết hạn</span>
                                    }
                                </td>
                                <td class="text-end">
                                    <div class="d-inline-flex gap-1">
                                        <a href="@Url.Action("EditHotDealPopup", "HotDeal", new { id = p.Id })" class="btn btn-sm btn-outline-primary">Sửa</a>
                                        <form action="@Url.Action("DeleteHotDealPopup", "HotDeal", new { id = p.Id })" method="post" onsubmit="return confirm('Xác nhận xóa Hot Deal Popup này?');">
                                            @Html.AntiForgeryToken()
                                            <button type="submit" class="btn btn-sm btn-outline-danger">Xóa</button>
                                        </form>
                                    </div>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- 2. Gắn sản phẩm vào danh sách Hot Deal -->
    <div class="col-12">
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3"><i class="bi bi-fire text-danger me-2"></i>Sản Phẩm Đang Khuyến Mãi Hot Deal (@hotProducts.Count mẫu)</h6>

            <form action="@Url.Action("SetProductHotDeal", "HotDeal")" method="post" class="row g-2 align-items-end mb-4 p-3 rounded-3 bg-light border">
                @Html.AntiForgeryToken()
                <input type="hidden" name="isHotDeal" value="true" />
                <div class="col-md-6">
                    <label class="form-label small fw-semibold">Chọn mẫu hoa đưa vào Hot Deal</label>
                    <select name="productId" class="form-select form-select-sm">
                        @foreach (var ap in allProducts)
                        {
                            <option value="@ap.Id">@ap.Name (Giá hiện tại: @AppHelpers.FormatVnd(ap.Price))</option>
                        }
                    </select>
                </div>
                <div class="col-md-4">
                    <label class="form-label small fw-semibold">Giá khuyến mãi mới (Để trống nếu giữ giá hiện tại)</label>
                    <input type="number" name="salePrice" step="1000" class="form-control form-control-sm" placeholder="VD: 499000" />
                </div>
                <div class="col-md-2 d-grid">
                    <button type="submit" class="btn btn-sm btn-danger">+ Thêm Hot Deal</button>
                </div>
            </form>

            <div class="row g-3">
                @foreach (var hp in hotProducts)
                {
                    <div class="col-md-6 col-xl-4">
                        <div class="border rounded-3 p-3 d-flex align-items-center justify-content-between gap-2">
                            <div class="d-flex align-items-center gap-2">
                                <img src="@hp.MainImageUrl" class="rounded-2 border" style="width:52px;height:52px;object-fit:cover" alt="" />
                                <div>
                                    <div class="fw-bold small">@hp.Name</div>
                                    <div class="text-danger fw-bold small">
                                        @AppHelpers.FormatVnd(hp.Price)
                                        @if (hp.DiscountPercent > 0)
                                        {
                                            <span class="badge bg-danger ms-1">-@hp.DiscountPercent%</span>
                                        }
                                    </div>
                                </div>
                            </div>
                            <form action="@Url.Action("SetProductHotDeal", "HotDeal")" method="post">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="productId" value="@hp.Id" />
                                <input type="hidden" name="isHotDeal" value="false" />
                                <button type="submit" class="btn btn-sm btn-outline-secondary" title="Gỡ khỏi Hot Deal">Gỡ</button>
                            </form>
                        </div>
                    </div>
                }
            </div>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Menu\Create.cshtml
```html
@model FlowerShop.Models.MenuItem
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Thêm Mục Menu Mới - CHAM Flowers CMS";
}

<div class="mb-4">
    <a href="@Url.Action("Index", "Menu")" class="text-decoration-none text-muted small">
        <i class="bi bi-arrow-left me-1"></i> Quay lại Danh sách Menu
    </a>
    <h4 class="fw-bold mt-2"><i class="bi bi-plus-circle text-danger me-2"></i>Thêm Mục Menu Điều Hướng Mới</h4>
    <p class="text-muted small">Tạo thêm nút điều hướng mới trên thanh menu của khách hàng (gắn link trang web, biểu tượng và huy hiệu nổi bật).</p>
</div>

<div class="row">
    <div class="col-lg-8">
        <div class="admin-card p-4">
            @using (Html.BeginForm("Create", "Menu", FormMethod.Post))
            {
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(true, "", new { @class = "text-danger mb-3" })

                <!-- Gợi ý chọn nhanh các trang có sẵn -->
                <div class="p-3 bg-light rounded-4 border mb-4">
                    <div class="fw-bold small text-dark mb-2">
                        <i class="bi bi-magic text-danger me-1"></i> Bấm để chọn nhanh các trang phổ biến có sẵn:
                    </div>
                    <div class="d-flex flex-wrap gap-2">
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Trang chủ', '/', 'bi-house-door', '')">
                            <i class="bi bi-house-door me-1"></i> Trang chủ
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Sản phẩm hoa', '/Product', 'bi-flower1', '')">
                            <i class="bi bi-flower1 me-1"></i> Sản phẩm hoa
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Hot Deal', '/HotDeal', 'bi-fire', 'HOT')">
                            <i class="bi bi-fire text-danger me-1"></i> Hot Deal (HOT)
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Giỏ hàng', '/Order/Cart', 'bi-bag-heart', '')">
                            <i class="bi bi-bag-heart me-1"></i> Giỏ hàng
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Tin tức & Cẩm nang', '/Home/News', 'bi-newspaper', '')">
                            <i class="bi bi-newspaper me-1"></i> Tin tức
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Giới thiệu', '/Home/About', 'bi-info-circle', '')">
                            <i class="bi bi-info-circle me-1"></i> Giới thiệu
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Lịch sử đơn hàng', '/Account/History', 'bi-clock-history', '')">
                            <i class="bi bi-clock-history me-1"></i> Lịch sử đơn
                        </button>
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold small">Tên hiển thị trên Menu <span class="text-danger">*</span></label>
                    @Html.TextBoxFor(m => m.Name, new { @class = "form-control", placeholder = "Ví dụ: Trang chủ, Hoa Khai Trương, Giỏ hàng...", id = "txtMenuName", required = "required" })
                    @Html.ValidationMessageFor(m => m.Name, "", new { @class = "text-danger small" })
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold small">Đường dẫn liên kết (URL) <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <span class="input-group-text bg-light"><i class="bi bi-link-45deg"></i></span>
                        @Html.TextBoxFor(m => m.Url, new { @class = "form-control", placeholder = "Ví dụ: / hoặc /Product hoặc /Order/Cart hoặc https://...", id = "txtMenuUrl", required = "required" })
                    </div>
                    <div class="form-text small">
                        - Đường dẫn nội bộ bắt đầu bằng dấu gạch chéo <code>/</code> (ví dụ: <code>/Product</code>, <code>/Order/Cart</code>).<br />
                        - Hoặc đường dẫn trang web ngoài bắt đầu bằng <code>https://...</code>
                    </div>
                    @Html.ValidationMessageFor(m => m.Url, "", new { @class = "text-danger small" })
                </div>

                <div class="row g-3 mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-bold small">Mã Icon Bootstrap (Tùy chọn)</label>
                        <div class="input-group">
                            <span class="input-group-text bg-light" id="previewIconBox"><i class="bi bi-star"></i></span>
                            @Html.TextBoxFor(m => m.Icon, new { @class = "form-control", placeholder = "Ví dụ: bi-fire, bi-flower1, bi-bag-heart...", id = "txtMenuIcon" })
                        </div>
                        <div class="form-text small">Ví dụ: <code>bi-house-door</code>, <code>bi-fire</code>, <code>bi-flower1</code>, <code>bi-bag-heart</code></div>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold small">Huy hiệu (Badge)</label>
                        @Html.TextBoxFor(m => m.BadgeText, new { @class = "form-control", placeholder = "HOT, NEW, SALE...", id = "txtBadgeText" })
                        <div class="form-text small">Để trống nếu không có</div>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold small">Màu huy hiệu</label>
                        @Html.DropDownListFor(m => m.BadgeColor, new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Đỏ (danger)", Value = "danger", Selected = true },
                            new SelectListItem { Text = "Cam vàng (warning)", Value = "warning" },
                            new SelectListItem { Text = "Xanh lá (success)", Value = "success" },
                            new SelectListItem { Text = "Xanh lam (primary)", Value = "primary" },
                            new SelectListItem { Text = "Xanh ngọc (info)", Value = "info" }
                        }, new { @class = "form-select" })
                    </div>
                </div>

                <div class="row g-3 mb-4">
                    <div class="col-md-4">
                        <label class="form-label fw-bold small">Thứ tự hiển thị</label>
                        @Html.TextBoxFor(m => m.DisplayOrder, new { @class = "form-control", type = "number", min = "0" })
                        <div class="form-text small">Số nhỏ hơn sẽ đứng trước</div>
                    </div>
                    <div class="col-md-4 d-flex align-items-center pt-3">
                        <div class="form-check form-switch">
                            @Html.CheckBoxFor(m => m.OpenNewTab, new { @class = "form-check-input", role = "switch", id = "switchNewTab" })
                            <label class="form-check-label fw-semibold small" for="switchNewTab">Mở trong tab mới (_blank)</label>
                        </div>
                    </div>
                    <div class="col-md-4 d-flex align-items-center pt-3">
                        <div class="form-check form-switch">
                            @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input", role = "switch", id = "switchIsActive" })
                            <label class="form-check-label fw-semibold small text-success" for="switchIsActive">Hiển thị ngay trên website</label>
                        </div>
                    </div>
                </div>

                <hr class="my-4" />

                <div class="d-flex justify-content-between align-items-center">
                    <a href="@Url.Action("Index", "Menu")" class="btn btn-light rounded-pill px-4">Hủy bỏ</a>
                    <button type="submit" class="btn btn-danger rounded-pill px-5">
                        <i class="bi bi-check-circle me-1"></i> Lưu Mục Menu
                    </button>
                </div>
            }
        </div>
    </div>

    <!-- Cột hướng dẫn bên phải -->
    <div class="col-lg-4">
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3"><i class="bi bi-question-circle text-primary me-2"></i>Hướng dẫn gắn Link trang</h6>
            <div class="small text-muted space-y-2">
                <p><strong>1. Gắn link trang nội bộ:</strong><br />
                Chỉ cần điền đường dẫn sau tên miền. Ví dụ:<br />
                - Trang chủ: <code>/</code><br />
                - Xem hoa: <code>/Product</code><br />
                - Giỏ hàng: <code>/Order/Cart</code><br />
                - Hot Deal: <code>/HotDeal</code><br />
                - Cẩm nang: <code>/Home/News</code><br />
                - Giới thiệu: <code>/Home/About</code><br />
                - Lọc theo loại hoa: <code>/Product?flowerCategoryId=1</code>
                </p>
                <p><strong>2. Gắn link bên ngoài:</strong><br />
                Nếu muốn trỏ tới trang Facebook, Zalo hoặc Shopee, bạn điền đầy đủ <code>https://...</code> và nên tích chọn <strong>"Mở trong tab mới"</strong> để khách không bị thoát khỏi website.
                </p>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        function setPreset(name, url, icon, badge) {
            document.getElementById('txtMenuName').value = name;
            document.getElementById('txtMenuUrl').value = url;
            document.getElementById('txtMenuIcon').value = icon;
            document.getElementById('txtBadgeText').value = badge;
            updateIconPreview(icon);
        }

        function updateIconPreview(icon) {
            var box = document.getElementById('previewIconBox');
            if (box) {
                box.innerHTML = '<i class="bi ' + (icon || 'bi-star') + '"></i>';
            }
        }

        document.getElementById('txtMenuIcon').addEventListener('input', function () {
            updateIconPreview(this.value.trim());
        });
    </script>
}
```

### File: FlowerShop/Views\Menu\Edit.cshtml
```html
@model FlowerShop.Models.MenuItem
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Chỉnh sửa Mục Menu: " + Model.Name + " - CHAM Flowers CMS";
}

<div class="mb-4">
    <a href="@Url.Action("Index", "Menu")" class="text-decoration-none text-muted small">
        <i class="bi bi-arrow-left me-1"></i> Quay lại Danh sách Menu
    </a>
    <h4 class="fw-bold mt-2"><i class="bi bi-pencil-square text-danger me-2"></i>Chỉnh Sửa Mục Menu: "@Model.Name"</h4>
    <p class="text-muted small">Thay đổi tên, liên kết đường dẫn, thứ tự hoặc biểu tượng của mục menu này.</p>
</div>

<div class="row">
    <div class="col-lg-8">
        <div class="admin-card p-4">
            @using (Html.BeginForm("Edit", "Menu", FormMethod.Post))
            {
                @Html.AntiForgeryToken()
                @Html.HiddenFor(m => m.Id)
                @Html.HiddenFor(m => m.CreatedAt)
                @Html.ValidationSummary(true, "", new { @class = "text-danger mb-3" })

                <!-- Gợi ý chọn nhanh các trang có sẵn -->
                <div class="p-3 bg-light rounded-4 border mb-4">
                    <div class="fw-bold small text-dark mb-2">
                        <i class="bi bi-magic text-danger me-1"></i> Bấm để chọn nhanh các trang phổ biến có sẵn:
                    </div>
                    <div class="d-flex flex-wrap gap-2">
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Trang chủ', '/', 'bi-house-door', '')">
                            <i class="bi bi-house-door me-1"></i> Trang chủ
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Sản phẩm hoa', '/Product', 'bi-flower1', '')">
                            <i class="bi bi-flower1 me-1"></i> Sản phẩm hoa
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Hot Deal', '/HotDeal', 'bi-fire', 'HOT')">
                            <i class="bi bi-fire text-danger me-1"></i> Hot Deal (HOT)
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Giỏ hàng', '/Order/Cart', 'bi-bag-heart', '')">
                            <i class="bi bi-bag-heart me-1"></i> Giỏ hàng
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Tin tức & Cẩm nang', '/Home/News', 'bi-newspaper', '')">
                            <i class="bi bi-newspaper me-1"></i> Tin tức
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Giới thiệu', '/Home/About', 'bi-info-circle', '')">
                            <i class="bi bi-info-circle me-1"></i> Giới thiệu
                        </button>
                        <button type="button" class="btn btn-sm btn-white border rounded-pill shadow-xs" onclick="setPreset('Lịch sử đơn hàng', '/Account/History', 'bi-clock-history', '')">
                            <i class="bi bi-clock-history me-1"></i> Lịch sử đơn
                        </button>
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold small">Tên hiển thị trên Menu <span class="text-danger">*</span></label>
                    @Html.TextBoxFor(m => m.Name, new { @class = "form-control", placeholder = "Ví dụ: Trang chủ, Hoa Khai Trương, Giỏ hàng...", id = "txtMenuName", required = "required" })
                    @Html.ValidationMessageFor(m => m.Name, "", new { @class = "text-danger small" })
                </div>

                <div class="mb-3">
                    <label class="form-label fw-bold small">Đường dẫn liên kết (URL) <span class="text-danger">*</span></label>
                    <div class="input-group">
                        <span class="input-group-text bg-light"><i class="bi bi-link-45deg"></i></span>
                        @Html.TextBoxFor(m => m.Url, new { @class = "form-control", placeholder = "Ví dụ: / hoặc /Product hoặc /Order/Cart hoặc https://...", id = "txtMenuUrl", required = "required" })
                    </div>
                    <div class="form-text small">
                        - Đường dẫn nội bộ bắt đầu bằng dấu gạch chéo <code>/</code> (ví dụ: <code>/Product</code>, <code>/Order/Cart</code>).<br />
                        - Hoặc đường dẫn trang web ngoài bắt đầu bằng <code>https://...</code>
                    </div>
                    @Html.ValidationMessageFor(m => m.Url, "", new { @class = "text-danger small" })
                </div>

                <div class="row g-3 mb-3">
                    <div class="col-md-6">
                        <label class="form-label fw-bold small">Mã Icon Bootstrap (Tùy chọn)</label>
                        <div class="input-group">
                            <span class="input-group-text bg-light" id="previewIconBox"><i class="bi @(string.IsNullOrEmpty(Model.Icon) ? "bi-star" : Model.Icon)"></i></span>
                            @Html.TextBoxFor(m => m.Icon, new { @class = "form-control", placeholder = "Ví dụ: bi-fire, bi-flower1, bi-bag-heart...", id = "txtMenuIcon" })
                        </div>
                        <div class="form-text small">Ví dụ: <code>bi-house-door</code>, <code>bi-fire</code>, <code>bi-flower1</code>, <code>bi-bag-heart</code></div>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold small">Huy hiệu (Badge)</label>
                        @Html.TextBoxFor(m => m.BadgeText, new { @class = "form-control", placeholder = "HOT, NEW, SALE...", id = "txtBadgeText" })
                        <div class="form-text small">Để trống nếu không có</div>
                    </div>
                    <div class="col-md-3">
                        <label class="form-label fw-bold small">Màu huy hiệu</label>
                        @Html.DropDownListFor(m => m.BadgeColor, new List<SelectListItem>
                        {
                            new SelectListItem { Text = "Đỏ (danger)", Value = "danger", Selected = (Model.BadgeColor == "danger") },
                            new SelectListItem { Text = "Cam vàng (warning)", Value = "warning", Selected = (Model.BadgeColor == "warning") },
                            new SelectListItem { Text = "Xanh lá (success)", Value = "success", Selected = (Model.BadgeColor == "success") },
                            new SelectListItem { Text = "Xanh lam (primary)", Value = "primary", Selected = (Model.BadgeColor == "primary") },
                            new SelectListItem { Text = "Xanh ngọc (info)", Value = "info", Selected = (Model.BadgeColor == "info") }
                        }, new { @class = "form-select" })
                    </div>
                </div>

                <div class="row g-3 mb-4">
                    <div class="col-md-4">
                        <label class="form-label fw-bold small">Thứ tự hiển thị</label>
                        @Html.TextBoxFor(m => m.DisplayOrder, new { @class = "form-control", type = "number", min = "0" })
                        <div class="form-text small">Số nhỏ hơn sẽ đứng trước</div>
                    </div>
                    <div class="col-md-4 d-flex align-items-center pt-3">
                        <div class="form-check form-switch">
                            @Html.CheckBoxFor(m => m.OpenNewTab, new { @class = "form-check-input", role = "switch", id = "switchNewTab" })
                            <label class="form-check-label fw-semibold small" for="switchNewTab">Mở trong tab mới (_blank)</label>
                        </div>
                    </div>
                    <div class="col-md-4 d-flex align-items-center pt-3">
                        <div class="form-check form-switch">
                            @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input", role = "switch", id = "switchIsActive" })
                            <label class="form-check-label fw-semibold small text-success" for="switchIsActive">Hiển thị trên website</label>
                        </div>
                    </div>
                </div>

                <hr class="my-4" />

                <div class="d-flex justify-content-between align-items-center">
                    <a href="@Url.Action("Index", "Menu")" class="btn btn-light rounded-pill px-4">Hủy bỏ</a>
                    <button type="submit" class="btn btn-danger rounded-pill px-5">
                        <i class="bi bi-check-circle me-1"></i> Cập Nhật Mục Menu
                    </button>
                </div>
            }
        </div>
    </div>

    <!-- Cột hướng dẫn bên phải -->
    <div class="col-lg-4">
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3"><i class="bi bi-question-circle text-primary me-2"></i>Hướng dẫn nhanh</h6>
            <div class="small text-muted space-y-2">
                <p><strong>- Đổi thứ tự:</strong> Nhập số thứ tự mong muốn (ví dụ 1, 2, 3...) hoặc sử dụng nút mũi tên ▲ ▼ ở màn hình danh sách.</p>
                <p><strong>- Ẩn tạm thời:</strong> Bỏ tích ô <em>"Hiển thị trên website"</em> nếu bạn muốn tạm ẩn menu mà không cần xóa đi.</p>
                <p><strong>- Gắn link sản phẩm theo bộ lọc:</strong> Bạn có thể dán đường dẫn kèm tham số, ví dụ <code>/Product?sort=bestseller</code> để dẫn khách thẳng vào danh sách hoa bán chạy!</p>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        function setPreset(name, url, icon, badge) {
            document.getElementById('txtMenuName').value = name;
            document.getElementById('txtMenuUrl').value = url;
            document.getElementById('txtMenuIcon').value = icon;
            document.getElementById('txtBadgeText').value = badge;
            updateIconPreview(icon);
        }

        function updateIconPreview(icon) {
            var box = document.getElementById('previewIconBox');
            if (box) {
                box.innerHTML = '<i class="bi ' + (icon || 'bi-star') + '"></i>';
            }
        }

        document.getElementById('txtMenuIcon').addEventListener('input', function () {
            updateIconPreview(this.value.trim());
        });
    </script>
}
```

### File: FlowerShop/Views\Menu\Index.cshtml
```html
@model List<FlowerShop.Models.MenuItem>
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Quản lý Menu Điều Hướng Website - CHAM Flowers CMS";
}

<div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
    <div>
        <h4 class="fw-bold mb-1"><i class="bi bi-menu-button-wide text-danger me-2"></i>Quản Lý Menu Điều Hướng</h4>
        <p class="text-muted small mb-0">Tự do thêm trang mới, gán liên kết (Trang chủ, Giỏ hàng, Sản phẩm...), sắp xếp thứ tự và bật/tắt hiển thị trên website.</p>
    </div>
    <div class="d-flex gap-2">
        <a href="@Url.Action("Index", "Home")" target="_blank" class="btn btn-outline-secondary rounded-pill px-3">
            <i class="bi bi-eye me-1"></i> Xem Website Thực Tế
        </a>
        <a href="@Url.Action("Create", "Menu")" class="btn btn-danger rounded-pill px-4">
            <i class="bi bi-plus-lg me-1"></i> + Thêm Mục Menu Mới
        </a>
    </div>
</div>

<!-- Khung Xem trước Thanh Menu Thực tế -->
<div class="admin-card p-4 mb-4">
    <div class="d-flex justify-content-between align-items-center mb-2">
        <h6 class="fw-bold mb-0 text-muted" style="font-size:0.85rem">
            <i class="bi bi-display me-2 text-primary"></i>XEM TRƯỚC THANH MENU ĐANG HOẠT ĐỘNG TRÊN WEBSITE:
        </h6>
        <span class="badge bg-light text-dark border">Cập nhật tự động theo CSDL</span>
    </div>
    <div class="p-3 rounded-3 border bg-white d-flex flex-wrap align-items-center gap-2">
        <span class="fw-bold text-danger me-3"><i class="bi bi-flower1"></i> CHAM Flowers</span>
        @if (Model != null && Model.Any(m => m.IsActive))
        {
            foreach (var item in Model.Where(m => m.IsActive).OrderBy(m => m.DisplayOrder))
            {
                <a href="@item.Url" target="@(item.OpenNewTab ? "_blank" : "_self")" class="btn btn-sm btn-light border rounded-pill px-3 text-dark text-decoration-none d-inline-flex align-items-center gap-1">
                    @if (!string.IsNullOrEmpty(item.Icon))
                    {
                        <i class="bi @item.Icon text-muted"></i>
                    }
                    <span class="fw-semibold">@item.Name</span>
                    @if (!string.IsNullOrEmpty(item.BadgeText))
                    {
                        <span class="badge bg-@(string.IsNullOrEmpty(item.BadgeColor) ? "danger" : item.BadgeColor) rounded-pill ms-1" style="font-size:0.62rem">@item.BadgeText</span>
                    }
                </a>
            }
        }
        else
        {
            <span class="text-muted small fst-italic">Chưa có mục menu nào đang kích hoạt.</span>
        }
    </div>
</div>

<!-- Danh sách các Mục Menu -->
<div class="admin-card p-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold mb-0"><i class="bi bi-list-nested me-2 text-danger"></i>Danh Sách Mục Menu (@Model.Count mục)</h6>
        <span class="text-muted small">Mẹo: Bấm mũi tên ▲ ▼ để đổi thứ tự xuất hiện trên website.</span>
    </div>

    <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
            <thead class="table-light small text-muted">
                <tr>
                    <th style="width: 110px;">Thứ tự</th>
                    <th>Tên hiển thị & Biểu tượng</th>
                    <th>Đường dẫn liên kết (URL)</th>
                    <th style="width: 120px;">Mở tab mới</th>
                    <th style="width: 130px;">Trạng thái</th>
                    <th style="width: 180px;" class="text-end">Hành động</th>
                </tr>
            </thead>
            <tbody>
                @if (Model == null || !Model.Any())
                {
                    <tr>
                        <td colspan="6" class="text-center py-5 text-muted">
                            <i class="bi bi-inbox fs-2 d-block mb-2 text-secondary"></i>
                            Chưa có mục menu nào trong hệ thống.
                            <div class="mt-2">
                                <a href="@Url.Action("Create", "Menu")" class="btn btn-sm btn-danger rounded-pill px-3">+ Thêm mục menu đầu tiên</a>
                            </div>
                        </td>
                    </tr>
                }
                else
                {
                    for (int i = 0; i < Model.Count; i++)
                    {
                        var m = Model[i];
                        <tr>
                            <td>
                                <div class="d-flex align-items-center gap-1">
                                    <span class="badge bg-dark rounded-pill" style="min-width:28px">@m.DisplayOrder</span>
                                    <div class="btn-group-vertical btn-group-sm">
                                        @if (i > 0)
                                        {
                                            <form action="@Url.Action("Move", "Menu")" method="post" class="d-inline">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="id" value="@m.Id" />
                                                <input type="hidden" name="direction" value="up" />
                                                <button type="submit" class="btn btn-outline-secondary p-0 px-1" title="Đưa lên trên" style="font-size:0.65rem;line-height:1">▲</button>
                                            </form>
                                        }
                                        @if (i < Model.Count - 1)
                                        {
                                            <form action="@Url.Action("Move", "Menu")" method="post" class="d-inline">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="id" value="@m.Id" />
                                                <input type="hidden" name="direction" value="down" />
                                                <button type="submit" class="btn btn-outline-secondary p-0 px-1" title="Đưa xuống dưới" style="font-size:0.65rem;line-height:1">▼</button>
                                            </form>
                                        }
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    @if (!string.IsNullOrEmpty(m.Icon))
                                    {
                                        <span class="badge bg-light text-primary border p-2 rounded-3 fs-6">
                                            <i class="bi @m.Icon"></i>
                                        </span>
                                    }
                                    <div>
                                        <div class="fw-bold">@m.Name</div>
                                        @if (!string.IsNullOrEmpty(m.BadgeText))
                                        {
                                            <span class="badge bg-@(string.IsNullOrEmpty(m.BadgeColor) ? "danger" : m.BadgeColor) rounded-pill" style="font-size:0.65rem">
                                                @m.BadgeText
                                            </span>
                                        }
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <code class="text-primary bg-light px-2 py-1 rounded">@m.Url</code>
                                    <a href="@m.Url" target="_blank" class="text-muted" title="Mở thử đường dẫn này">
                                        <i class="bi bi-box-arrow-up-right" style="font-size:0.8rem"></i>
                                    </a>
                                </div>
                            </td>
                            <td>
                                @if (m.OpenNewTab)
                                {
                                    <span class="badge bg-info-subtle text-info border border-info rounded-pill">Tab mới (_blank)</span>
                                }
                                else
                                {
                                    <span class="text-muted small">Cùng tab</span>
                                }
                            </td>
                            <td>
                                <form action="@Url.Action("ToggleActive", "Menu")" method="post" class="d-inline">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="id" value="@m.Id" />
                                    @if (m.IsActive)
                                    {
                                        <button type="submit" class="btn btn-sm btn-success rounded-pill px-3 py-1" title="Bấm để ẩn menu này khỏi website">
                                            <i class="bi bi-check-circle me-1"></i> Đang hiện
                                        </button>
                                    }
                                    else
                                    {
                                        <button type="submit" class="btn btn-sm btn-outline-secondary rounded-pill px-3 py-1" title="Bấm để hiển thị lại menu này">
                                            <i class="bi bi-eye-slash me-1"></i> Đang ẩn
                                        </button>
                                    }
                                </form>
                            </td>
                            <td class="text-end">
                                <a href="@Url.Action("Edit", "Menu", new { id = m.Id })" class="btn btn-sm btn-outline-primary rounded-pill px-3 me-1">
                                    <i class="bi bi-pencil-square me-1"></i> Sửa
                                </a>
                                <form action="@Url.Action("Delete", "Menu")" method="post" class="d-inline" onsubmit="return confirm('Bạn có chắc muốn xóa mục menu &quot;@m.Name&quot;?');">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="id" value="@m.Id" />
                                    <button type="submit" class="btn btn-sm btn-outline-danger rounded-pill px-2" title="Xóa menu">
                                        <i class="bi bi-trash"></i>
                                    </button>
                                </form>
                            </td>
                        </tr>
                    }
                }
            </tbody>
        </table>
    </div>
</div>
```

### File: FlowerShop/Views\Order\AdminCreate.cshtml
```html
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Tạo đơn hàng mới (Tại quầy / Điện thoại) - CHAM Flowers Executive CMS";
    var products = ViewBag.Products as List<FlowerShop.Models.Product> ?? new List<FlowerShop.Models.Product>();
    var customers = ViewBag.Customers as List<FlowerShop.Models.User> ?? new List<FlowerShop.Models.User>();
}

<div class="admin-card p-4" style="max-width:740px">
    <h6 class="fw-bold mb-3">Khởi Tạo Đơn Hàng Mới Tại Quầy</h6>
    <form action="@Url.Action("AdminCreate", "Order")" method="post">
        @Html.AntiForgeryToken()
        <div class="row g-3">
            <div class="col-md-6">
                <label class="form-label small fw-bold">Liên kết tài khoản khách (Tùy chọn)</label>
                <select name="userId" class="form-select" onchange="fillCustomerInfo(this)">
                    <option value="">-- Khách lẻ vãng lai --</option>
                    @foreach (var u in customers)
                    {
                        <option value="@u.Id" data-name="@u.FullName" data-phone="@u.Phone" data-addr="@u.Address">@u.FullName (@u.Phone)</option>
                    }
                </select>
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Phương thức thanh toán</label>
                <select name="paymentMethod" class="form-select">
                    <option value="COD">Tiền mặt / COD</option>
                    <option value="VNPay">Chuyển khoản / VNPay</option>
                </select>
            </div>

            <div class="col-md-6">
                <label class="form-label small fw-bold">Tên người nhận *</label>
                <input type="text" id="rcName" name="recipientName" class="form-control" required />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Số điện thoại nhận *</label>
                <input type="text" id="rcPhone" name="recipientPhone" class="form-control" required />
            </div>
            <div class="col-12">
                <label class="form-label small fw-bold">Địa chỉ giao hàng *</label>
                <input type="text" id="rcAddr" name="deliveryAddress" class="form-control" required />
            </div>

            <div class="col-md-8">
                <label class="form-label small fw-bold">Chọn mẫu hoa *</label>
                <select name="productId" class="form-select" required>
                    @foreach (var p in products)
                    {
                        <option value="@p.Id">@p.Name — @AppHelpers.FormatVnd(p.Price) (Kho còn: @p.StockQuantity)</option>
                    }
                </select>
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Số lượng *</label>
                <input type="number" name="quantity" value="1" min="1" class="form-control" required />
            </div>

            <div class="col-12">
                <label class="form-label small fw-bold">Ghi chú / Nội dung thiệp</label>
                <input type="text" name="note" class="form-control" placeholder="Lời chúc in trên banner/thiệp..." />
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Tạo đơn hàng</button>
            <a href="@Url.Action("AdminIndex", "Order")" class="btn btn-light border rounded-pill px-4">Hủy</a>
        </div>
    </form>
</div>

@section Scripts {
    <script>
        function fillCustomerInfo(sel) {
            var opt = sel.options[sel.selectedIndex];
            if (opt.value) {
                document.getElementById('rcName').value = opt.getAttribute('data-name') || '';
                document.getElementById('rcPhone').value = opt.getAttribute('data-phone') || '';
                document.getElementById('rcAddr').value = opt.getAttribute('data-addr') || '';
            }
        }
    </script>
}
```

### File: FlowerShop/Views\Order\AdminDetails.cshtml
```html
@model FlowerShop.Models.Order
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = string.Format("Xử lý Đơn hàng #{0} - CHAM Flowers Executive CMS", Model.Id);
    var statuses = new[] { "Chờ xác nhận", "Đang xử lý", "Đang giao", "Hoàn thành", "Đã hủy" };
}

<div class="mb-3 d-flex justify-content-between align-items-center">
    <a href="@Url.Action("AdminIndex", "Order")" class="btn btn-sm btn-light border rounded-pill"><i class="bi bi-arrow-left me-1"></i>Danh sách đơn hàng</a>
    @if (Model.Status != "Hoàn thành" && Model.Status != "Đang giao")
    {
        <form action="@Url.Action("DeleteOrder", "Order")" method="post" onsubmit="return confirm('Xóa đơn hàng #@Model.Id khỏi CSDL?');">
            @Html.AntiForgeryToken()
            <input type="hidden" name="id" value="@Model.Id" />
            <button type="submit" class="btn btn-sm btn-outline-danger rounded-pill"><i class="bi bi-trash me-1"></i>Xóa đơn</button>
        </form>
    }
</div>

<div class="row g-4">
    <!-- Chi tiết sản phẩm & Xóa sản phẩm khỏi đơn -->
    <div class="col-lg-7">
        <div class="admin-card p-4 mb-4">
            <h6 class="fw-bold mb-3">Danh Sách Sản Phẩm Hoa Trong Đơn #@Model.Id</h6>
            <div class="table-responsive">
                <table class="table align-middle mb-0">
                    <thead class="table-light small">
                        <tr><th>Sản phẩm</th><th>Đơn giá</th><th>SL</th><th>Thành tiền</th><th></th></tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model.OrderItems)
                        {
                            <tr>
                                <td>
                                    <div class="d-flex align-items-center gap-2">
                                        <img src="@(item.Product != null && !string.IsNullOrEmpty(item.Product.MainImageUrl) ? item.Product.MainImageUrl : "/images/samflora/b10-main.jpg")"
                                             class="rounded-2 border" style="width:44px;height:44px;object-fit:cover" alt="" />
                                        <div>
                                            <div class="fw-semibold small">@(item.Product != null ? item.Product.Name : "Sản phẩm")</div>
                                            <div class="text-muted" style="font-size:0.72rem">Tồn kho hiện tại: @(item.Product != null ? item.Product.StockQuantity : 0)</div>
                                        </div>
                                    </div>
                                </td>
                                <td class="small">@AppHelpers.FormatVnd(item.UnitPrice)</td>
                                <td class="fw-bold">@item.Quantity</td>
                                <td class="fw-bold text-danger small">@AppHelpers.FormatVnd(item.UnitPrice * item.Quantity)</td>
                                <td class="text-end">
                                    @if (Model.Status != "Hoàn thành" && Model.OrderItems.Count > 1)
                                    {
                                        <form action="@Url.Action("RemoveOrderItem", "Order")" method="post" onsubmit="return confirm('Xóa món này khỏi đơn hàng?');">
                                            @Html.AntiForgeryToken()
                                            <input type="hidden" name="orderId" value="@Model.Id" />
                                            <input type="hidden" name="orderItemId" value="@item.Id" />
                                            <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-x-lg"></i></button>
                                        </form>
                                    }
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
            <hr />
            <div class="d-flex justify-content-between small mb-1">
                <span>Phí giao hàng (@Model.ShippingCarrier):</span>
                <span>@AppHelpers.FormatVnd(Model.ShippingFee)</span>
            </div>
            @if (Model.DiscountAmount > 0)
            {
                <div class="d-flex justify-content-between small mb-1 text-success">
                    <span>Mã giảm giá (@Model.CouponCode):</span>
                    <span>-@AppHelpers.FormatVnd(Model.DiscountAmount)</span>
                </div>
            }
            <div class="d-flex justify-content-between border-top pt-2 mt-2">
                <span class="fw-bold">Tổng cộng:</span>
                <span class="h5 fw-bold text-danger mb-0">@AppHelpers.FormatVnd(Model.Total)</span>
            </div>
        </div>

        <!-- Cập nhật địa chỉ giao hàng -->
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3">Thông Tin Giao Nhận & Lời Chúc Trên Thiệp</h6>
            <form action="@Url.Action("UpdateShippingInfo", "Order")" method="post">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="@Model.Id" />
                <div class="row g-3">
                    <div class="col-md-6">
                        <label class="form-label small fw-semibold">Người nhận</label>
                        <input type="text" name="recipientName" value="@Model.RecipientName" class="form-control form-control-sm" required />
                    </div>
                    <div class="col-md-6">
                        <label class="form-label small fw-semibold">Số điện thoại</label>
                        <input type="text" name="recipientPhone" value="@Model.RecipientPhone" class="form-control form-control-sm" required />
                    </div>
                    <div class="col-12">
                        <label class="form-label small fw-semibold">Địa chỉ giao hoa</label>
                        <input type="text" name="deliveryAddress" value="@Model.DeliveryAddress" class="form-control form-control-sm" required />
                    </div>
                    <div class="col-12">
                        <label class="form-label small fw-semibold">Ghi chú / Nội dung thiệp</label>
                        <textarea name="note" rows="2" class="form-control form-control-sm">@Model.Note</textarea>
                    </div>
                </div>
                <button type="submit" class="btn btn-sm btn-dark rounded-pill px-3 mt-3">Lưu thông tin giao nhận</button>
            </form>
        </div>
    </div>

    <!-- Cập nhật Trạng thái Đơn hàng -->
    <div class="col-lg-5">
        <div class="admin-card p-4 mb-4">
            <h6 class="fw-bold mb-3">Điều Phối Trạng Thái Đơn Hàng</h6>
            <div class="p-3 rounded-3 bg-light mb-3 small">
                <div>Trạng thái hiện tại: <strong class="text-danger">@Model.Status</strong></div>
                <div>Thanh toán: <strong>@Model.PaymentMethod</strong> (@(Model.IsPaid ? "Đã thu tiền" : "Chưa thu tiền"))</div>
                <div>Đã trừ tồn kho: <strong>@(Model.StockDeducted ? "Đã trừ kho tự động" : "Chưa trừ")</strong></div>
            </div>

            <form action="@Url.Action("UpdateStatus", "Order")" method="post">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="@Model.Id" />
                <label class="form-label small fw-semibold">Chuyển sang trạng thái mới</label>
                <select name="newStatus" class="form-select mb-3">
                    @foreach (var s in statuses)
                    {
                        <option value="@s" @Html.Raw(Model.Status == s ? "selected=\"selected\"" : "")>@s</option>
                    }
                </select>
                <button type="submit" class="btn btn-danger w-100 rounded-pill fw-semibold">
                    <i class="bi bi-check2-circle me-1"></i> Cập nhật trạng thái
                </button>
                <div class="text-muted mt-2" style="font-size:0.75rem">
                    * Khi chuyển sang <strong>Hoàn thành</strong>, hệ thống tự động trừ tồn kho sản phẩm, ghi nhận thanh toán và cộng chi tiêu nâng hạng VIP cho khách hàng.
                </div>
            </form>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Order\AdminIndex.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.Order>
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Quản lý Đơn đặt hàng - CHAM Flowers Executive CMS";
    string curStatus = ViewBag.Status as string;
    var statuses = new[] { "Chờ xác nhận", "Đang xử lý", "Đang giao", "Hoàn thành", "Đã hủy" };
}

<div class="admin-card p-3 px-4 mb-4 d-flex flex-wrap justify-content-between align-items-center gap-3">
    <div class="d-flex flex-wrap gap-2">
        <a href="@Url.Action("AdminIndex", "Order")" class="admin-filter-pill @(string.IsNullOrEmpty(curStatus) ? "active" : "")">Tất cả</a>
        @foreach (var s in statuses)
        {
            <a href="@Url.Action("AdminIndex", "Order", new { status = s })" class="admin-filter-pill @(curStatus == s ? "active" : "")">@s</a>
        }
    </div>

    <div class="d-flex flex-wrap align-items-center gap-2">
        <form action="@Url.Action("AdminIndex", "Order")" method="get" class="d-flex align-items-center gap-2">
            <input type="hidden" name="status" value="@curStatus" />
            <div class="admin-quick-search" style="width:220px">
                <i class="bi bi-search text-muted me-2" style="font-size:0.82rem"></i>
                <input type="text" name="search" value="@ViewBag.Search" placeholder="Mã đơn, tên, SĐT..." />
            </div>
            <button type="submit" class="btn btn-sm btn-dark rounded-pill px-3" style="height:38px">Tìm</button>
        </form>
        <a href="@Url.Action("AdminCreate", "Order")" class="btn btn-sm btn-admin-primary text-nowrap">
            <i class="bi bi-plus-lg me-1"></i>Tạo đơn tại quầy
        </a>
    </div>
</div>

<div class="admin-card overflow-hidden">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead>
                <tr>
                    <th class="ps-4">Mã đơn</th>
                    <th>Người nhận hoa</th>
                    <th>Thời gian đặt</th>
                    <th>Phương thức TT</th>
                    <th>Tổng thanh toán</th>
                    <th>Trạng thái</th>
                    <th class="text-end pe-4">Thao tác</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var o in Model)
                {
                    string badge = "bg-warning-subtle text-warning-emphasis";
                    if (o.Status == "Hoàn thành") { badge = "bg-success-subtle text-success"; }
                    else if (o.Status == "Đang giao") { badge = "bg-info-subtle text-info-emphasis"; }
                    else if (o.Status == "Đang xử lý") { badge = "bg-primary-subtle text-primary"; }
                    else if (o.Status == "Đã hủy") { badge = "bg-secondary-subtle text-secondary"; }

                    string rInit = AppHelpers.GetInitials(o.RecipientName);
                    <tr>
                        <td class="ps-4 fw-bold text-dark">#@o.Id</td>
                        <td>
                            <div class="d-flex align-items-center gap-2">
                                <span class="customer-avatar-circle" style="width:34px;height:34px;font-size:0.76rem">@rInit</span>
                                <div>
                                    <div class="fw-bold small text-dark">@o.RecipientName</div>
                                    <div class="text-muted" style="font-size:0.75rem"><i class="bi bi-telephone me-1"></i>@o.RecipientPhone</div>
                                </div>
                            </div>
                        </td>
                        <td class="small text-muted">@o.CreatedAt.ToString("dd/MM/yyyy HH:mm")</td>
                        <td class="small">
                            <span class="fw-semibold text-dark">@o.PaymentMethod</span>
                            <span class="badge @(o.IsPaid ? "bg-success-subtle text-success" : "bg-light text-muted border") ms-1">
                                @(o.IsPaid ? "Đã TT" : "Chưa TT")
                            </span>
                        </td>
                        <td class="fw-bold text-danger small">@AppHelpers.FormatVnd(o.Total)</td>
                        <td><span class="badge @badge">@o.Status</span></td>
                        <td class="text-end pe-4">
                            <a href="@Url.Action("AdminDetails", "Order", new { id = o.Id })" class="btn btn-sm btn-outline-primary rounded-pill px-3">
                                <i class="bi bi-eye me-1"></i>Chi tiết & Duyệt
                            </a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

@if (Model.PageCount > 1)
{
    <nav class="mt-4 d-flex justify-content-center">
        <ul class="pagination pagination-sm">
            @for (int i = 1; i <= Model.PageCount; i++)
            {
                <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                    <a class="page-link" href="@Url.Action("AdminIndex", "Order", new { page = i, status = curStatus, search = ViewBag.Search })">@i</a>
                </li>
            }
        </ul>
    </nav>
}
```

### File: FlowerShop/Views\Order\Details.cshtml
```html
@model FlowerShop.Models.Order
@using FlowerShop.Services
@{
    ViewBag.Title = string.Format("Chi tiết đơn hàng #{0} - CHAM Flowers", Model.Id);
}

<div class="container py-4" style="max-width:900px">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <a href="@Url.Action("MyOrders", "Order")" class="text-decoration-none text-muted small"><i class="bi bi-arrow-left me-1"></i>Quay lại danh sách đơn</a>
        @if (Model.Status == "Chờ xác nhận")
        {
            <form action="@Url.Action("CancelOrder", "Order")" method="post" onsubmit="return confirm('Xác nhận hủy đơn hàng #@Model.Id?');">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="@Model.Id" />
                <button type="submit" class="btn btn-sm btn-outline-danger rounded-pill"><i class="bi bi-x-circle me-1"></i>Hủy đơn hàng</button>
            </form>
        }
    </div>

    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm">
        <div class="d-flex flex-wrap justify-content-between align-items-start border-bottom pb-3 mb-4">
            <div>
                <h2 class="h4 fw-bold mb-1">Hóa Đơn Đặt Hoa #@Model.Id</h2>
                <div class="text-muted small">Ngày đặt: @Model.CreatedAt.ToString("HH:mm dd/MM/yyyy")</div>
            </div>
            <div class="text-end">
                <span class="badge bg-danger px-3 py-2 rounded-pill fs-6">@Model.Status</span>
                <div class="small mt-1 text-muted">
                    Thanh toán: <strong>@Model.PaymentMethod</strong> (@(Model.IsPaid ? "Đã thanh toán" : "Chưa thanh toán"))
                </div>
                @if (!string.IsNullOrEmpty(Model.VnPayTransactionNo))
                {
                    <div class="small text-success">Mã GD VNPay: <strong>@Model.VnPayTransactionNo</strong></div>
                }
            </div>
        </div>

        <div class="row g-4 mb-4">
            <div class="col-md-6">
                <h6 class="fw-bold text-uppercase small text-muted">Thông tin người nhận hoa</h6>
                <div class="fw-bold">@Model.RecipientName</div>
                <div class="small">SĐT: @Model.RecipientPhone</div>
                <div class="small">Địa chỉ: @Model.DeliveryAddress</div>
            </div>
            <div class="col-md-6">
                <h6 class="fw-bold text-uppercase small text-muted">Thông tin vận chuyển & Ghi chú</h6>
                <div class="small">Đơn vị giao: <strong>@Model.ShippingCarrier</strong></div>
                <div class="small">Thời gian giao dự kiến: @Model.DeliveryDate.ToString("HH:mm dd/MM/yyyy")</div>
                @if (!string.IsNullOrEmpty(Model.Note))
                {
                    <div class="small mt-1 p-2 rounded-3 bg-light">Lời nhắn/Thiệp: <em>"@Model.Note"</em></div>
                }
            </div>
        </div>

        <div class="table-responsive mb-3">
            <table class="table align-middle">
                <thead class="table-light small">
                    <tr>
                        <th>Sản phẩm hoa</th>
                        <th>Đơn giá</th>
                        <th>SL</th>
                        <th class="text-end">Thành tiền</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model.OrderItems)
                    {
                        <tr>
                            <td>
                                <div class="d-flex align-items-center gap-2">
                                    <img src="@(item.Product != null && !string.IsNullOrEmpty(item.Product.MainImageUrl) ? item.Product.MainImageUrl : "/images/samflora/b10-main.jpg")"
                                         class="rounded-3 border" style="width:48px;height:48px;object-fit:cover" alt="" />
                                    <div>
                                        <a href="@Url.Action("Details", "Product", new { id = item.ProductId })"
                                           class="fw-semibold text-dark text-decoration-none">@(item.Product != null ? item.Product.Name : "Sản phẩm")</a>
                                        @if (Model.Status == "Hoàn thành")
                                        {
                                            <div>
                                                <a href="@Url.Action("Details", "Product", new { id = item.ProductId })"
                                                   class="badge bg-warning-subtle text-dark text-decoration-none mt-1">
                                                    <i class="bi bi-star-fill text-warning"></i> Đánh giá sản phẩm
                                                </a>
                                            </div>
                                        }
                                    </div>
                                </div>
                            </td>
                            <td>@AppHelpers.FormatVnd(item.UnitPrice)</td>
                            <td>@item.Quantity</td>
                            <td class="text-end fw-bold">@AppHelpers.FormatVnd(item.UnitPrice * item.Quantity)</td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>

        <div class="ms-auto" style="max-width:320px">
            <div class="d-flex justify-content-between small mb-1">
                <span>Phí vận chuyển:</span>
                <span>@AppHelpers.FormatVnd(Model.ShippingFee)</span>
            </div>
            @if (Model.DiscountAmount > 0)
            {
                <div class="d-flex justify-content-between small mb-1 text-success">
                    <span>Giảm giá (@Model.CouponCode):</span>
                    <span>-@AppHelpers.FormatVnd(Model.DiscountAmount)</span>
                </div>
            }
            <div class="d-flex justify-content-between border-top pt-2 mt-2">
                <span class="fw-bold">Tổng thanh toán:</span>
                <span class="h5 fw-bold text-danger mb-0">@AppHelpers.FormatVnd(Model.Total)</span>
            </div>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Order\MyOrders.cshtml
```html
@model List<FlowerShop.Models.Order>
@using FlowerShop.Services
@{
    ViewBag.Title = "Lịch sử & Theo dõi đơn hàng - CHAM Flowers";
    string cur = ViewBag.CurrentStatus as string;
    var statuses = new[] { "Chờ xác nhận", "Đang xử lý", "Đang giao", "Hoàn thành", "Đã hủy" };
}

<div class="container py-4">
    <h1 class="h3 fw-bold mb-3"><i class="bi bi-receipt-cutoff text-danger me-2"></i>Đơn Hàng Của Tôi</h1>

    <div class="d-flex flex-wrap gap-2 mb-4">
        <a href="@Url.Action("MyOrders", "Order")" class="btn btn-sm rounded-pill @(string.IsNullOrEmpty(cur) ? "btn-nt-primary" : "btn-light border")">Tất cả</a>
        @foreach (var st in statuses)
        {
            <a href="@Url.Action("MyOrders", "Order", new { status = st })" class="btn btn-sm rounded-pill @(cur == st ? "btn-nt-primary" : "btn-light border")">@st</a>
        }
    </div>

    @if (!Model.Any())
    {
        <div class="bg-white p-5 rounded-4 border text-center">
            <p class="text-muted mb-3">Chưa có đơn hàng nào ở mục này.</p>
            <a href="@Url.Action("Index", "Product")" class="btn btn-nt-primary btn-sm">Đặt hoa ngay</a>
        </div>
    }
    else
    {
        <div class="d-grid gap-3">
            @foreach (var o in Model)
            {
                string badgeClass = "bg-warning-subtle text-warning-emphasis";
                if (o.Status == "Hoàn thành") { badgeClass = "bg-success-subtle text-success"; }
                else if (o.Status == "Đang giao") { badgeClass = "bg-info-subtle text-info"; }
                else if (o.Status == "Đang xử lý") { badgeClass = "bg-primary-subtle text-primary"; }
                else if (o.Status == "Đã hủy") { badgeClass = "bg-secondary-subtle text-secondary"; }

                <div class="bg-white p-4 rounded-4 border shadow-sm">
                    <div class="d-flex flex-wrap justify-content-between align-items-center border-bottom pb-3 mb-3 gap-2">
                        <div>
                            <span class="fw-bold">Đơn hàng #@o.Id</span>
                            <span class="text-muted small ms-2">• Đặt lúc @o.CreatedAt.ToString("HH:mm dd/MM/yyyy")</span>
                        </div>
                        <div class="d-flex align-items-center gap-2">
                            <span class="badge @(o.IsPaid ? "bg-success" : "bg-light text-dark border")">
                                @(o.IsPaid ? string.Format("Đã thanh toán ({0})", o.PaymentMethod) : string.Format("Chưa thanh toán ({0})", o.PaymentMethod))
                            </span>
                            <span class="badge @badgeClass px-3 py-2 rounded-pill">@o.Status</span>
                        </div>
                    </div>

                    <div class="row align-items-center g-3">
                        <div class="col-md-8">
                            @foreach (var item in o.OrderItems)
                            {
                                <div class="d-flex align-items-center gap-3 mb-2">
                                    <img src="@(item.Product != null && !string.IsNullOrEmpty(item.Product.MainImageUrl) ? item.Product.MainImageUrl : "/images/samflora/b10-main.jpg")"
                                         class="rounded-3 border" style="width:52px;height:52px;object-fit:cover" alt="" />
                                    <div class="small">
                                        <div class="fw-bold">@(item.Product != null ? item.Product.Name : "Sản phẩm")</div>
                                        <div class="text-muted">Số lượng: @item.Quantity × @AppHelpers.FormatVnd(item.UnitPrice)</div>
                                    </div>
                                </div>
                            }
                        </div>
                        <div class="col-md-4 text-md-end">
                            <div class="text-muted small">Tổng cộng:</div>
                            <div class="h5 fw-bold text-danger mb-2">@AppHelpers.FormatVnd(o.Total)</div>
                            <div class="d-flex justify-content-md-end gap-2">
                                <a href="@Url.Action("Details", "Order", new { id = o.Id })" class="btn btn-sm btn-nt-outline">Xem chi tiết</a>
                                @if (o.Status == "Chờ xác nhận")
                                {
                                    <form action="@Url.Action("CancelOrder", "Order")" method="post" onsubmit="return confirm('Bạn chắc chắn muốn hủy đơn hàng #@o.Id?');">
                                        @Html.AntiForgeryToken()
                                        <input type="hidden" name="id" value="@o.Id" />
                                        <button type="submit" class="btn btn-sm btn-outline-secondary rounded-pill">Hủy đơn</button>
                                    </form>
                                }
                            </div>
                        </div>
                    </div>
                </div>
            }
        </div>
    }
</div>
```

### File: FlowerShop/Views\Payment\PaymentFailed.cshtml
```html
@model FlowerShop.Models.Order
@using FlowerShop.Services
@{
    ViewBag.Title = "Thanh toán chưa hoàn tất - CHAM Flowers";
}

<div class="container py-5" style="max-width:620px">
    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm text-center">
        <i class="bi bi-exclamation-circle-fill text-danger display-3"></i>
        <h3 class="fw-bold mt-3">Thanh Toán Chưa Thành Công</h3>
        <p class="text-muted small">Giao dịch cho đơn hàng <strong>#@Model.Id</strong> (@AppHelpers.FormatVnd(Model.Total)) chưa hoàn tất. Bạn có thể thử thanh toán lại qua VNPay hoặc đổi sang nhận hàng trả tiền mặt (COD).</p>

        <form action="@Url.Action("RetryPayment", "Payment")" method="post" class="d-flex flex-wrap justify-content-center gap-2 mt-4">
            @Html.AntiForgeryToken()
            <input type="hidden" name="orderId" value="@Model.Id" />
            <button type="submit" name="paymentMethod" value="VNPay" class="btn btn-primary rounded-pill px-4">
                <i class="bi bi-arrow-repeat me-1"></i> Thanh toán lại qua VNPay
            </button>
            <button type="submit" name="paymentMethod" value="COD" class="btn btn-nt-outline px-4">
                Đổi sang thanh toán COD
            </button>
        </form>
    </div>
</div>
```

### File: FlowerShop/Views\Payment\PaymentSuccess.cshtml
```html
@model FlowerShop.Models.Order
@using FlowerShop.Services
@{
    ViewBag.Title = "Đặt hoa thành công - CHAM Flowers";
}

<div class="container py-5" style="max-width:680px">
    <div class="bg-white p-4 p-md-5 rounded-4 border shadow-sm text-center">
        <div class="rounded-circle bg-success-subtle text-success d-inline-flex align-items-center justify-content-center mb-3" style="width:80px;height:80px">
            <i class="bi bi-check-lg display-4"></i>
        </div>
        <h2 class="fw-bold mb-2">Cảm Ơn Bạn Đã Đặt Hoa Tại CHAM Flowers!</h2>
        <p class="text-muted">Đơn hàng <strong>#@Model.Id</strong> đã được gửi tới hệ thống quản trị để chuẩn bị hoa tươi.</p>

        <div class="bg-light rounded-4 p-3 text-start small my-4 border">
            <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">Người nhận:</span>
                <strong>@Model.RecipientName (@Model.RecipientPhone)</strong>
            </div>
            <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">Địa chỉ giao:</span>
                <span>@Model.DeliveryAddress</span>
            </div>
            <div class="d-flex justify-content-between mb-1">
                <span class="text-muted">Phương thức thanh toán:</span>
                <strong>@Model.PaymentMethod (@(Model.IsPaid ? "Đã thanh toán" : "Thanh toán khi nhận hàng"))</strong>
            </div>
            @if (!string.IsNullOrEmpty(Model.VnPayTransactionNo))
            {
                <div class="d-flex justify-content-between mb-1 text-success">
                    <span>Mã giao dịch VNPay:</span>
                    <strong>@Model.VnPayTransactionNo</strong>
                </div>
            }
            <div class="d-flex justify-content-between border-top pt-2 mt-2">
                <span class="fw-bold">Tổng tiền:</span>
                <strong class="text-danger fs-6">@AppHelpers.FormatVnd(Model.Total)</strong>
            </div>
        </div>

        <div class="d-flex flex-wrap justify-content-center gap-3">
            <a href="@Url.Action("Details", "Order", new { id = Model.Id })" class="btn btn-nt-primary">Theo dõi tiến độ đơn hàng</a>
            <a href="@Url.Action("Index", "Product")" class="btn btn-nt-outline">Tiếp tục mua sắm</a>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Payment\VnPayGateway.cshtml
```html
@using FlowerShop.Services
@{
    ViewBag.Title = "Cổng thanh toán trực tuyến VNPay Sandbox - CHAM Flowers";
    var order = ViewBag.Order as FlowerShop.Models.Order;
    string rawQuery = ViewBag.QueryString as string ?? "";
}

<div class="container py-5" style="max-width:760px">
    <div class="bg-white rounded-4 border shadow overflow-hidden">
        <div class="p-4 text-white d-flex justify-content-between align-items-center" style="background: linear-gradient(90deg, #005baa 0%, #0072bc 100%);">
            <div>
                <h4 class="fw-bold mb-0"><i class="bi bi-shield-lock-fill me-2"></i>VNPAY PAYMENT GATEWAY</h4>
                <div class="small opacity-75">Cổng thanh toán điện tử bảo mật chuẩn HMAC-SHA512 (Sandbox Test Mode)</div>
            </div>
            <span class="badge bg-warning text-dark px-3 py-2">NCB SANDBOX</span>
        </div>

        <div class="p-4 p-md-5">
            <div class="row g-4">
                <div class="col-md-5 border-end">
                    <div class="text-muted small text-uppercase fw-bold mb-2">Thông tin đơn hàng</div>
                    <div class="mb-2 small">Đơn vị thụ hưởng: <strong>CHAM FLOWERS</strong></div>
                    <div class="mb-2 small">Mã tham chiếu: <strong>#@(order != null ? order.Id : 0)</strong></div>
                    <div class="mb-3 small">Nội dung: <em>Thanh toán đơn hoa tươi #@(order != null ? order.Id : 0)</em></div>
                    <div class="p-3 rounded-3 bg-primary-subtle text-primary">
                        <div class="small">Số tiền thanh toán:</div>
                        <div class="h4 fw-bold mb-0">@AppHelpers.FormatVnd(order != null ? order.Total : 0)</div>
                    </div>
                </div>

                <div class="col-md-7">
                    <h6 class="fw-bold mb-3">Thẻ ATM Nội Địa Test (Ngân hàng NCB)</h6>
                    <div class="mb-2">
                        <label class="form-label small text-muted mb-1">Số thẻ Test Sandbox</label>
                        <input type="text" class="form-control form-control-sm bg-light fw-semibold" value="9704198526191432198" readonly />
                    </div>
                    <div class="row g-2 mb-3">
                        <div class="col-7">
                            <label class="form-label small text-muted mb-1">Tên chủ thẻ</label>
                            <input type="text" class="form-control form-control-sm bg-light" value="NGUYEN VAN A" readonly />
                        </div>
                        <div class="col-5">
                            <label class="form-label small text-muted mb-1">Mã OTP Test</label>
                            <input type="text" class="form-control form-control-sm bg-light" value="123456" readonly />
                        </div>
                    </div>

                    <div class="d-grid gap-2 mt-4">
                        <form action="@Url.Action("VnPaySimulateCallback", "Payment")" method="post">
                            <input type="hidden" name="rawQuery" value="@rawQuery" />
                            <input type="hidden" name="isSuccess" value="true" />
                            <button type="submit" class="btn btn-primary w-100 py-2 fw-bold rounded-pill">
                                <i class="bi bi-check2-circle me-1"></i> Xác nhận Thanh toán Thành công (Mã 00)
                            </button>
                        </form>

                        <form action="@Url.Action("VnPaySimulateCallback", "Payment")" method="post">
                            <input type="hidden" name="rawQuery" value="@rawQuery" />
                            <input type="hidden" name="isSuccess" value="false" />
                            <button type="submit" class="btn btn-outline-danger w-100 btn-sm rounded-pill">
                                Hủy giao dịch / Giả lập lỗi thanh toán (Mã 24)
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Product\AdminIndex.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.Product>
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = "Quản lý Sản phẩm & Tồn kho";
    var categories = ViewBag.Categories as List<FlowerShop.Models.Category> ?? new List<FlowerShop.Models.Category>();
    int? selCat = ViewBag.CategoryId;
}

<div class="admin-card p-3 px-4 mb-4 d-flex flex-wrap justify-content-between align-items-center gap-3">
    <form action="@Url.Action("AdminIndex", "Product")" method="get" class="d-flex flex-wrap align-items-center gap-2">
        <div class="admin-quick-search" style="width:255px">
            <i class="bi bi-search text-muted me-2" style="font-size:0.82rem"></i>
            <input type="text" name="search" value="@ViewBag.Search" placeholder="Tìm tên mẫu hoa, slug..." />
        </div>
        <select name="categoryId" class="form-select form-select-sm rounded-pill px-3" style="width:200px;height:38px">
            <option value="">-- Tất cả danh mục --</option>
            @foreach (var c in categories)
            {
                <option value="@c.Id" @Html.Raw(selCat == c.Id ? "selected=\"selected\"" : "")>@c.Name</option>
            }
        </select>
        <button type="submit" class="btn btn-sm btn-dark rounded-pill px-3" style="height:38px">
            <i class="bi bi-sliders me-1"></i>Lọc kho
        </button>
    </form>

    <a href="@Url.Action("Create", "Product")" class="btn btn-sm btn-admin-primary d-inline-flex align-items-center gap-1">
        <i class="bi bi-plus-lg"></i> Thêm sản phẩm mới
    </a>
</div>

<div class="admin-card overflow-hidden">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead>
                <tr>
                    <th class="ps-4">Mã</th>
                    <th>Mẫu hoa thiết kế</th>
                    <th>Danh mục</th>
                    <th>Giá bán niêm yết</th>
                    <th>Tồn kho</th>
                    <th>Nhãn tiếp thị</th>
                    <th>Trạng thái</th>
                    <th class="text-end pe-4">Thao tác</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var p in Model)
                {
                    <tr>
                        <td class="ps-4 fw-bold text-muted small">#@p.Id</td>
                        <td>
                            <div class="d-flex align-items-center gap-3">
                                <img src="@p.MainImageUrl" class="rounded-3 border shadow-sm" style="width:52px;height:52px;object-fit:cover" alt="" />
                                <div>
                                    <div class="fw-bold small text-dark">@p.Name</div>
                                    <div class="text-muted" style="font-size:0.73rem">/@p.Slug</div>
                                </div>
                            </div>
                        </td>
                        <td>
                            <span class="badge bg-light text-secondary border">@(p.Category != null ? p.Category.Name : "")</span>
                        </td>
                        <td>
                            <div class="fw-bold text-danger small">@AppHelpers.FormatVnd(p.Price)</div>
                            @if (p.OriginalPrice.HasValue)
                            {
                                <div class="text-muted text-decoration-line-through" style="font-size:0.74rem">@AppHelpers.FormatVnd(p.OriginalPrice.Value)</div>
                            }
                        </td>
                        <td>
                            <span class="badge @(p.StockQuantity <= 5 ? "bg-danger-subtle text-danger" : "bg-success-subtle text-success")">
                                @p.StockQuantity bó
                            </span>
                        </td>
                        <td>
                            @if (p.IsFeatured)
                            {
                                <span class="badge bg-primary-subtle text-primary me-1">Nổi bật</span>
                            }
                            @if (p.IsHotDeal)
                            {
                                <span class="badge bg-warning-subtle text-warning-emphasis"><i class="bi bi-fire me-1"></i>Hot Deal</span>
                            }
                        </td>
                        <td>
                            <span class="badge @(p.IsActive ? "bg-success-subtle text-success" : "bg-secondary-subtle text-secondary")">
                                @(p.IsActive ? "● Đang bán" : "○ Đang ẩn")
                            </span>
                        </td>
                        <td class="text-end pe-4">
                            <div class="d-inline-flex gap-1">
                                <a href="@Url.Action("Details", "Product", new { id = p.Id })" target="_blank" class="btn btn-sm btn-light border rounded-pill px-2" title="Xem trên cửa hàng"><i class="bi bi-box-arrow-up-right"></i></a>
                                <a href="@Url.Action("Edit", "Product", new { id = p.Id })" class="btn btn-sm btn-outline-primary rounded-pill px-3"><i class="bi bi-pencil-square me-1"></i>Sửa</a>
                                <form action="@Url.Action("Delete", "Product", new { id = p.Id })" method="post" onsubmit="return confirm('Xác nhận xóa sản phẩm @p.Name?');">
                                    @Html.AntiForgeryToken()
                                    <button type="submit" class="btn btn-sm btn-outline-danger rounded-pill px-2" title="Xóa"><i class="bi bi-trash"></i></button>
                                </form>
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

@if (Model.PageCount > 1)
{
    <nav class="mt-4 d-flex justify-content-center">
        <ul class="pagination pagination-sm">
            @for (int i = 1; i <= Model.PageCount; i++)
            {
                <li class="page-item @(i == Model.PageNumber ? "active" : "")">
                    <a class="page-link" href="@Url.Action("AdminIndex", "Product", new { page = i, search = ViewBag.Search, categoryId = ViewBag.CategoryId })">@i</a>
                </li>
            }
        </ul>
    </nav>
}
```

### File: FlowerShop/Views\Product\Categories.cshtml
```html
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = "Quản lý Danh mục • Chủng loại hoa • Nhà cung cấp";
    var categories = ViewBag.Categories as List<FlowerShop.Models.Category> ?? new List<FlowerShop.Models.Category>();
    var flowerCategories = ViewBag.FlowerCategories as List<FlowerShop.Models.FlowerCategory> ?? new List<FlowerShop.Models.FlowerCategory>();
    var suppliers = ViewBag.Suppliers as List<FlowerShop.Models.Supplier> ?? new List<FlowerShop.Models.Supplier>();
}

<div class="row g-4">
    <!-- 1. Danh mục sản phẩm (Categories) -->
    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <h6 class="fw-bold mb-3"><i class="bi bi-tags-fill text-danger me-2"></i>Danh Mục Chủ Đề (Categories)</h6>
            <form action="@Url.Action("SaveCategory", "Product")" method="post" class="d-flex gap-2 mb-3">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="0" />
                <input type="text" name="name" class="form-control form-control-sm" placeholder="Tên danh mục mới (VD: Hoa Chia Buồn)..." required />
                <input type="hidden" name="isActive" value="true" />
                <button type="submit" class="btn btn-sm btn-danger text-nowrap">+ Thêm</button>
            </form>

            <table class="table table-sm align-middle mb-0">
                <thead class="small text-muted">
                    <tr><th>Tên danh mục</th><th>Số SP</th><th>Hiển thị</th><th></th></tr>
                </thead>
                <tbody>
                    @foreach (var c in categories)
                    {
                        <tr>
                            <td class="fw-semibold small">@c.Name</td>
                            <td><span class="badge bg-light text-dark border">@c.Products.Count</span></td>
                            <td><span class="badge @(c.IsActive ? "bg-success" : "bg-secondary")">@(c.IsActive ? "Hiện" : "Ẩn")</span></td>
                            <td class="text-end">
                                <form action="@Url.Action("DeleteCategory", "Product")" method="post" class="d-inline" onsubmit="return confirm('Xóa danh mục @c.Name?');">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="id" value="@c.Id" />
                                    <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                                </form>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>

    <!-- 2. Chủng loại hoa (FlowerCategories) -->
    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <h6 class="fw-bold mb-3"><i class="bi bi-flower3 text-danger me-2"></i>Chủng Loại Hoa (FlowerCategories)</h6>
            <form action="@Url.Action("SaveFlowerCategory", "Product")" method="post" class="row g-2 mb-3">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="0" />
                <div class="col-sm-5">
                    <input type="text" name="name" class="form-control form-control-sm" placeholder="Tên loài hoa..." required />
                </div>
                <div class="col-sm-5">
                    <input type="text" name="description" class="form-control form-control-sm" placeholder="Ý nghĩa ngắn..." />
                </div>
                <div class="col-sm-2 d-grid">
                    <button type="submit" class="btn btn-sm btn-danger">+ Thêm</button>
                </div>
            </form>

            <table class="table table-sm align-middle mb-0">
                <thead class="small text-muted">
                    <tr><th>Loài hoa</th><th>Mô tả</th><th></th></tr>
                </thead>
                <tbody>
                    @foreach (var fc in flowerCategories)
                    {
                        <tr>
                            <td class="fw-semibold small">@fc.Name</td>
                            <td class="small text-muted">@fc.Description</td>
                            <td class="text-end">
                                <form action="@Url.Action("DeleteFlowerCategory", "Product")" method="post" class="d-inline">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="id" value="@fc.Id" />
                                    <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                                </form>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>

    <!-- 3. Nhà cung cấp (Suppliers) -->
    <div class="col-12">
        <div class="admin-card p-4">
            <h6 class="fw-bold mb-3"><i class="bi bi-truck-flatbed text-danger me-2"></i>Nhà Cung Cấp Nguồn Hoa (Suppliers)</h6>
            <form action="@Url.Action("SaveSupplier", "Product")" method="post" class="row g-2 mb-3">
                @Html.AntiForgeryToken()
                <input type="hidden" name="id" value="0" />
                <div class="col-md-3"><input type="text" name="name" class="form-control form-control-sm" placeholder="Tên nhà vườn / đối tác *" required /></div>
                <div class="col-md-2"><input type="text" name="phone" class="form-control form-control-sm" placeholder="Số điện thoại" /></div>
                <div class="col-md-3"><input type="email" name="email" class="form-control form-control-sm" placeholder="Email liên hệ" /></div>
                <div class="col-md-3"><input type="text" name="address" class="form-control form-control-sm" placeholder="Địa chỉ vườn hoa" /></div>
                <div class="col-md-1 d-grid"><button type="submit" class="btn btn-sm btn-danger">Lưu</button></div>
            </form>

            <div class="table-responsive">
                <table class="table table-sm align-middle mb-0">
                    <thead class="small text-muted">
                        <tr><th>Tên nhà cung cấp</th><th>Điện thoại</th><th>Email</th><th>Địa chỉ</th><th>Số SP</th><th></th></tr>
                    </thead>
                    <tbody>
                        @foreach (var s in suppliers)
                        {
                            <tr>
                                <td class="fw-semibold small">@s.Name</td>
                                <td class="small">@s.Phone</td>
                                <td class="small">@s.Email</td>
                                <td class="small text-muted">@s.Address</td>
                                <td><span class="badge bg-light text-dark border">@s.Products.Count</span></td>
                                <td class="text-end">
                                    <form action="@Url.Action("DeleteSupplier", "Product")" method="post" class="d-inline">
                                        @Html.AntiForgeryToken()
                                        <input type="hidden" name="id" value="@s.Id" />
                                        <button type="submit" class="btn btn-sm btn-outline-danger py-0"><i class="bi bi-trash"></i></button>
                                    </form>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Product\Create.cshtml
```html
@model FlowerShop.Models.Product
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = "Thêm Sản phẩm Hoa mới";
}

<div class="admin-card p-4" style="max-width:880px">
    <form action="@Url.Action("Create", "Product")" method="post" enctype="multipart/form-data">
        @Html.AntiForgeryToken()
        @if (!ViewData.ModelState.IsValid)
        {
            @Html.ValidationSummary(false, "", new { @class = "alert alert-danger small rounded-3" })
        }

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label small fw-bold">Tên sản phẩm hoa *</label>
                @Html.TextBoxFor(m => m.Name, new { @class = "form-control", placeholder = "VD: Bó Hoa Hồng Trà Cụm Nơ Lụa" })
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Danh mục chủ đề *</label>
                @Html.DropDownListFor(m => m.CategoryId, ViewBag.CategoryList as SelectList, new { @class = "form-select" })
            </div>

            <div class="col-md-6">
                <label class="form-label small fw-bold">Chủng loại hoa</label>
                @Html.DropDownListFor(m => m.FlowerCategoryId, ViewBag.FlowerCategoryList as SelectList, "-- Chọn loại hoa --", new { @class = "form-select" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Nhà cung cấp</label>
                @Html.DropDownListFor(m => m.SupplierId, ViewBag.SupplierList as SelectList, "-- Chọn nhà cung cấp --", new { @class = "form-select" })
            </div>

            <div class="col-md-4">
                <label class="form-label small fw-bold">Giá bán hiện tại (VNĐ) *</label>
                <input type="number" name="Price" value="@((long)Model.Price)" step="1000" class="form-control" required />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Giá gốc chưa giảm (VNĐ)</label>
                <input type="number" name="OriginalPrice" value="@(Model.OriginalPrice.HasValue ? ((long)Model.OriginalPrice.Value).ToString() : "")" step="1000" class="form-control" />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Số lượng tồn kho *</label>
                <input type="number" name="StockQuantity" value="@Model.StockQuantity" class="form-control" required />
            </div>

            <div class="col-md-6">
                <label class="form-label small fw-bold">Tải lên ảnh đại diện chính (.jpg, .png, .gif, .svg)</label>
                <input type="file" name="mainImageFile" class="form-control" accept=".jpg,.jpeg,.png,.gif,.webp,.svg" />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Hoặc chọn mẫu ảnh có sẵn / URL</label>
                <input type="text" name="customImageUrl" class="form-control" placeholder="/images/samflora/b10-main.jpg" />
            </div>

            <div class="col-md-6">
                <label class="form-label small fw-bold">Thư viện ảnh phụ (Gallery)</label>
                <input type="file" name="galleryFiles" multiple class="form-control" accept=".jpg,.jpeg,.png,.gif,.webp,.svg" />
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Hạn bảo quản lô hoa</label>
                <input type="date" name="ExpiryDate" value="@(Model.ExpiryDate.HasValue ? Model.ExpiryDate.Value.ToString("yyyy-MM-dd") : "")" class="form-control" />
            </div>

            <div class="col-12">
                <label class="form-label small fw-bold">Mô tả chi tiết bó hoa</label>
                @Html.TextAreaFor(m => m.Description, new { rows = 3, @class = "form-control", placeholder = "Thành phần các loài hoa, giấy gói, kích thước và ý nghĩa..." })
            </div>

            <div class="col-12 d-flex flex-wrap gap-4 pt-2">
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsFeatured, new { @class = "form-check-input" })
                    <label for="IsFeatured" class="form-check-label small fw-semibold">Sản phẩm nổi bật (Trang chủ)</label>
                </div>
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsHotDeal, new { @class = "form-check-input" })
                    <label for="IsHotDeal" class="form-check-label small fw-semibold">Đưa vào chương trình Hot Deal</label>
                </div>
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input" })
                    <label for="IsActive" class="form-check-label small fw-semibold">Đang kinh doanh hiển thị</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4"><i class="bi bi-save me-1"></i> Lưu sản phẩm</button>
            <a href="@Url.Action("AdminIndex", "Product")" class="btn btn-light border rounded-pill px-4">Hủy bỏ</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\Product\Edit.cshtml
```html
@model FlowerShop.Models.Product
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewData["Title"] = string.Format("Chỉnh sửa Sản phẩm #{0}", Model.Id);
}

<div class="admin-card p-4" style="max-width:900px">
    <form action="@Url.Action("Edit", "Product", new { id = Model.Id })" method="post" enctype="multipart/form-data">
        @Html.AntiForgeryToken()
        @Html.HiddenFor(m => m.Id)
        @if (!ViewData.ModelState.IsValid)
        {
            @Html.ValidationSummary(false, "", new { @class = "alert alert-danger small rounded-3" })
        }

        <div class="row g-3">
            <div class="col-md-8">
                <label class="form-label small fw-bold">Tên sản phẩm hoa *</label>
                @Html.TextBoxFor(m => m.Name, new { @class = "form-control" })
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Danh mục *</label>
                @Html.DropDownListFor(m => m.CategoryId, ViewBag.CategoryList as SelectList, new { @class = "form-select" })
            </div>

            <div class="col-md-6">
                <label class="form-label small fw-bold">Chủng loại hoa</label>
                @Html.DropDownListFor(m => m.FlowerCategoryId, ViewBag.FlowerCategoryList as SelectList, "-- Chọn loại hoa --", new { @class = "form-select" })
            </div>
            <div class="col-md-6">
                <label class="form-label small fw-bold">Nhà cung cấp</label>
                @Html.DropDownListFor(m => m.SupplierId, ViewBag.SupplierList as SelectList, "-- Chọn nhà cung cấp --", new { @class = "form-select" })
            </div>

            <div class="col-md-4">
                <label class="form-label small fw-bold">Giá bán (VNĐ) *</label>
                <input type="number" name="Price" value="@((long)Model.Price)" step="1000" class="form-control" required />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Giá gốc (VNĐ)</label>
                <input type="number" name="OriginalPrice" value="@(Model.OriginalPrice.HasValue ? ((long)Model.OriginalPrice.Value).ToString() : "")" step="1000" class="form-control" />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Tồn kho *</label>
                <input type="number" name="StockQuantity" value="@Model.StockQuantity" class="form-control" required />
            </div>

            @if (Model.Images != null && Model.Images.Any())
            {
                <div class="col-12">
                    <label class="form-label small fw-bold">Ảnh hiện tại (Tích chọn để xóa ảnh cũ)</label>
                    <div class="d-flex flex-wrap gap-3">
                        @foreach (var img in Model.Images)
                        {
                            <label class="border rounded-3 p-2 text-center small">
                                <img src="@img.ImageUrl" class="d-block rounded-2 mb-1" style="width:76px;height:76px;object-fit:cover" alt="" />
                                <input type="checkbox" name="deleteImageIds" value="@img.Id" class="form-check-input me-1" /> Xóa
                            </label>
                        }
                    </div>
                </div>
            }

            <div class="col-md-4">
                <label class="form-label small fw-bold">Thay ảnh chính mới</label>
                <input type="file" name="mainImageFile" class="form-control form-control-sm" />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Thêm ảnh Gallery</label>
                <input type="file" name="galleryFiles" multiple class="form-control form-control-sm" />
            </div>
            <div class="col-md-4">
                <label class="form-label small fw-bold">Hạn bảo quản</label>
                <input type="date" name="ExpiryDate" value="@(Model.ExpiryDate.HasValue ? Model.ExpiryDate.Value.ToString("yyyy-MM-dd") : "")" class="form-control form-control-sm" />
            </div>

            <div class="col-12">
                <label class="form-label small fw-bold">Mô tả chi tiết</label>
                @Html.TextAreaFor(m => m.Description, new { rows = 3, @class = "form-control" })
            </div>

            <div class="col-12 d-flex flex-wrap gap-4">
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsFeatured, new { @class = "form-check-input" })
                    <label for="IsFeatured" class="form-check-label small fw-semibold">Sản phẩm nổi bật</label>
                </div>
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsHotDeal, new { @class = "form-check-input" })
                    <label for="IsHotDeal" class="form-check-label small fw-semibold">Hot Deal khuyến mãi</label>
                </div>
                <div class="form-check">
                    @Html.CheckBoxFor(m => m.IsActive, new { @class = "form-check-input" })
                    <label for="IsActive" class="form-check-label small fw-semibold">Đang kinh doanh</label>
                </div>
            </div>
        </div>

        <div class="mt-4 d-flex gap-2">
            <button type="submit" class="btn btn-danger rounded-pill px-4">Lưu cập nhật</button>
            <a href="@Url.Action("AdminIndex", "Product")" class="btn btn-light border rounded-pill px-4">Quay lại</a>
        </div>
    </form>
</div>
```

### File: FlowerShop/Views\Report\Index.cshtml
```html
@model FlowerShop.Models.ReportDashboardViewModel
@using FlowerShop.Services
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Dashboard & Báo cáo Thống kê Kinh doanh - CHAM Flowers Executive CMS";
    var curPreset = Request.QueryString["preset"] ?? "";
    var adminName = (User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name)) ? User.Identity.Name : "Quản trị viên";
}

<!-- Executive Welcome Hero & Bộ lọc thời gian -->
<div class="admin-hero-banner mb-4">
    <div class="row align-items-center g-3 position-relative" style="z-index:1">
        <div class="col-xl-5">
            <div class="d-inline-flex align-items-center gap-2 px-3 py-1 rounded-pill mb-2" style="background:rgba(255,255,255,0.14);font-size:0.76rem;font-weight:600">
                <i class="bi bi-stars text-warning"></i> Báo cáo kỳ: @Model.PeriodLabel
            </div>
            <h3 class="serif-heading fw-bold mb-1">Xin chào, @adminName!</h3>
            <p class="mb-0 text-white-50 small">Theo dõi doanh thu, đơn đặt hoa, khách hàng VIP và hạn bảo quản kho hoa tươi theo thời gian thực.</p>
        </div>

        <div class="col-xl-7">
            <div class="p-3 rounded-4 d-flex flex-wrap justify-content-xl-end align-items-center gap-2" style="background:rgba(255,255,255,0.12);backdrop-filter:blur(10px);border:1px solid rgba(255,255,255,0.16)">
                <form action="@Url.Action("Index", "Report")" method="get" class="d-flex flex-wrap align-items-center gap-2">
                    <div class="btn-group btn-group-sm" role="group">
                        <a href="@Url.Action("Index", "Report", new { preset = "7d" })" class="btn @(curPreset == "7d" ? "btn-warning fw-bold text-dark" : "btn-outline-light")">7 ngày</a>
                        <a href="@Url.Action("Index", "Report", new { preset = "30d" })" class="btn @(string.IsNullOrEmpty(curPreset) || curPreset == "30d" ? "btn-warning fw-bold text-dark" : "btn-outline-light")">30 ngày</a>
                        <a href="@Url.Action("Index", "Report", new { preset = "90d" })" class="btn @(curPreset == "90d" ? "btn-warning fw-bold text-dark" : "btn-outline-light")">90 ngày</a>
                    </div>
                    <input type="date" name="fromDate" value="@Model.FromDate.ToString("yyyy-MM-dd")" class="form-control form-control-sm border-0" style="width:136px" title="Từ ngày" />
                    <span class="text-white-50 small">→</span>
                    <input type="date" name="toDate" value="@Model.ToDate.ToString("yyyy-MM-dd")" class="form-control form-control-sm border-0" style="width:136px" title="Đến ngày" />
                    <button type="submit" class="btn btn-sm btn-light fw-bold text-danger rounded-pill px-3">
                        <i class="bi bi-funnel-fill me-1"></i>Lọc
                    </button>
                </form>

                <a href="@Url.Action("ExportCsv", "Report", new { fromDate = Model.FromDate.ToString("yyyy-MM-dd"), toDate = Model.ToDate.ToString("yyyy-MM-dd") })"
                   class="btn btn-sm btn-success rounded-pill px-3 fw-semibold shadow-sm">
                    <i class="bi bi-file-earmark-excel-fill me-1"></i> Xuất Excel (.CSV)
                </a>
            </div>
        </div>
    </div>
</div>

@if (ViewBag.NoDataMessage != null)
{
    <div class="alert alert-warning rounded-4 border-0 shadow-sm mb-4">
        <i class="bi bi-info-circle-fill me-2"></i>@ViewBag.NoDataMessage
    </div>
}

<!-- 4 Signature Executive KPI Cards -->
<div class="row g-3 mb-4">
    <div class="col-sm-6 col-xl-3">
        <div class="stat-kpi">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <div class="text-muted small fw-semibold mb-1">Doanh thu thực nhận</div>
                    <div class="kpi-value text-danger">@AppHelpers.FormatVnd(Model.TotalRevenue)</div>
                </div>
                <div class="kpi-icon-orb kpi-orb-rose">
                    <i class="bi bi-cash-coin"></i>
                </div>
            </div>
            <div class="d-flex align-items-center justify-content-between pt-2 border-top">
                <span class="text-muted" style="font-size:0.78rem">Lợi nhuận gộp ước tính</span>
                <span class="badge bg-success-subtle text-success">+@AppHelpers.FormatVnd(Model.GrossProfit)</span>
            </div>
        </div>
    </div>

    <div class="col-sm-6 col-xl-3">
        <div class="stat-kpi kpi-orders">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <div class="text-muted small fw-semibold mb-1">Tổng đơn hàng kỳ này</div>
                    <div class="kpi-value">@Model.TotalOrders <span class="fs-6 fw-semibold text-muted">đơn</span></div>
                </div>
                <div class="kpi-icon-orb kpi-orb-indigo">
                    <i class="bi bi-bag-heart-fill"></i>
                </div>
            </div>
            <div class="d-flex align-items-center justify-content-between pt-2 border-top small">
                <span>Hoàn thành: <strong class="text-success">@Model.CompletedOrders</strong></span>
                <a href="@Url.Action("AdminIndex", "Order", new { status = "Chờ xác nhận" })" class="badge bg-warning-subtle text-warning-emphasis text-decoration-none">
                    Chờ duyệt: @Model.PendingOrders
                </a>
            </div>
        </div>
    </div>

    <div class="col-sm-6 col-xl-3">
        <div class="stat-kpi kpi-vip">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <div class="text-muted small fw-semibold mb-1">Hội viên & Khách VIP</div>
                    <div class="kpi-value">@Model.TotalCustomers <span class="fs-6 fw-semibold text-muted">khách</span></div>
                </div>
                <div class="kpi-icon-orb kpi-orb-amber">
                    <i class="bi bi-gem"></i>
                </div>
            </div>
            <div class="d-flex align-items-center justify-content-between pt-2 border-top small">
                <span>Hạng VIP: <strong class="text-danger">@Model.VipCustomers</strong></span>
                <span class="badge bg-primary-subtle text-primary">+@Model.NewCustomersInPeriod mới</span>
            </div>
        </div>
    </div>

    <div class="col-sm-6 col-xl-3">
        <div class="stat-kpi kpi-stock">
            <div class="d-flex justify-content-between align-items-start mb-3">
                <div>
                    <div class="text-muted small fw-semibold mb-1">Danh mục & Kho hoa tươi</div>
                    <div class="kpi-value">@Model.TotalProducts <span class="fs-6 fw-semibold text-muted">mẫu hoa</span></div>
                </div>
                <div class="kpi-icon-orb kpi-orb-emerald">
                    <i class="bi bi-flower1"></i>
                </div>
            </div>
            <div class="d-flex align-items-center justify-content-between pt-2 border-top small">
                <span class="badge bg-danger-subtle text-danger">Sắp hết: @Model.LowStockCount</span>
                <span class="badge bg-warning-subtle text-warning-emphasis">Sắp héo/hết hạn: @Model.ExpiringSoonCount</span>
            </div>
        </div>
    </div>
</div>

<!-- Biểu đồ Doanh thu & Biểu đồ Trạng thái đơn hàng -->
<div class="row g-4 mb-4">
    <div class="col-lg-8">
        <div class="admin-card p-4 h-100">
            <div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
                <div>
                    <h6 class="fw-bold mb-0">Biểu Đồ Doanh Thu & Lợi Nhuận Gộp</h6>
                    <div class="text-muted small">Phân tích biến động doanh thu thực tế theo mốc thời gian (@Model.PeriodLabel)</div>
                </div>
                <span class="badge bg-danger-subtle text-danger"><i class="bi bi-graph-up-arrow me-1"></i>Tỷ suất gộp ~42%</span>
            </div>
            <canvas id="revenueChart" height="118"></canvas>
        </div>
    </div>

    <div class="col-lg-4">
        <div class="admin-card p-4 h-100 d-flex flex-column">
            <div class="mb-3">
                <h6 class="fw-bold mb-0">Tỷ Lệ Trạng Thái Đơn Hàng</h6>
                <div class="text-muted small">Phân bổ @Model.TotalOrders đơn hàng trong kỳ</div>
            </div>
            <div class="my-auto position-relative" style="max-height:220px">
                <canvas id="orderStatusChart"></canvas>
            </div>
            <div class="row g-2 mt-3 pt-3 border-top small">
                <div class="col-6 d-flex justify-content-between"><span><i class="bi bi-circle-fill text-success me-1" style="font-size:0.55rem"></i>Hoàn thành</span><strong>@Model.CompletedOrders</strong></div>
                <div class="col-6 d-flex justify-content-between"><span><i class="bi bi-circle-fill text-warning me-1" style="font-size:0.55rem"></i>Chờ duyệt</span><strong>@Model.PendingOrders</strong></div>
                <div class="col-6 d-flex justify-content-between"><span><i class="bi bi-circle-fill text-primary me-1" style="font-size:0.55rem"></i>Đang xử lý</span><strong>@Model.ProcessingOrders</strong></div>
                <div class="col-6 d-flex justify-content-between"><span><i class="bi bi-circle-fill text-info me-1" style="font-size:0.55rem"></i>Đang giao</span><strong>@Model.ShippingOrders</strong></div>
            </div>
        </div>
    </div>
</div>

<!-- Top Hoa Bán Chạy & Top Khách Hàng VIP -->
<div class="row g-4 mb-4">
    <div class="col-lg-7">
        <div class="admin-card overflow-hidden h-100">
            <div class="p-4 pb-3 d-flex justify-content-between align-items-center">
                <div>
                    <h6 class="fw-bold mb-0"><i class="bi bi-trophy-fill text-warning me-2"></i>Top Mẫu Hoa Bán Chạy Nhất</h6>
                    <div class="text-muted small">Xếp hạng theo số lượng bó/hộp hoa tiêu thụ</div>
                </div>
                <a href="@Url.Action("AdminIndex", "Product")" class="btn btn-sm btn-light border rounded-pill px-3">Xem kho hoa</a>
            </div>
            <div class="table-responsive">
                <table class="table align-middle">
                    <thead>
                        <tr>
                            <th class="ps-4">Hạng</th>
                            <th>Mẫu hoa thiết kế</th>
                            <th>Danh mục</th>
                            <th>Đã bán</th>
                            <th>Doanh thu</th>
                            <th class="pe-4 text-end">Tồn kho</th>
                        </tr>
                    </thead>
                    <tbody>
                        @{ int rank = 1; }
                        @foreach (var p in Model.BestSellingProducts)
                        {
                            string rCls = rank == 1 ? "rank-1" : (rank == 2 ? "rank-2" : (rank == 3 ? "rank-3" : "rank-n"));
                            <tr>
                                <td class="ps-4"><span class="rank-medal @rCls">@rank</span></td>
                                <td>
                                    <div class="d-flex align-items-center gap-3">
                                        <img src="@p.ImageUrl" class="rounded-3 border shadow-sm" style="width:44px;height:44px;object-fit:cover" alt="" />
                                        <span class="fw-bold small text-dark">@p.ProductName</span>
                                    </div>
                                </td>
                                <td class="small text-muted">@p.CategoryName</td>
                                <td><span class="badge bg-success-subtle text-success">@p.SoldQuantity bó</span></td>
                                <td class="fw-bold text-danger small">@AppHelpers.FormatVnd(p.TotalRevenue)</td>
                                <td class="pe-4 text-end">
                                    <span class="badge @(p.StockQuantity <= 5 ? "bg-danger-subtle text-danger" : "bg-light text-secondary border")">@p.StockQuantity</span>
                                </td>
                            </tr>
                            rank++;
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <div class="col-lg-5">
        <div class="admin-card overflow-hidden h-100">
            <div class="p-4 pb-3 d-flex justify-content-between align-items-center">
                <div>
                    <h6 class="fw-bold mb-0"><i class="bi bi-gem text-danger me-2"></i>Top Khách Hàng VIP Thân Thiết</h6>
                    <div class="text-muted small">Tự động nâng hạng VIP khi tích lũy từ 5.000.000 ₫</div>
                </div>
                <a href="@Url.Action("Index", "Customer")" class="btn btn-sm btn-light border rounded-pill px-3">Tất cả</a>
            </div>
            <div class="table-responsive">
                <table class="table align-middle">
                    <thead>
                        <tr>
                            <th class="ps-4">Khách hàng</th>
                            <th>Hạng thẻ</th>
                            <th class="text-end pe-4">Tổng chi tiêu</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var u in Model.TopVipCustomers)
                        {
                            string uInit = AppHelpers.GetInitials(u.FullName);
                            <tr>
                                <td class="ps-4">
                                    <div class="d-flex align-items-center gap-2">
                                        <span class="customer-avatar-circle">@uInit</span>
                                        <div>
                                            <div class="fw-bold small text-dark">@u.FullName</div>
                                            <div class="text-muted" style="font-size:0.74rem">@u.Phone</div>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    @if (u.IsVip)
                                    {
                                        <span class="badge bg-warning-subtle text-warning-emphasis border border-warning-subtle"><i class="bi bi-award-fill me-1"></i>VIP</span>
                                    }
                                    else
                                    {
                                        <span class="badge bg-light text-secondary border">Thành viên</span>
                                    }
                                </td>
                                <td class="text-end pe-4 fw-bold text-danger small">@AppHelpers.FormatVnd(u.TotalSpent)</td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<!-- Báo cáo Kho: Cảnh báo sắp hết hàng & sắp hết hạn tươi -->
<div class="row g-4">
    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h6 class="fw-bold text-danger mb-0"><i class="bi bi-exclamation-triangle-fill me-2"></i>Cảnh Báo Hoa Sắp Hết Hàng (Tồn ≤ 5)</h6>
                <span class="badge bg-danger-subtle text-danger">@Model.LowStockProducts.Count mẫu</span>
            </div>
            @if (!Model.LowStockProducts.Any())
            {
                <div class="p-4 rounded-4 bg-success-subtle text-success small fw-semibold text-center">
                    <i class="bi bi-check-circle-fill me-1"></i> Tất cả sản phẩm đều đủ số lượng an toàn trong kho.
                </div>
            }
            else
            {
                <div class="d-grid gap-2">
                    @foreach (var lp in Model.LowStockProducts)
                    {
                        <div class="p-3 rounded-4 border d-flex justify-content-between align-items-center gap-2" style="background:#fffafb">
                            <div class="d-flex align-items-center gap-3 min-w-0">
                                <img src="@lp.MainImageUrl" class="rounded-3 border" style="width:42px;height:42px;object-fit:cover" alt="" />
                                <div class="min-w-0">
                                    <div class="fw-bold small text-dark text-truncate">@lp.Name</div>
                                    <div class="text-danger" style="font-size:0.75rem">Chỉ còn <strong>@lp.StockQuantity bó</strong> trong kho lạnh</div>
                                </div>
                            </div>
                            <a href="@Url.Action("Edit", "Product", new { id = lp.Id })" class="btn btn-sm btn-dark rounded-pill px-3 text-nowrap">
                                <i class="bi bi-plus-circle me-1"></i>Nhập thêm
                            </a>
                        </div>
                    }
                </div>
            }
        </div>
    </div>

    <div class="col-lg-6">
        <div class="admin-card p-4 h-100">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h6 class="fw-bold text-warning-emphasis mb-0"><i class="bi bi-hourglass-split me-2"></i>Cảnh Báo Mẫu Hoa Sắp Đến Hạn Bảo Quản</h6>
                <span class="badge bg-warning-subtle text-warning-emphasis">@Model.ExpiringProducts.Count mẫu</span>
            </div>
            @if (!Model.ExpiringProducts.Any())
            {
                <div class="p-4 rounded-4 bg-success-subtle text-success small fw-semibold text-center">
                    <i class="bi bi-check-circle-fill me-1"></i> Không có lô hoa nào sắp hết hạn tươi trong 4 ngày tới.
                </div>
            }
            else
            {
                <div class="d-grid gap-2">
                    @foreach (var ep in Model.ExpiringProducts)
                    {
                        <div class="p-3 rounded-4 border d-flex justify-content-between align-items-center gap-2" style="background:#fffdf5">
                            <div class="d-flex align-items-center gap-3 min-w-0">
                                <img src="@ep.MainImageUrl" class="rounded-3 border" style="width:42px;height:42px;object-fit:cover" alt="" />
                                <div class="min-w-0">
                                    <div class="fw-bold small text-dark text-truncate">@ep.Name</div>
                                    <div class="text-warning-emphasis" style="font-size:0.75rem">Hạn tươi dự kiến: <strong>@(ep.ExpiryDate.HasValue ? ep.ExpiryDate.Value.ToString("dd/MM/yyyy") : "")</strong></div>
                                </div>
                            </div>
                            <a href="@Url.Action("Manage", "HotDeal")" class="btn btn-sm btn-admin-primary text-nowrap">
                                <i class="bi bi-fire me-1"></i>Đẩy Hot Deal
                            </a>
                        </div>
                    }
                </div>
            }
        </div>
    </div>
</div>

@section Scripts {
    <script>
        var revLabels = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.RevenueSeries.Select(x => x.Label)));
        var revData = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.RevenueSeries.Select(x => x.Revenue)));
        var profitData = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(Model.RevenueSeries.Select(x => x.GrossProfit)));

        if (window.Chart) {
            Chart.defaults.font.family = "'Plus Jakarta Sans', 'Segoe UI', sans-serif";
            Chart.defaults.color = '#64748b';

            var revCanvas = document.getElementById('revenueChart');
            if (revCanvas) {
                var ctx = revCanvas.getContext('2d');
                var gradRev = ctx.createLinearGradient(0, 0, 0, 280);
                gradRev.addColorStop(0, '#e11d48');
                gradRev.addColorStop(1, '#fb7185');

                var gradProfit = ctx.createLinearGradient(0, 0, 0, 280);
                gradProfit.addColorStop(0, '#f59e0b');
                gradProfit.addColorStop(1, '#fcd34d');

                new Chart(revCanvas, {
                    type: 'bar',
                    data: {
                        labels: revLabels,
                        datasets: [
                            {
                                label: 'Doanh thu thực nhận',
                                data: revData,
                                backgroundColor: gradRev,
                                borderRadius: 10,
                                maxBarThickness: 34
                            },
                            {
                                label: 'Lợi nhuận gộp ước tính',
                                data: profitData,
                                backgroundColor: gradProfit,
                                borderRadius: 10,
                                maxBarThickness: 34
                            }
                        ]
                    },
                    options: {
                        responsive: true,
                        interaction: { mode: 'index', intersect: false },
                        plugins: {
                            legend: {
                                position: 'top',
                                align: 'end',
                                labels: { usePointStyle: true, boxWidth: 8, padding: 16, font: { weight: '600' } }
                            },
                            tooltip: {
                                backgroundColor: '#1e1b2e',
                                padding: 12,
                                cornerRadius: 12,
                                callbacks: {
                                    label: function (context) {
                                        var val = Number(context.raw || 0).toLocaleString('vi-VN');
                                        return ' ' + context.dataset.label + ': ' + val + ' ₫';
                                    }
                                }
                            }
                        },
                        scales: {
                            x: { grid: { display: false }, border: { display: false } },
                            y: {
                                border: { display: false },
                                grid: { color: '#f1f5f9' },
                                ticks: {
                                    callback: function (v) {
                                        if (v >= 1000000) return (v / 1000000).toFixed(1).replace('.0', '') + 'M';
                                        if (v >= 1000) return (v / 1000) + 'K';
                                        return v;
                                    }
                                }
                            }
                        }
                    }
                });
            }

            var statusCanvas = document.getElementById('orderStatusChart');
            if (statusCanvas) {
                new Chart(statusCanvas, {
                    type: 'doughnut',
                    data: {
                        labels: ['Chờ xác nhận', 'Đang xử lý', 'Đang giao', 'Hoàn thành', 'Đã hủy'],
                        datasets: [{
                            data: [@Model.PendingOrders, @Model.ProcessingOrders, @Model.ShippingOrders, @Model.CompletedOrders, @Model.CancelledOrders],
                            backgroundColor: ['#f59e0b', '#3b82f6', '#06b6d4', '#10b981', '#cbd5e1'],
                            borderWidth: 3,
                            borderColor: '#ffffff',
                            hoverOffset: 6
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        cutout: '70%',
                        plugins: {
                            legend: { display: false }
                        }
                    }
                });
            }
        }
    </script>
}
```

### File: FlowerShop/Views\Review\Details.cshtml
```html
@model FlowerShop.Models.Review
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = string.Format("Chi tiết Đánh giá #{0} - CHAM Flowers Executive CMS", Model.Id);
}

<div class="admin-card p-4" style="max-width:680px">
    <div class="d-flex justify-content-between align-items-start mb-3">
        <div>
            <h5 class="fw-bold mb-1">@(Model.Product != null ? Model.Product.Name : "Sản phẩm")</h5>
            <div class="text-muted small">Người gửi: <strong>@(Model.User != null ? Model.User.FullName : "Khách hàng")</strong> (@(Model.User != null ? Model.User.Email : "")) • @Model.CreatedAt.ToString("HH:mm dd/MM/yyyy")</div>
        </div>
        <span class="badge @(Model.IsApproved ? "bg-success" : "bg-warning text-dark")">
            @(Model.IsApproved ? "Đã phê duyệt" : "Chờ phê duyệt")
        </span>
    </div>

    <div class="text-warning mb-2">
        @for (int i = 0; i < Model.Rating; i++)
        {
            <i class="bi bi-star-fill"></i>
        }
        <span class="text-dark fw-semibold ms-1">(@Model.Rating / 5 sao)</span>
    </div>

    <div class="p-3 rounded-3 bg-light border mb-4">@Model.Content</div>

    <div class="d-flex gap-2">
        @if (!Model.IsApproved)
        {
            <form action="@Url.Action("Approve", "Review", new { id = Model.Id })" method="post">
                @Html.AntiForgeryToken()
                <button type="submit" class="btn btn-success rounded-pill px-4"><i class="bi bi-check-lg me-1"></i>Phê duyệt hiển thị</button>
            </form>
        }
        else
        {
            <form action="@Url.Action("Hide", "Review", new { id = Model.Id })" method="post">
                @Html.AntiForgeryToken()
                <button type="submit" class="btn btn-outline-secondary rounded-pill px-4">Ẩn khỏi trang web</button>
            </form>
        }
        <form action="@Url.Action("Delete", "Review", new { id = Model.Id })" method="post" onsubmit="return confirm('Xóa vĩnh viễn đánh giá này?');">
            @Html.AntiForgeryToken()
            <button type="submit" class="btn btn-outline-danger rounded-pill px-3">Xóa</button>
        </form>
        <a href="@Url.Action("Index", "Review")" class="btn btn-light border rounded-pill px-3">Quay lại</a>
    </div>
</div>
```

### File: FlowerShop/Views\Review\Index.cshtml
```html
@model PagedList.IPagedList<FlowerShop.Models.Review>
@using FlowerShop.Models
@{
    Layout = "~/Views/Shared/_LayoutAdmin.cshtml";
    ViewBag.Title = "Quản lý & Phê duyệt Đánh giá Sản phẩm - CHAM Flowers Executive CMS";
    string cur = ViewBag.Filter as string;
}

<div class="admin-card p-3 px-4 mb-4 d-flex flex-wrap gap-2">
    <a href="@Url.Action("Index", "Review")" class="admin-filter-pill @(string.IsNullOrEmpty(cur) ? "active" : "")">Tất cả đánh giá</a>
    <a href="@Url.Action("Index", "Review", new { filter = "pending" })" class="admin-filter-pill @(cur == "pending" ? "active" : "")">Chờ phê duyệt</a>
    <a href="@Url.Action("Index", "Review", new { filter = "approved" })" class="admin-filter-pill @(cur == "approved" ? "active" : "")">Đã hiển thị công khai</a>
    <a href="@Url.Action("Index", "Review", new { filter = "hidden" })" class="admin-filter-pill @(cur == "hidden" ? "active" : "")">Đã ẩn</a>
</div>

<div class="admin-card overflow-hidden">
    <div class="table-responsive">
        <table class="table align-middle mb-0">
            <thead class="table-light small text-muted">
                <tr>
                    <th class="ps-3">Sản phẩm</th>
                    <th>Khách hàng</th>
                    <th>Xếp hạng</th>
                    <th>Nội dung phản hồi</th>
                    <th>Trạng thái</th>
                    <th class="text-end pe-3">Phê duyệt</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var r in Model)
                {
                    <tr>
                        <td class="ps-3">
                            <div class="fw-semibold small">@(r.Product != null ? r.Product.Name : "Sản phẩm")</div>
                            <div class="text-muted" style="font-size:0.72rem">@r.CreatedAt.ToString("dd/MM/yyyy HH:mm")</div>
                        </td>
                        <td class="small">@(r.User != null ? r.User.FullName : "Khách hàng")</td>
                        <td class="text-warning small text-nowrap">
                            @for (int i = 0; i < r.Rating; i++)
                            {
                                <i class="bi bi-star-fill"></i>
                            }
                        </td>
                        <td class="small" style="max-width:340px">@r.Content</td>
                        <td>
                            @if (r.IsApproved)
                            {
                                <span class="badge bg-success">Đã duyệt đăng</span>
                            }
                            else if (r.Status == ReviewStatus.Hidden)
                            {
                                <span class="badge bg-secondary">Đã ẩn</span>
                            }
                            else
                            {
                                <span class="badge bg-warning text-dark">Chờ duyệt</span>
                            }
                        </td>
                        <td class="text-end pe-3">
                            <div class="d-inline-flex gap-1">
                                <a href="@Url.Action("Details", "Review", new { id = r.Id })" class="btn btn-sm btn-light border">Chi tiết</a>
                                @if (!r.IsApproved)
                                {
                                    <form action="@Url.Action("Approve", "Review", new { id = r.Id })" method="post">
                                        @Html.AntiForgeryToken()
                                        <button type="submit" class="btn btn-sm btn-success">Duyệt</button>
                                    </form>
                                }
                                else
                                {
                                    <form action="@Url.Action("Hide", "Review", new { id = r.Id })" method="post">
                                        @Html.AntiForgeryToken()
                                        <button type="submit" class="btn btn-sm btn-outline-secondary">Ẩn</button>
                                    </form>
                                }
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>
```

### File: FlowerShop/Views\Shared\Error.cshtml
```html
@model System.Web.Mvc.HandleErrorInfo
@{
    ViewData["Title"] = "Đã xảy ra lỗi";
}
<div class="container py-5 text-center">
    <h1 class="text-danger fw-bold">Đã xảy ra lỗi xử lý yêu cầu</h1>
    <p class="text-muted">Vui lòng thử lại hoặc quay về trang chủ CHAM Flowers.</p>
    <a href="@Url.Action("Index", "Home")" class="btn btn-nt-primary px-4">Về trang chủ</a>
</div>
```

### File: FlowerShop/Views\Shared\_NavMenu.cshtml
```html
@model IEnumerable<FlowerShop.Models.MenuItem>
@{
    var rawUrl = Request.RawUrl != null ? Request.RawUrl.ToLower() : "/";
    var path = Request.Url != null ? Request.Url.AbsolutePath.ToLower() : "/";
}

<ul class="navbar-nav mx-auto mb-2 mb-xl-0 gap-1">
    @foreach (var item in Model)
    {
        var itemUrl = (item.Url ?? "").Trim();
        var itemUrlLower = itemUrl.ToLower();

        // Kiểm tra xem menu này có đang active không
        bool isActive = false;
        if (itemUrlLower == "/" || itemUrlLower == "/home" || itemUrlLower == "/home/index")
        {
            isActive = (path == "/" || path == "/home" || path == "/home/index");
        }
        else if (!string.IsNullOrEmpty(itemUrlLower) && !itemUrlLower.StartsWith("http"))
        {
            isActive = path.StartsWith(itemUrlLower) || rawUrl.StartsWith(itemUrlLower);
        }

        <li class="nav-item">
            <a class="nav-link nt-nav-link @(isActive ? "active" : "")"
               href="@itemUrl"
               target="@(item.OpenNewTab ? "_blank" : "_self")"
               @(item.OpenNewTab ? "rel=\"noopener noreferrer\"" : "")>
                @if (!string.IsNullOrEmpty(item.Icon))
                {
                    <i class="bi @item.Icon me-1"></i>
                }
                <span>@item.Name</span>
                @if (!string.IsNullOrEmpty(item.BadgeText))
                {
                    <span class="badge bg-@(string.IsNullOrEmpty(item.BadgeColor) ? "danger" : item.BadgeColor) ms-1 rounded-pill" style="font-size: 0.65rem; vertical-align: middle;">
                        @item.BadgeText
                    </span>
                }
            </a>
        </li>
    }
</ul>
```

### File: FlowerShop/Views\Shared\_ProductCard.cshtml
```html
@model FlowerShop.Models.Product

<div class="flower-card">
    <div class="flower-img-wrap">
        @if (Model.DiscountPercent > 0)
        {
            <span class="badge-discount">-@Model.DiscountPercent%</span>
        }
        @if (Model.IsHotDeal)
        {
            <span class="badge-hot"><i class="bi bi-fire"></i> Hot Deal</span>
        }
        <a href="@Url.Action("Details", "Product", new { id = Model.Id })">
            <img src="@Model.MainImageUrl" alt="@Model.Name" loading="lazy" />
        </a>
    </div>
    <div class="p-3 d-flex flex-column flex-grow-1">
        <div class="d-flex justify-content-between align-items-center mb-1">
            <span class="text-muted" style="font-size:0.76rem">@(Model.Category != null ? Model.Category.Name : "Hoa tươi nghệ thuật")</span>
            <span class="text-warning small"><i class="bi bi-star-fill"></i> @Model.AverageRating.ToString("0.0")</span>
        </div>
        <h6 class="fw-bold mb-2 flex-grow-1">
            <a href="@Url.Action("Details", "Product", new { id = Model.Id })" class="text-dark text-decoration-none">
                @Model.Name
            </a>
        </h6>
        <div class="mb-3">
            <span class="flower-price">@AppHelpers.FormatVnd(Model.Price)</span>
            @if (Model.OriginalPrice.HasValue && Model.OriginalPrice > Model.Price)
            {
                <span class="flower-old-price">@AppHelpers.FormatVnd(Model.OriginalPrice.Value)</span>
            }
        </div>
        <div class="d-flex gap-2">
            <a href="@Url.Action("Details", "Product", new { id = Model.Id })" class="btn btn-light border rounded-pill btn-sm flex-grow-1">
                Chi tiết
            </a>
            @if (Model.StockQuantity > 0)
            {
                <button type="button" data-id="@Model.Id" onclick="window['addToCartAjax'] &amp;&amp; window['addToCartAjax'](this.getAttribute('data-id'), 1)" class="btn btn-nt-primary btn-sm flex-grow-1">
                    <i class="bi bi-cart-plus me-1"></i> Thêm giỏ
                </button>
            }
            else
            {
                <button type="button" class="btn btn-secondary btn-sm rounded-pill flex-grow-1" disabled>Hết hàng</button>
            }
        </div>
    </div>
</div>
```

### File: FlowerShop/Views\Shared\_ValidationScriptsPartial.cshtml
```html
<script src="~/Scripts/jquery.validate.min.js"></script>
<script src="~/Scripts/jquery.validate.unobtrusive.min.js"></script>
```

