using Microsoft.EntityFrameworkCore;
using shoesMic.Models;

namespace shoesMic.Data
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// Управляет подключением к PostgreSQL через Entity Framework Core.
    /// Содержит DbSet-наборы для всех сущностей и настройку маппинга (Fluent API).
    /// </summary>
    public class AppDbContext : DbContext
    {
        // ──────────────────────────────────────────────
        //  DbSet-наборы — представляют таблицы БД
        // ──────────────────────────────────────────────

        /// <summary>Таблица пользователей (<c>users</c>).</summary>
        public DbSet<User> Users { get; set; }

        /// <summary>Таблица ролей (<c>roles</c>).</summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>Таблица товаров (<c>products</c>).</summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>Таблица заказов (<c>orders</c>).</summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>Таблица позиций заказов (<c>order_items</c>).</summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>Таблица пунктов выдачи (<c>pickup_points</c>).</summary>
        public DbSet<PickupPoint> PickupPoints { get; set; }

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        /// <summary>
        /// Создаёт контекст с переданными параметрами подключения.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ──────────────────────────────────────────────
        //  Fluent API — конфигурация маппинга
        // ──────────────────────────────────────────────

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Роль ──────────────────────────────────
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("roles");
                e.HasKey(r => r.RoleId);
                e.Property(r => r.RoleId).HasColumnName("role_id");
                e.Property(r => r.RoleName).HasColumnName("role_name");
            });

            // ── Пользователь ──────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(u => u.UserId);
                e.Property(u => u.UserId).HasColumnName("user_id");
                e.Property(u => u.RoleId).HasColumnName("role_id");
                e.Property(u => u.FullName).HasColumnName("full_name");
                e.Property(u => u.Login).HasColumnName("login");
                e.Property(u => u.Password).HasColumnName("password");

                // Связь: один пользователь → одна роль
                e.HasOne(u => u.Role)
                 .WithMany()
                 .HasForeignKey(u => u.RoleId);
            });

            // ── Товар ─────────────────────────────────
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(p => p.Article);                          // Первичный ключ — артикул
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

            // ── Пункт выдачи ──────────────────────────
            modelBuilder.Entity<PickupPoint>(e =>
            {
                e.ToTable("pickup_points");
                e.HasKey(pp => pp.PointId);
                e.Property(pp => pp.PointId).HasColumnName("point_id");
                e.Property(pp => pp.Address).HasColumnName("address");
            });

            // ── Заказ ─────────────────────────────────
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

                // Связь: заказ → клиент
                e.HasOne(o => o.Client)
                 .WithMany()
                 .HasForeignKey(o => o.ClientId);

                // Связь: заказ → пункт выдачи
                e.HasOne(o => o.PickupPoint)
                 .WithMany()
                 .HasForeignKey(o => o.PointId);
            });

            // ── Позиция заказа ────────────────────────
            modelBuilder.Entity<OrderItem>(e =>
            {
                e.ToTable("order_items");
                e.HasKey(oi => oi.OrderItemId);
                e.Property(oi => oi.OrderItemId).HasColumnName("order_item_id");
                e.Property(oi => oi.OrderId).HasColumnName("order_id");
                e.Property(oi => oi.Article).HasColumnName("article");
                e.Property(oi => oi.Quantity).HasColumnName("quantity");

                // Связь: позиция → заказ (один-ко-многим)
                e.HasOne(oi => oi.Order)
                 .WithMany(o => o.OrderItems)
                 .HasForeignKey(oi => oi.OrderId);

                // Связь: позиция → товар
                e.HasOne(oi => oi.Product)
                 .WithMany()
                 .HasForeignKey(oi => oi.Article);
            });
        }
    }
}

