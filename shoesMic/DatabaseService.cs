using Microsoft.EntityFrameworkCore;
using shoesMic.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace shoesMic.Services
{
    public class DatabaseService
    {
        private readonly DbContextOptions<AppDbContext> _options;

        public DatabaseService(string host, int port, string database, string username, string password)
        {
            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        private AppDbContext CreateContext() => new AppDbContext(_options);

        #region Users & Authentication
        public (int userId, string fullName, string role)? AuthenticateUser(string login, string password)
        {
            try
            {
                using var ctx = CreateContext();
                var user = ctx.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user == null) return null;

                return (user.UserId, user.FullName ?? "", user.Role?.RoleName ?? "");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Products
        public List<Product> GetAllProducts()
        {
            try
            {
                using var ctx = CreateContext();
                return ctx.Products.OrderBy(p => p.Name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        public List<Product> GetProductsByCategory(string category)
        {
            try
            {
                using var ctx = CreateContext();
                return ctx.Products
                    .Where(p => p.Category == category)
                    .OrderBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка фильтрации по категории: {ex.Message}");
                return new List<Product>();
            }
        }

        public List<string> GetCategories()
        {
            try
            {
                using var ctx = CreateContext();
                return ctx.Products
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения категорий: {ex.Message}");
                return new List<string>();
            }
        }

        public List<Product> SearchProducts(string searchText)
        {
            try
            {
                using var ctx = CreateContext();
                var text = searchText.ToLower();
                return ctx.Products
                    .Where(p =>
                        EF.Functions.ILike(p.Name, $"%{searchText}%") ||
                        EF.Functions.ILike(p.Article, $"%{searchText}%") ||
                        EF.Functions.ILike(p.Category, $"%{searchText}%") ||
                        EF.Functions.ILike(p.Manufacturer, $"%{searchText}%") ||
                        (p.Description != null && EF.Functions.ILike(p.Description, $"%{searchText}%")))
                    .OrderBy(p => p.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска товаров: {ex.Message}");
                return new List<Product>();
            }
        }

        public bool AddProduct(Product p)
        {
            try
            {
                using var ctx = CreateContext();
                ctx.Products.Add(p);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления товара: {ex.Message}");
                return false;
            }
        }

        public bool UpdateProduct(string article, Product p)
        {
            try
            {
                using var ctx = CreateContext();
                var existing = ctx.Products.Find(article);
                if (existing == null) return false;

                existing.Name = p.Name;
                existing.Price = p.Price;
                existing.Supplier = p.Supplier;
                existing.Manufacturer = p.Manufacturer;
                existing.Category = p.Category;
                existing.Discount = p.Discount;
                existing.Stock = p.Stock;
                existing.Description = p.Description;
                existing.Photo = p.Photo;
                existing.Unit = p.Unit;

                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления товара: {ex.Message}");
                return false;
            }
        }

        public bool DeleteProduct(string article)
        {
            try
            {
                using var ctx = CreateContext();
                var product = ctx.Products.Find(article);
                if (product == null) return false;

                ctx.Products.Remove(product);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления товара: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}