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
