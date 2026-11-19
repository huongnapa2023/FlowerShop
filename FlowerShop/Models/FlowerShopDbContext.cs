using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FlowerShop.Models
{
    public class FlowerShopDbContext : DbContext
    {
        static FlowerShopDbContext()
        {
            Database.SetInitializer<FlowerShopDbContext>(null);
        }

        public FlowerShopDbContext() : base(LocalDbConnection.GetConnectionString())
        {
        }

        public FlowerShopDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
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
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }

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
