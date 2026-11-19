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

            // Đảm bảo tự động migrate các cột phân quyền RBAC và nạp tài khoản Admin con mẫu
            EnsureUserRbacColumnsAndSeedSubAdmins(context);

            // Đảm bảo bảng Campaigns tồn tại và nạp các chiến dịch theo mùa mẫu
            EnsureCampaignsTableAndSeed(context);

            // Nâng cấp các banner mặc định thành ảnh Hero Panorama 16:9 sắc nét, chuẩn phong cách Boutique
            EnsureHeroBannersAreHighQuality(context);

            // Đảm bảo tự động migrate các cột Popup Builder và bảng cấu hình Header/Footer (SiteSettings)
            EnsurePopupBuilderAndSiteSettings(context);

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

        public static void EnsureUserRbacColumnsAndSeedSubAdmins(FlowerShopDbContext context)
        {
            try
            {
                // 1. Tự động thêm cột IsSuperAdmin và AllowedPermissions vào bảng Users nếu chưa có (Migration an toàn)
                string sqlAddColumns = @"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'IsSuperAdmin')
                    BEGIN
                        ALTER TABLE [dbo].[Users] ADD [IsSuperAdmin] BIT NOT NULL CONSTRAINT [DF_Users_IsSuperAdmin] DEFAULT 0;
                    END

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'AllowedPermissions')
                    BEGIN
                        ALTER TABLE [dbo].[Users] ADD [AllowedPermissions] NVARCHAR(MAX) NULL;
                    END
                ";
                context.Database.ExecuteSqlCommand(sqlAddColumns);

                // 2. Cập nhật tài khoản admin@chamflowers.vn thành Admin Tổng tối cao (Toàn bộ 9 quyền)
                string fullPerms = "REPORT,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,PRODUCT_DELETE,CATEGORY,CATEGORY_CREATE,CATEGORY_EDIT,CATEGORY_DELETE,ORDER,ORDER_CREATE,ORDER_EDIT,ORDER_DELETE,HOTDEAL,HOTDEAL_CREATE,HOTDEAL_EDIT,HOTDEAL_DELETE,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,CUSTOMER_DELETE,REVIEW,REVIEW_EDIT,REVIEW_DELETE,CHAT,BANNER,BANNER_CREATE,BANNER_EDIT,BANNER_DELETE,CRUD_GRANULAR";
                string adminPass = AppHelpers.HashPassword("Admin@123");

                string sqlUpdateAdmin = @"
                    UPDATE [dbo].[Users]
                    SET [IsSuperAdmin] = 1,
                        [Role] = 'Admin',
                        [Address] = N'Trụ sở Tổng (Toàn hệ thống)',
                        [AllowedPermissions] = @p0,
                        [IsActive] = 1
                    WHERE [Email] = 'admin@chamflowers.vn' OR ([Role] = 'Admin' AND [Email] NOT IN ('admin1@chamflowers.vn', 'admin2@chamflowers.vn') AND [IsSuperAdmin] = 1);
                ";
                context.Database.ExecuteSqlCommand(sqlUpdateAdmin, fullPerms);

                // 3. Khởi tạo / Nâng cấp tài khoản mẫu Admin 1 (admin1@chamflowers.vn) - CƠ SỞ HÀ NỘI (Đầy đủ Thêm + Sửa + Xóa)
                string hanoiPerms = "ORDER,ORDER_CREATE,ORDER_EDIT,ORDER_DELETE,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,PRODUCT_DELETE,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,CUSTOMER_DELETE,CRUD_GRANULAR";
                int countAdmin1 = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[Users] WHERE [Email] = 'admin1@chamflowers.vn'").FirstOrDefault();
                if (countAdmin1 == 0)
                {
                    string sqlInsertAdmin1 = @"
                        INSERT INTO [dbo].[Users] ([FullName], [Full_name], [Email], [Password], [Phone], [Address], [Role], [IsActive], [IsVip], [TotalSpent], [CreatedAt], [IsSuperAdmin], [AllowedPermissions])
                        VALUES (N'Nguyễn Văn An (Admin Cơ sở Hà Nội)', N'Nguyễn Văn An', 'admin1@chamflowers.vn', @p0, '0901112233', N'Cơ sở Hà Nội', 'Admin', 1, 0, 0, GETDATE(), 0, @p1);
                    ";
                    context.Database.ExecuteSqlCommand(sqlInsertAdmin1, adminPass, hanoiPerms);
                }
                else
                {
                    // Tự động nâng cấp dữ liệu mẫu cũ của admin1 lên cấu hình Cơ sở Hà Nội (Đủ Thêm/Sửa/Xóa)
                    string sqlUpgradeAdmin1 = @"
                        UPDATE [dbo].[Users]
                        SET [FullName] = N'Nguyễn Văn An (Admin Cơ sở Hà Nội)',
                            [Address] = N'Cơ sở Hà Nội',
                            [AllowedPermissions] = @p0
                        WHERE [Email] = 'admin1@chamflowers.vn' AND ([AllowedPermissions] IS NULL OR [AllowedPermissions] NOT LIKE '%CRUD_GRANULAR%');
                    ";
                    context.Database.ExecuteSqlCommand(sqlUpgradeAdmin1, hanoiPerms);
                }

                // 4. Khởi tạo / Nâng cấp tài khoản mẫu Admin 2 (admin2@chamflowers.vn) - CƠ SỞ BẮC NINH (Chỉ Thêm + Sửa, KHÔNG CÓ XÓA)
                string bacninhPerms = "ORDER,ORDER_CREATE,ORDER_EDIT,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,CHAT,CRUD_GRANULAR";
                int countAdmin2 = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[Users] WHERE [Email] = 'admin2@chamflowers.vn'").FirstOrDefault();
                if (countAdmin2 == 0)
                {
                    string sqlInsertAdmin2 = @"
                        INSERT INTO [dbo].[Users] ([FullName], [Full_name], [Email], [Password], [Phone], [Address], [Role], [IsActive], [IsVip], [TotalSpent], [CreatedAt], [IsSuperAdmin], [AllowedPermissions])
                        VALUES (N'Lê Thu Trang (Admin Cơ sở Bắc Ninh)', N'Lê Thu Trang', 'admin2@chamflowers.vn', @p0, '0904445566', N'Cơ sở Bắc Ninh', 'Admin', 1, 0, 0, GETDATE(), 0, @p1);
                    ";
                    context.Database.ExecuteSqlCommand(sqlInsertAdmin2, adminPass, bacninhPerms);
                }
                else
                {
                    // Tự động nâng cấp dữ liệu mẫu cũ của admin2 lên cấu hình Cơ sở Bắc Ninh (Chỉ Thêm/Sửa - Cấm Xóa)
                    string sqlUpgradeAdmin2 = @"
                        UPDATE [dbo].[Users]
                        SET [FullName] = N'Lê Thu Trang (Admin Cơ sở Bắc Ninh)',
                            [Address] = N'Cơ sở Bắc Ninh',
                            [AllowedPermissions] = @p0
                        WHERE [Email] = 'admin2@chamflowers.vn' AND ([AllowedPermissions] IS NULL OR [AllowedPermissions] NOT LIKE '%CRUD_GRANULAR%');
                    ";
                    context.Database.ExecuteSqlCommand(sqlUpgradeAdmin2, bacninhPerms);
                }

                // 5. Bổ sung danh sách nhân viên & quản lý các chi nhánh phong phú (admin3 -> admin10)
                var branchStaffSeeds = new[]
                {
                    new {
                        Email = "admin3@chamflowers.vn",
                        FullName = "Trần Minh Hoàng (NV Bán hàng - Hà Nội)",
                        ShortName = "Trần Minh Hoàng",
                        Phone = "0903112244",
                        Branch = "Cơ sở Hà Nội",
                        Perms = "ORDER,ORDER_CREATE,ORDER_EDIT,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,CHAT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin4@chamflowers.vn",
                        FullName = "Phạm Hoàng Yến (Trưởng Cơ sở Hải Phòng)",
                        ShortName = "Phạm Hoàng Yến",
                        Phone = "0904223355",
                        Branch = "Cơ sở Hải Phòng",
                        Perms = "REPORT,ORDER,ORDER_CREATE,ORDER_EDIT,ORDER_DELETE,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,PRODUCT_DELETE,CATEGORY,CATEGORY_CREATE,CATEGORY_EDIT,CHAT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin5@chamflowers.vn",
                        FullName = "Vũ Đức Thắng (Thủ kho Hoa - Bắc Ninh)",
                        ShortName = "Vũ Đức Thắng",
                        Phone = "0905334466",
                        Branch = "Cơ sở Bắc Ninh",
                        Perms = "PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,CATEGORY,CATEGORY_CREATE,CATEGORY_EDIT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin6@chamflowers.vn",
                        FullName = "Đỗ Ngọc Mai (Trưởng Cơ sở Đà Nẵng)",
                        ShortName = "Đỗ Ngọc Mai",
                        Phone = "0906445577",
                        Branch = "Cơ sở Đà Nẵng",
                        Perms = "REPORT,ORDER,ORDER_CREATE,ORDER_EDIT,ORDER_DELETE,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,PRODUCT_DELETE,HOTDEAL,HOTDEAL_CREATE,HOTDEAL_EDIT,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,CHAT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin7@chamflowers.vn",
                        FullName = "Bùi Thanh Tùng (NV Điều phối - Đà Nẵng)",
                        ShortName = "Bùi Thanh Tùng",
                        Phone = "0907556688",
                        Branch = "Cơ sở Đà Nẵng",
                        Perms = "ORDER,ORDER_EDIT,CHAT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin8@chamflowers.vn",
                        FullName = "Hoàng Bảo Châu (Marketing & Sự kiện)",
                        ShortName = "Hoàng Bảo Châu",
                        Phone = "0908667799",
                        Branch = "Cơ sở TP.HCM (Quận 1)",
                        Perms = "HOTDEAL,HOTDEAL_CREATE,HOTDEAL_EDIT,HOTDEAL_DELETE,BANNER,BANNER_CREATE,BANNER_EDIT,BANNER_DELETE,REVIEW,REVIEW_EDIT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin9@chamflowers.vn",
                        FullName = "Ngô Tuấn Kiệt (Quản lý Cơ sở Quảng Ninh)",
                        ShortName = "Ngô Tuấn Kiệt",
                        Phone = "0909778800",
                        Branch = "Cơ sở Quảng Ninh",
                        Perms = "ORDER,ORDER_CREATE,ORDER_EDIT,PRODUCT,PRODUCT_CREATE,PRODUCT_EDIT,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,REVIEW,REVIEW_EDIT,CHAT,CRUD_GRANULAR"
                    },
                    new {
                        Email = "admin10@chamflowers.vn",
                        FullName = "Đặng Khánh Linh (CSKH & Tư vấn Online)",
                        ShortName = "Đặng Khánh Linh",
                        Phone = "0912889911",
                        Branch = "Cơ sở TP.HCM (Thảo Điền)",
                        Perms = "ORDER,ORDER_CREATE,CUSTOMER,CUSTOMER_CREATE,CUSTOMER_EDIT,REVIEW,REVIEW_EDIT,CHAT,CRUD_GRANULAR"
                    }
                };

                foreach (var staff in branchStaffSeeds)
                {
                    int exists = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[Users] WHERE [Email] = @p0", staff.Email).FirstOrDefault();
                    if (exists == 0)
                    {
                        string sqlInsertStaff = @"
                            INSERT INTO [dbo].[Users] ([FullName], [Full_name], [Email], [Password], [Phone], [Address], [Role], [IsActive], [IsVip], [TotalSpent], [CreatedAt], [IsSuperAdmin], [AllowedPermissions])
                            VALUES (@p0, @p1, @p2, @p3, @p4, @p5, 'Admin', 1, 0, 0, GETDATE(), 0, @p6);
                        ";
                        context.Database.ExecuteSqlCommand(sqlInsertStaff, staff.FullName, staff.ShortName, staff.Email, adminPass, staff.Phone, staff.Branch, staff.Perms);
                    }
                }

                // 6. Tự động nạp danh sách Khách hàng mẫu phong phú (tách biệt hoàn toàn khỏi nhân viên)
                EnsureSeedCustomers(context);
            }
            catch
            {
                // Tránh throw exception làm gián đoạn ứng dụng
            }
        }

        public static void EnsureSeedCustomers(FlowerShopDbContext context)
        {
            try
            {
                string custPass = AppHelpers.HashPassword("Khach@123");
                var customerSeeds = new[]
                {
                    new {
                        Email = "nguyenha.my@gmail.com",
                        FullName = "Nguyễn Hà My",
                        Phone = "0912113355",
                        Address = "Số 15 Chùa Bộc, Quận Đống Đa, Hà Nội",
                        BirthDate = new DateTime(1998, 3, 14),
                        TotalSpent = 3450000m,
                        IsVip = true,
                        IsActive = true
                    },
                    new {
                        Email = "vuhoang.phuong@gmail.com",
                        FullName = "Vũ Hoàng Phương",
                        Phone = "0934224466",
                        Address = "Tòa Discovery Complex, 302 Cầu Giấy, Hà Nội",
                        BirthDate = new DateTime(2001, 7, 20),
                        TotalSpent = 850000m,
                        IsVip = false,
                        IsActive = true
                    },
                    new {
                        Email = "dinhquang.thang@gmail.com",
                        FullName = "Đinh Quang Thắng",
                        Phone = "0945335577",
                        Address = "KĐT Dabaco, Đường Lý Thái Tổ, TP. Bắc Ninh",
                        BirthDate = new DateTime(1994, 11, 8),
                        TotalSpent = 2750000m,
                        IsVip = true,
                        IsActive = true
                    },
                    new {
                        Email = "lequynh.nga@gmail.com",
                        FullName = "Lê Quỳnh Nga",
                        Phone = "0966446688",
                        Address = "68 Đinh Tiên Hoàng, Quận Hồng Bàng, TP. Hải Phòng",
                        BirthDate = new DateTime(2000, 9, 2),
                        TotalSpent = 620000m,
                        IsVip = false,
                        IsActive = true
                    },
                    new {
                        Email = "nguyenminh.tri@gmail.com",
                        FullName = "Nguyễn Minh Trí",
                        Phone = "0977557799",
                        Address = "12 Bạch Đằng, Quận Hải Châu, TP. Đà Nẵng",
                        BirthDate = new DateTime(1991, 4, 25),
                        TotalSpent = 4120000m,
                        IsVip = true,
                        IsActive = true
                    },
                    new {
                        Email = "tranthi.mai@gmail.com",
                        FullName = "Trần Thị Mai",
                        Phone = "0988668800",
                        Address = "Số 25 Trần Hưng Đạo, TP. Hạ Long, Quảng Ninh",
                        BirthDate = new DateTime(1997, 12, 19),
                        TotalSpent = 490000m,
                        IsVip = false,
                        IsActive = true
                    },
                    new {
                        Email = "duonggia.huy@gmail.com",
                        FullName = "Dương Gia Huy",
                        Phone = "0909112233",
                        Address = "Khu đô thị Phú Mỹ Hưng, Quận 7, TP.HCM",
                        BirthDate = new DateTime(1989, 6, 30),
                        TotalSpent = 5600000m,
                        IsVip = true,
                        IsActive = true
                    },
                    new {
                        Email = "hoangthu.trang@gmail.com",
                        FullName = "Hoàng Thu Trang",
                        Phone = "0918223344",
                        Address = "82 Nam Kỳ Khởi Nghĩa, Quận 3, TP.HCM",
                        BirthDate = new DateTime(2002, 1, 15),
                        TotalSpent = 750000m,
                        IsVip = false,
                        IsActive = true
                    },
                    new {
                        Email = "dangvan.long@gmail.com",
                        FullName = "Đặng Văn Long",
                        Phone = "0929334455",
                        Address = "Khu Công Nghệ Cao, TP. Thủ Đức, TP.HCM",
                        BirthDate = new DateTime(1995, 8, 10),
                        TotalSpent = 300000m,
                        IsVip = false,
                        IsActive = false
                    }
                };

                foreach (var c in customerSeeds)
                {
                    int count = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[Users] WHERE [Email] = @p0", c.Email).FirstOrDefault();
                    if (count == 0)
                    {
                        string sqlInsertCust = @"
                            INSERT INTO [dbo].[Users] ([FullName], [Full_name], [Email], [Password], [Phone], [Address], [BirthDate], [Role], [IsActive], [IsVip], [TotalSpent], [CreatedAt], [IsSuperAdmin], [AllowedPermissions])
                            VALUES (@p0, @p0, @p1, @p2, @p3, @p4, @p5, 'Customer', @p6, @p7, @p8, GETDATE(), 0, NULL);
                        ";
                        context.Database.ExecuteSqlCommand(sqlInsertCust, c.FullName, c.Email, custPass, c.Phone, c.Address, c.BirthDate, c.IsActive ? 1 : 0, c.IsVip ? 1 : 0, c.TotalSpent);
                    }
                }
            }
            catch
            {
            }
        }

        private static void EnsureCampaignsTableAndSeed(FlowerShopDbContext context)
        {
            try
            {
                // 1. Tạo bảng Campaigns nếu chưa tồn tại
                string sqlCreateTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Campaigns')
                    BEGIN
                        CREATE TABLE [dbo].[Campaigns] (
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [Name] NVARCHAR(150) NOT NULL,
                            [SeasonType] NVARCHAR(50) NULL DEFAULT 'Other',
                            [PromoBarText] NVARCHAR(255) NULL,
                            [BannerUrl] NVARCHAR(255) NULL,
                            [BannerTitle] NVARCHAR(150) NULL,
                            [BannerSubtitle] NVARCHAR(300) NULL,
                            [TargetUrl] NVARCHAR(255) NULL,
                            [CouponCode] NVARCHAR(50) NULL,
                            [DiscountPercent] INT NULL,
                            [ThemeColor] NVARCHAR(30) NULL DEFAULT '#be123c',
                            [StartDate] DATETIME NOT NULL,
                            [EndDate] DATETIME NOT NULL,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
                            [UpdatedAt] DATETIME NULL
                        );
                    END
                ";
                context.Database.ExecuteSqlCommand(sqlCreateTable);

                // 2. Nạp dữ liệu mẫu các chiến dịch nếu bảng đang trống
                int count = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[Campaigns]").FirstOrDefault();
                if (count == 0)
                {
                    DateTime now = DateTime.Now;

                    // Chiến dịch 1: ĐANG DIỄN RA (Tri Ân Thầy Cô 20/11)
                    DateTime start1 = now.AddDays(-3);
                    DateTime end1 = now.AddDays(15);
                    string sqlSeed1 = @"
                        INSERT INTO [dbo].[Campaigns] 
                        ([Name], [SeasonType], [PromoBarText], [BannerUrl], [BannerTitle], [BannerSubtitle], [TargetUrl], [CouponCode], [DiscountPercent], [ThemeColor], [StartDate], [EndDate], [IsActive], [CreatedAt])
                        VALUES 
                        (N'Tri Ân Thầy Cô 20/11', '20-11', 
                         N'Mùa Tri Ân 20/11: BST Hoa Tươi Tặng Thầy Cô • Nhập mã TRIAN2011 giảm 20% + Tặng thiệp chúc mừng cao cấp',
                         'https://images.unsplash.com/photo-1587317996270-908c352f1b09?auto=format&fit=crop&w=1920&q=85',
                         N'MÙA TRI ÂN 20/11 - TÔN VINH NGHỀ GIÁO',
                         N'Gửi trọn lòng biết ơn sâu sắc qua từng đóa hoa tươi thắm cùng ưu đãi 20%',
                         '/Product?search=20/11', 'TRIAN2011', 20, '#881337', @p0, @p1, 1, GETDATE());
                    ";
                    context.Database.ExecuteSqlCommand(sqlSeed1, start1, end1);

                    // Chiến dịch 2: SẮP DIỄN RA (Giáng Sinh Noel 2026)
                    DateTime start2 = now.AddDays(25);
                    DateTime end2 = now.AddDays(50);
                    string sqlSeed2 = @"
                        INSERT INTO [dbo].[Campaigns] 
                        ([Name], [SeasonType], [PromoBarText], [BannerUrl], [BannerTitle], [BannerSubtitle], [TargetUrl], [CouponCode], [DiscountPercent], [ThemeColor], [StartDate], [EndDate], [IsActive], [CreatedAt])
                        VALUES 
                        (N'Giáng Sinh Rực Rỡ Noel 2026', 'Noel', 
                         N'Mừng Giáng Sinh Rực Rỡ: Tặng Hộp Quà Giáng Sinh Tuyết Trắng Cho Đơn Từ 499k • Mã NOEL25',
                         'https://images.unsplash.com/photo-1576895225168-a13e88ce5fae?auto=format&fit=crop&w=1920&q=85',
                         N'GIÁNG SINH ẤM ÁP - NOEL 2026',
                         N'Sắc đỏ ấm áp, thông xanh ngập tràn niềm vui an lành và gắn kết',
                         '/Product?search=Noel', 'NOEL25', 25, '#15803d', @p0, @p1, 1, GETDATE());
                    ";
                    context.Database.ExecuteSqlCommand(sqlSeed2, start2, end2);

                    // Chiến dịch 3: SẮP DIỄN RA (Tết Sum Vầy 2027)
                    DateTime start3 = now.AddDays(65);
                    DateTime end3 = now.AddDays(95);
                    string sqlSeed3 = @"
                        INSERT INTO [dbo].[Campaigns] 
                        ([Name], [SeasonType], [PromoBarText], [BannerUrl], [BannerTitle], [BannerSubtitle], [TargetUrl], [CouponCode], [DiscountPercent], [ThemeColor], [StartDate], [EndDate], [IsActive], [CreatedAt])
                        VALUES 
                        (N'Tết Sum Vầy - Đón Xuân May Mắn 2027', 'Tet', 
                         N'Tết Rộn Ràng - Đón Xuân Như Ý: Giảm 15% Hoa Mai, Đào, Lan Hồ Điệp Khai Xuân • Mã TET2027',
                         'https://images.unsplash.com/photo-1571446914179-df4ab952de19?auto=format&fit=crop&w=1920&q=85',
                         N'TẾT SUM VẦY - ĐÓN XUÂN NHƯ Ý',
                         N'Rạng rỡ sắc xuân tài lộc với Lan Hồ Điệp và Hoa Tết tuyển chọn',
                         '/Product?search=Tet', 'TET2027', 15, '#d97706', @p0, @p1, 1, GETDATE());
                    ";
                    context.Database.ExecuteSqlCommand(sqlSeed3, start3, end3);

                    // Chiến dịch 4: SẮP DIỄN RA (Valentine 14/2 - Lời Yêu Ngọt Ngào)
                    DateTime start4 = now.AddDays(100);
                    DateTime end4 = now.AddDays(115);
                    string sqlSeed4 = @"
                        INSERT INTO [dbo].[Campaigns] 
                        ([Name], [SeasonType], [PromoBarText], [BannerUrl], [BannerTitle], [BannerSubtitle], [TargetUrl], [CouponCode], [DiscountPercent], [ThemeColor], [StartDate], [EndDate], [IsActive], [CreatedAt])
                        VALUES 
                        (N'Valentine 14/2 - Lời Yêu Ngọt Ngào', 'Valentine', 
                         N'Valentine Day 14/2: 999 Đóa Hồng Đỏ Tình Yêu • Giảm 30% Khi Đặt Sớm • Nhập mã LOVE2027',
                         'https://images.unsplash.com/photo-1563436798345-8c9b4b97b8d0?auto=format&fit=crop&w=1920&q=85',
                         N'VALENTINE 14/2 - GỬI TRỌN YÊU THƯƠNG',
                         N'Hoa hồng đỏ nhập khẩu cao cấp - Minh chứng hoàn hảo cho tình yêu vĩnh cửu',
                         '/Product?search=Valentine', 'LOVE2027', 30, '#ec4899', @p0, @p1, 1, GETDATE());
                    ";
                    context.Database.ExecuteSqlCommand(sqlSeed4, start4, end4);
                }
            }
            catch
            {
                // Tránh throw exception làm gián đoạn ứng dụng
            }
        }

        private static void EnsureHeroBannersAreHighQuality(FlowerShopDbContext context)
        {
            try
            {
                // 1. Tự động nâng cấp các banner mặc định sang ảnh khổ ngang Panorama 16:9 sắc nét, chuẩn phong cách Floral Boutique
                string sqlUpdateBanners = @"
                    UPDATE [dbo].[Banners]
                    SET [ImageUrl] = 'https://images.unsplash.com/photo-1561181286-d3fee7d55364?auto=format&fit=crop&w=1920&q=85'
                    WHERE [ImageUrl] LIKE '%editorial-2.jpg%' OR [Title] LIKE N'%Trao Hoa Tươi%';

                    UPDATE [dbo].[Banners]
                    SET [ImageUrl] = 'https://images.unsplash.com/photo-1526047932273-341f2a7631f9?auto=format&fit=crop&w=1920&q=85'
                    WHERE [ImageUrl] LIKE '%b10-main.jpg%' OR [Title] LIKE N'%Ưu Đãi Hot Deal%';

                    UPDATE [dbo].[Banners]
                    SET [ImageUrl] = 'https://images.unsplash.com/photo-1582794543139-8ac9cb0f7b11?auto=format&fit=crop&w=1920&q=85'
                    WHERE [ImageUrl] LIKE '%b6-sub2.jpg%' OR [Title] LIKE N'%Khai Trương%' OR [Title] LIKE N'%Lan Hồ Điệp%';
                ";
                context.Database.ExecuteSqlCommand(sqlUpdateBanners);

                // 2. Tự động nâng cấp ảnh Banner Hero của các chiến dịch theo mùa thành ảnh hoa nghệ thuật đúng chuẩn chủ đề
                string sqlUpdateCampaigns = @"
                    -- Tri Ân Thầy Cô 20/11: Bó hoa tươi nghệ thuật phối màu cam hồng rực rỡ tặng thầy cô
                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1587317996270-908c352f1b09?auto=format&fit=crop&w=1920&q=85'
                    WHERE [SeasonType] = '20-11' OR [Name] LIKE N'%20/11%' OR [Name] LIKE N'%Tri Ân%';

                    -- Giáng Sinh Noel: Vòng hoa Noel, hoa Trạng Nguyên đỏ rực rỡ, cành thông tuyết & quả thông
                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1576895225168-a13e88ce5fae?auto=format&fit=crop&w=1920&q=85'
                    WHERE [SeasonType] = 'Noel' OR [Name] LIKE N'%Giáng Sinh%' OR [Name] LIKE N'%Noel%';

                    -- Tết Sum Vầy: Hoa Cúc Mâm Xôi Vàng / Hoa Mai Đào rực rỡ sắc xuân đón Tết tài lộc
                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1571446914179-df4ab952de19?auto=format&fit=crop&w=1920&q=85'
                    WHERE [SeasonType] = 'Tet' OR [Name] LIKE N'%Tết%' OR [Name] LIKE N'%Xuân%';

                    -- Valentine 14/2: Bó hoa hồng nhung đỏ Ecuador rực rỡ phối hoa baby trắng tinh khôi
                    UPDATE [dbo].[Campaigns]
                    SET [BannerUrl] = 'https://images.unsplash.com/photo-1563436798345-8c9b4b97b8d0?auto=format&fit=crop&w=1920&q=85'
                    WHERE [SeasonType] = 'Valentine' OR [Name] LIKE N'%Valentine%' OR [Name] LIKE N'%Yêu%';
                ";
                context.Database.ExecuteSqlCommand(sqlUpdateCampaigns);
            }
            catch
            {
                // Tránh gián đoạn ứng dụng
            }
        }

        public static void EnsurePopupBuilderAndSiteSettings(FlowerShopDbContext context)
        {
            try
            {
                // 1. Tự động thêm các cột mới của Popup Builder vào bảng HotDealPopups nếu chưa có
                string sqlAlterPopup = @"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'LayoutType')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [LayoutType] NVARCHAR(30) NULL DEFAULT 'TopImage';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'BadgeText')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [BadgeText] NVARCHAR(80) NULL DEFAULT N'FLASH SALE HÔM NAY';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'ButtonText')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [ButtonText] NVARCHAR(80) NULL DEFAULT N'Xem ngay chương trình khuyến mãi';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'TargetUrl')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [TargetUrl] NVARCHAR(255) NULL DEFAULT '/HotDeal';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'BackgroundColor')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [BackgroundColor] NVARCHAR(20) NULL DEFAULT '#ffffff';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'TextColor')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [TextColor] NVARCHAR(20) NULL DEFAULT '#1e293b';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'AccentColor')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [AccentColor] NVARCHAR(20) NULL DEFAULT '#be123c';

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'DelaySeconds')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [DelaySeconds] INT NOT NULL CONSTRAINT [DF_HotDealPopups_DelaySeconds] DEFAULT 1;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HotDealPopups') AND name = 'ShowFrequency')
                        ALTER TABLE [dbo].[HotDealPopups] ADD [ShowFrequency] NVARCHAR(30) NULL DEFAULT 'OncePerSession';
                ";
                context.Database.ExecuteSqlCommand(sqlAlterPopup);

                // Cập nhật giá trị mặc định cho các bản ghi HotDealPopups cũ bị NULL
                string sqlFixNullPopup = @"
                    UPDATE [dbo].[HotDealPopups] SET [LayoutType] = 'TopImage' WHERE [LayoutType] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [BadgeText] = N'FLASH SALE HÔM NAY' WHERE [BadgeText] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [ButtonText] = N'Xem ngay chương trình khuyến mãi' WHERE [ButtonText] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [TargetUrl] = '/HotDeal' WHERE [TargetUrl] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [BackgroundColor] = '#ffffff' WHERE [BackgroundColor] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [TextColor] = '#1e293b' WHERE [TextColor] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [AccentColor] = '#be123c' WHERE [AccentColor] IS NULL;
                    UPDATE [dbo].[HotDealPopups] SET [ShowFrequency] = 'OncePerSession' WHERE [ShowFrequency] IS NULL;
                ";
                context.Database.ExecuteSqlCommand(sqlFixNullPopup);

                // 2. Tạo bảng SiteSettings (Tuỳ biến Header & Footer) nếu chưa tồn tại
                string sqlCreateSettings = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SiteSettings')
                    BEGIN
                        CREATE TABLE [dbo].[SiteSettings] (
                            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [ShopName] NVARCHAR(120) NULL,
                            [LogoUrl] NVARCHAR(255) NULL,
                            [Hotline] NVARCHAR(50) NULL,
                            [DefaultTopbarText] NVARCHAR(300) NULL,
                            [TopbarBgColor] NVARCHAR(25) NULL,
                            [HeaderBgColor] NVARCHAR(25) NULL,
                            [ShowTopbar] BIT NOT NULL DEFAULT 1,
                            [ShowSearchBar] BIT NOT NULL DEFAULT 1,
                            [ShowHotDealButton] BIT NOT NULL DEFAULT 1,
                            [FooterLogoUrl] NVARCHAR(255) NULL,
                            [FooterBgColor] NVARCHAR(25) NULL,
                            [FooterTextColor] NVARCHAR(25) NULL,
                            [FooterAbout] NVARCHAR(600) NULL,
                            [Address] NVARCHAR(255) NULL,
                            [SecondaryPhone] NVARCHAR(50) NULL,
                            [Email] NVARCHAR(120) NULL,
                            [WorkingHours] NVARCHAR(150) NULL,
                            [FooterCol1Title] NVARCHAR(100) NULL,
                            [FooterCol1Links] NVARCHAR(MAX) NULL,
                            [FooterCol2Title] NVARCHAR(100) NULL,
                            [FooterCol2Links] NVARCHAR(MAX) NULL,
                            [FacebookUrl] NVARCHAR(255) NULL,
                            [InstagramUrl] NVARCHAR(255) NULL,
                            [ZaloUrl] NVARCHAR(255) NULL,
                            [TikTokUrl] NVARCHAR(255) NULL,
                            [CopyrightText] NVARCHAR(255) NULL,
                            [ShowFloatingChat] BIT NOT NULL DEFAULT 1,
                            [UpdatedAt] DATETIME NOT NULL DEFAULT GETDATE()
                        );
                    END
                ";
                context.Database.ExecuteSqlCommand(sqlCreateSettings);

                // Khởi tạo dòng cấu hình mặc định nếu bảng SiteSettings đang trống
                int countSettings = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM [dbo].[SiteSettings]").FirstOrDefault();
                if (countSettings == 0)
                {
                    context.SiteSettings.Add(new SiteSetting());
                    context.SaveChanges();
                }
            }
            catch
            {
                // Tránh gián đoạn ứng dụng
            }
        }
    }
}
