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

        [AdminPermissionAuthorize(AdminPermissions.Order)]
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

        [AdminPermissionAuthorize(AdminPermissions.Order)]
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

        [AdminPermissionAuthorize("ORDER_EDIT")]
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

        [AdminPermissionAuthorize("ORDER_DELETE")]
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

        [AdminPermissionAuthorize("ORDER_EDIT")]
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

            if (newStatus == "Đã hủy" && !AppHelpers.CanDelete(AdminPermissions.Order))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn chỉ có quyền Thêm & Sửa, không có quyền Hủy / Xóa đơn hàng!";
                return RedirectToAction("AdminDetails", new { id = id });
            }

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

        [AdminPermissionAuthorize("ORDER_CREATE")]
        [HttpGet]
        public ActionResult AdminCreate()
        {
            ViewBag.Products = _context.Products.Where(p => p.IsActive).OrderBy(p => p.Name).ToList();
            ViewBag.Customers = _context.Users.Where(u => u.Role == "Customer" && u.IsActive).OrderBy(u => u.FullName).ToList();
            return View();
        }

        [AdminPermissionAuthorize("ORDER_CREATE")]
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

        [AdminPermissionAuthorize("ORDER_DELETE")]
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
