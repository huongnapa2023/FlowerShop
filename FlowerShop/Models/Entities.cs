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

    /// <summary>
    /// Định nghĩa 9 mã phân quyền quản trị của CHAM Flowers
    /// </summary>
    public static class AdminPermissions
    {
        public const string Report = "REPORT";
        public const string Product = "PRODUCT";
        public const string Category = "CATEGORY";
        public const string Order = "ORDER";
        public const string HotDeal = "HOTDEAL";
        public const string Customer = "CUSTOMER";
        public const string Review = "REVIEW";
        public const string Chat = "CHAT";
        public const string Banner = "BANNER";

        public static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>
        {
            { Report, "Thống kê & Báo cáo doanh thu (Report)" },
            { Product, "Quản lý Sản phẩm & Kho hoa (Product)" },
            { Category, "Quản lý Danh mục, Loại hoa & Nhà cung cấp (Category)" },
            { Order, "Quản lý Đơn hàng & Cập nhật trạng thái (Order)" },
            { HotDeal, "Quản lý Hot Deal & Popup khuyến mãi (HotDeal)" },
            { Customer, "Quản lý Tài khoản Khách hàng (Customer)" },
            { Review, "Kiểm duyệt & Ẩn/Hiện Đánh giá (Review)" },
            { Chat, "Trực Chat Tư vấn Khách hàng (Chat)" },
            { Banner, "Quản lý Banner, Tin tức, Voucher & Menu (Banner)" }
        };

        public static readonly Dictionary<string, string> Badges = new Dictionary<string, string>
        {
            { Report, "bi-grid-1x2-fill" },
            { Product, "bi-flower1" },
            { Category, "bi-tags-fill" },
            { Order, "bi-bag-check-fill" },
            { HotDeal, "bi-fire" },
            { Customer, "bi-people-fill" },
            { Review, "bi-star-half" },
            { Chat, "bi-chat-heart-fill" },
            { Banner, "bi-images" }
        };

        public static List<string> GetAll()
        {
            return new List<string> { Report, Product, Category, Order, HotDeal, Customer, Review, Chat, Banner };
        }

        // Các phân hệ hỗ trợ phân quyền chi tiết 3 cấp (Thêm / Sửa / Xoá)
        public static readonly HashSet<string> CrudModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Product, Category, Order, HotDeal, Customer, Review, Banner
        };

        public static bool SupportsCrud(string moduleCode)
        {
            return !string.IsNullOrWhiteSpace(moduleCode) && CrudModules.Contains(moduleCode.Trim().ToUpperInvariant());
        }

        public static string CreateOf(string moduleCode)
        {
            return (moduleCode ?? string.Empty).Trim().ToUpperInvariant() + "_CREATE";
        }

        public static string EditOf(string moduleCode)
        {
            return (moduleCode ?? string.Empty).Trim().ToUpperInvariant() + "_EDIT";
        }

        public static string DeleteOf(string moduleCode)
        {
            return (moduleCode ?? string.Empty).Trim().ToUpperInvariant() + "_DELETE";
        }
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

        // Phân quyền Quản trị viên (Super Admin & Sub-Admins)
        public bool IsSuperAdmin { get; set; } = false; // Admin Tổng có quyền tối cao

        public string AllowedPermissions { get; set; } = string.Empty; // Chuỗi mã quyền ngăn cách bằng dấu phẩy (vd: "ORDER,PRODUCT")

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

        // Thuộc tính mở rộng cho Popup Builder trực quan
        [MaxLength(30)]
        public string LayoutType { get; set; } = "TopImage"; // TopImage, SplitLeft, PosterOverlay

        [MaxLength(80)]
        public string BadgeText { get; set; } = "FLASH SALE HÔM NAY";

        [MaxLength(80)]
        public string ButtonText { get; set; } = "Xem ngay chương trình khuyến mãi";

        [MaxLength(255)]
        public string TargetUrl { get; set; } = "/HotDeal";

        [MaxLength(20)]
        public string BackgroundColor { get; set; } = "#ffffff";

        [MaxLength(20)]
        public string TextColor { get; set; } = "#1e293b";

        [MaxLength(20)]
        public string AccentColor { get; set; } = "#be123c";

        public int DelaySeconds { get; set; } = 1;

        [MaxLength(30)]
        public string ShowFrequency { get; set; } = "OncePerSession"; // OncePerSession, EveryLoad, OncePerDay

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

    /// <summary>
    /// Chiến dịch / Sự kiện lễ hội theo mùa (Seasonal Campaigns)
    /// Tự động bật/tắt giao diện theo ngày (IsActiveNow), cài đặt sẵn trước không cần can thiệp thủ công.
    /// </summary>
    [Table("Campaigns")]
    public class Campaign
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên chiến dịch")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string SeasonType { get; set; } = "Other"; // 20-11, Noel, Tet, Valentine, 8-3, Other

        [MaxLength(255)]
        public string PromoBarText { get; set; } = string.Empty; // Dòng chữ hiển thị trên thanh topbar

        [MaxLength(255)]
        public string BannerUrl { get; set; } // Ảnh banner trang chủ

        [MaxLength(150)]
        public string BannerTitle { get; set; }

        [MaxLength(300)]
        public string BannerSubtitle { get; set; }

        [MaxLength(255)]
        public string TargetUrl { get; set; } // Link dẫn đến BST hoa của dịp đó

        [MaxLength(50)]
        public string CouponCode { get; set; } // Mã voucher liên kết

        public int? DiscountPercent { get; set; } // % giảm giá

        [MaxLength(30)]
        public string ThemeColor { get; set; } = "#be123c"; // Mã màu chủ đạo (vd: đỏ Noel, vàng Tết...)

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Tự động xác định chiến dịch có đang trong thời gian hiệu lực để hiển thị hay không
        /// </summary>
        [NotMapped]
        public bool IsActiveNow
        {
            get
            {
                var now = DateTime.Now;
                return IsActive && now >= StartDate && now <= EndDate;
            }
        }

        [NotMapped]
        public string StatusLabel
        {
            get
            {
                if (!IsActive) return "Tạm tắt";
                var now = DateTime.Now;
                if (now < StartDate) return "Sắp diễn ra";
                if (now > EndDate) return "Đã kết thúc";
                return "Đang diễn ra";
            }
        }

        [NotMapped]
        public string StatusBadgeClass
        {
            get
            {
                if (!IsActive) return "bg-secondary";
                var now = DateTime.Now;
                if (now < StartDate) return "bg-warning text-dark";
                if (now > EndDate) return "bg-light text-muted border";
                return "bg-success";
            }
        }
    }

    /// <summary>
    /// Cấu hình Tuỳ biến Header & Footer toàn hệ thống (Theme Customizer)
    /// </summary>
    [Table("SiteSettings")]
    public class SiteSetting
    {
        [Key]
        public int Id { get; set; }

        // --- 1. Cấu hình Header & Thanh Topbar ---
        [MaxLength(120)]
        public string ShopName { get; set; } = "CHAM Flowers";

        [MaxLength(255)]
        public string LogoUrl { get; set; } = "/images/logo-cham.svg";

        [MaxLength(50)]
        public string Hotline { get; set; } = "1900 6868";

        [MaxLength(300)]
        public string DefaultTopbarText { get; set; } = "Giao hoa tươi hỏa tốc 2H nội thành TP.HCM • Nhập mã CHAMFLOWERS15 giảm ngay 15%";

        [MaxLength(25)]
        public string TopbarBgColor { get; set; } = "#881337";

        [MaxLength(25)]
        public string HeaderBgColor { get; set; } = "#ffffff";

        public bool ShowTopbar { get; set; } = true;

        public bool ShowSearchBar { get; set; } = true;

        public bool ShowHotDealButton { get; set; } = true;

        // --- 2. Cấu hình Footer (Chân trang) ---
        [MaxLength(255)]
        public string FooterLogoUrl { get; set; } = "/images/logo-cham-white.svg";

        [MaxLength(25)]
        public string FooterBgColor { get; set; } = "#0f172a";

        [MaxLength(25)]
        public string FooterTextColor { get; set; } = "#f8fafc";

        [MaxLength(600)]
        public string FooterAbout { get; set; } = "Thương hiệu hoa tươi nghệ thuật cao cấp chuyên cung cấp hoa sinh nhật, hoa khai trương, lan hồ điệp và hoa cưới thiết kế độc bản. Cam kết hoa tươi mới mỗi ngày từ Đà Lạt & nhập khẩu.";

        [MaxLength(255)]
        public string Address { get; set; } = "128 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh";

        [MaxLength(50)]
        public string SecondaryPhone { get; set; } = "0909 123 456";

        [MaxLength(120)]
        public string Email { get; set; } = "cskh@chamflowers.vn";

        [MaxLength(150)]
        public string WorkingHours { get; set; } = "07:00 - 22:00 (Tất cả các ngày trong tuần)";

        [MaxLength(100)]
        public string FooterCol1Title { get; set; } = "Danh mục hoa";

        public string FooterCol1Links { get; set; } = "Tất cả mẫu hoa | /Product\nHot Deal giảm giá | /HotDeal\nCẩm nang chăm hoa | /Home/News\nVề CHAM Flowers | /Home/About";

        [MaxLength(100)]
        public string FooterCol2Title { get; set; } = "Hỗ trợ khách hàng";

        public string FooterCol2Links { get; set; } = "Giỏ hàng & Mã giảm giá | /Order/Cart\nTra cứu trạng thái đơn hàng | /Order/MyOrders\nThanh toán bảo mật COD & VNPay | #\nBảo hành độ tươi hoa 72 giờ | #";

        [MaxLength(255)]
        public string FacebookUrl { get; set; } = "https://facebook.com";

        [MaxLength(255)]
        public string InstagramUrl { get; set; } = "https://instagram.com";

        [MaxLength(255)]
        public string ZaloUrl { get; set; } = "https://zalo.me/0909123456";

        [MaxLength(255)]
        public string TikTokUrl { get; set; } = "https://tiktok.com";

        [MaxLength(255)]
        public string CopyrightText { get; set; } = "© 2026 CHAM Flowers Boutique. Thiết kế hoa tươi nghệ thuật cao cấp.";

        public bool ShowFloatingChat { get; set; } = true;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
