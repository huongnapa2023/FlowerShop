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

        [AdminPermissionAuthorize(AdminPermissions.Product)]
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

        [AdminPermissionAuthorize("PRODUCT_CREATE")]
        [HttpGet]
        public ActionResult Create()
        {
            PopulateDropdowns();
            return View(new Product { IsActive = true, StockQuantity = 20, ExpiryDate = DateTime.Now.AddDays(7) });
        }

        [AdminPermissionAuthorize("PRODUCT_CREATE")]
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

        [AdminPermissionAuthorize("PRODUCT_EDIT")]
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

        [AdminPermissionAuthorize("PRODUCT_EDIT")]
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

        [AdminPermissionAuthorize("PRODUCT_DELETE")]
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

        [AdminPermissionAuthorize(AdminPermissions.Category)]
        public ActionResult Categories()
        {
            ViewBag.Categories = _context.Categories.Include(c => c.Products).OrderBy(c => c.Id).ToList();
            ViewBag.FlowerCategories = _context.FlowerCategories.Include(fc => fc.Products).OrderBy(fc => fc.Id).ToList();
            ViewBag.Suppliers = _context.Suppliers.Include(s => s.Products).OrderBy(s => s.Id).ToList();
            return View();
        }

        [AdminPermissionAuthorize(AdminPermissions.Category)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategory(int id, string name, bool isActive = true)
        {
            if (id == 0 && !AppHelpers.CanCreate(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Thêm mới danh mục.";
                return RedirectToAction("Categories");
            }
            if (id > 0 && !AppHelpers.CanEdit(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Chỉnh sửa danh mục.";
                return RedirectToAction("Categories");
            }

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

        [AdminPermissionAuthorize("CATEGORY_DELETE")]
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

        [AdminPermissionAuthorize(AdminPermissions.Category)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveFlowerCategory(int id, string name, string description, string imageUrl)
        {
            if (id == 0 && !AppHelpers.CanCreate(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Thêm mới loại hoa.";
                return RedirectToAction("Categories");
            }
            if (id > 0 && !AppHelpers.CanEdit(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Chỉnh sửa loại hoa.";
                return RedirectToAction("Categories");
            }

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

        [AdminPermissionAuthorize("CATEGORY_DELETE")]
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

        [AdminPermissionAuthorize(AdminPermissions.Category)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSupplier(int id, string name, string phone, string email, string address, bool isActive = true)
        {
            if (id == 0 && !AppHelpers.CanCreate(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Thêm mới nhà cung cấp.";
                return RedirectToAction("Categories");
            }
            if (id > 0 && !AppHelpers.CanEdit(AdminPermissions.Category))
            {
                TempData["Error"] = "⛔ Giới hạn cơ sở: Tài khoản của bạn không có quyền Chỉnh sửa nhà cung cấp.";
                return RedirectToAction("Categories");
            }

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

        [AdminPermissionAuthorize("CATEGORY_DELETE")]
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
