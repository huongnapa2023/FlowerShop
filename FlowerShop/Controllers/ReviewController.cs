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

        [AdminPermissionAuthorize(AdminPermissions.Review)]
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

        [AdminPermissionAuthorize(AdminPermissions.Review)]
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

        [AdminPermissionAuthorize(AdminPermissions.Review)]
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

        [AdminPermissionAuthorize(AdminPermissions.Review)]
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

        [AdminPermissionAuthorize(AdminPermissions.Review)]
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
