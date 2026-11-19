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
