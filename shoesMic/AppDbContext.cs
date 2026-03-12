using Microsoft.EntityFrameworkCore;
using shoesMic.data;

namespace shoesMic
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PickupPoint> PickupPoints { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Role
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("roles");
                e.HasKey(r => r.RoleId);
                e.Property(r => r.RoleId).HasColumnName("role_id");
                e.Property(r => r.RoleName).HasColumnName("role_name");
            });

            // User
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(u => u.UserId);
                e.Property(u => u.UserId).HasColumnName("user_id");
                e.Property(u => u.RoleId).HasColumnName("role_id");
                e.Property(u => u.FullName).HasColumnName("full_name");
                e.Property(u => u.Login).HasColumnName("login");
                e.Property(u => u.Password).HasColumnName("password");
                e.HasOne(u => u.Role)
                 .WithMany()
                 .HasForeignKey(u => u.RoleId);
            });

            // Product
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(p => p.Article);
                e.Property(p => p.Article).HasColumnName("article");
                e.Property(p => p.Name).HasColumnName("name");
                e.Property(p => p.Price).HasColumnName("price");
                e.Property(p => p.Supplier).HasColumnName("supplier");
                e.Property(p => p.Manufacturer).HasColumnName("manufacturer");
                e.Property(p => p.Category).HasColumnName("category");
                e.Property(p => p.Discount).HasColumnName("discount");
                e.Property(p => p.Stock).HasColumnName("stock");
                e.Property(p => p.Description).HasColumnName("description");
                e.Property(p => p.Photo).HasColumnName("photo");
                e.Property(p => p.Unit).HasColumnName("unit");
            });

            // PickupPoint
            modelBuilder.Entity<PickupPoint>(e =>
            {
                e.ToTable("pickup_points");
                e.HasKey(pp => pp.PointId);
                e.Property(pp => pp.PointId).HasColumnName("point_id");
                e.Property(pp => pp.Address).HasColumnName("address");
            });

            // Order
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(o => o.OrderId);
                e.Property(o => o.OrderId).HasColumnName("order_id");
                e.Property(o => o.OrderDate).HasColumnName("order_date");
                e.Property(o => o.DeliveryDate).HasColumnName("delivery_date");
                e.Property(o => o.ClientId).HasColumnName("client_id");
                e.Property(o => o.PointId).HasColumnName("point_id");
                e.Property(o => o.Code).HasColumnName("code");
                e.Property(o => o.Status).HasColumnName("status");
                e.HasOne(o => o.Client)
                 .WithMany()
                 .HasForeignKey(o => o.ClientId);
                e.HasOne(o => o.PickupPoint)
                 .WithMany()
                 .HasForeignKey(o => o.PointId);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.ToTable("order_items");
                e.HasKey(oi => oi.OrderItemId);
                e.Property(oi => oi.OrderItemId).HasColumnName("order_item_id");
                e.Property(oi => oi.OrderId).HasColumnName("order_id");
                e.Property(oi => oi.Article).HasColumnName("article");
                e.Property(oi => oi.Quantity).HasColumnName("quantity");
                e.HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId);
                e.HasOne(oi => oi.Product)
                 .WithMany()
                 .HasForeignKey(oi => oi.Article);
            });
        }
    }
}

