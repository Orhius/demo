using Microsoft.EntityFrameworkCore;
using shoesMic.Data;
using shoesMic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace shoesMic.Services
{
    /// <summary>
    /// Сервис для работы с базой данных через Entity Framework Core + PostgreSQL.
    /// Содержит методы аутентификации пользователей и CRUD-операции над товарами.
    /// </summary>
    public class DatabaseService
    {
        // ──────────────────────────────────────────────
        //  Поля
        // ──────────────────────────────────────────────

        /// <summary>Параметры подключения к базе данных.</summary>
        private readonly DbContextOptions<AppDbContext> _options;

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        /// <summary>
        /// Инициализирует сервис с параметрами подключения к PostgreSQL.
        /// </summary>
        /// <param name="host">Адрес сервера БД.</param>
        /// <param name="port">Порт сервера БД.</param>
        /// <param name="database">Имя базы данных.</param>
        /// <param name="username">Имя пользователя БД.</param>
        /// <param name="password">Пароль пользователя БД.</param>
        public DatabaseService(string host, int port, string database, string username, string password)
        {
            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;
        }

        // ──────────────────────────────────────────────
        //  Приватный метод — создание контекста
        // ──────────────────────────────────────────────

        /// <summary>Создаёт новый экземпляр <see cref="AppDbContext"/>.</summary>
        private AppDbContext CreateContext() => new AppDbContext(_options);

        // ══════════════════════════════════════════════
        //  АУТЕНТИФИКАЦИЯ
        // ══════════════════════════════════════════════

        #region Аутентификация

        /// <summary>
        /// Проверяет логин и пароль пользователя.
        /// </summary>
        /// <param name="login">Логин пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <returns>
        /// Кортеж (userId, fullName, role) при успехе, либо <c>null</c> если пользователь не найден.
        /// </returns>
        public (int userId, string fullName, string role)? AuthenticateUser(string login, string password)
        {
            try
            {
                using var ctx = CreateContext();

                // Ищем пользователя с совпадающим логином и паролем, подгружаем роль
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

        // ══════════════════════════════════════════════
        //  ТОВАРЫ
        // ══════════════════════════════════════════════

        #region Товары

        /// <summary>
        /// Возвращает все товары из базы данных, отсортированные по названию.
        /// </summary>
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

        /// <summary>
        /// Возвращает товары указанной категории, отсортированные по названию.
        /// </summary>
        /// <param name="category">Название категории.</param>
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

        /// <summary>
        /// Возвращает список уникальных категорий товаров в алфавитном порядке.
        /// </summary>
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

        /// <summary>
        /// Выполняет поиск товаров по тексту (регистронезависимо, ILIKE).
        /// Поиск ведётся по названию, артикулу, категории, производителю и описанию.
        /// </summary>
        /// <param name="searchText">Строка поиска.</param>
        public List<Product> SearchProducts(string searchText)
        {
            try
            {
                using var ctx = CreateContext();
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

        /// <summary>
        /// Добавляет новый товар в базу данных.
        /// </summary>
        /// <param name="p">Объект товара для добавления.</param>
        /// <returns><c>true</c> при успехе, <c>false</c> при ошибке.</returns>
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

        /// <summary>
        /// Обновляет существующий товар по артикулу.
        /// </summary>
        /// <param name="article">Артикул товара для обновления.</param>
        /// <param name="p">Объект с новыми данными товара.</param>
        /// <returns><c>true</c> при успехе, <c>false</c> если товар не найден или произошла ошибка.</returns>
        public bool UpdateProduct(string article, Product p)
        {
            try
            {
                using var ctx = CreateContext();

                // Ищем существующий товар по артикулу
                var existing = ctx.Products.Find(article);
                if (existing == null) return false;

                // Обновляем все поля, кроме артикула (первичный ключ не меняем)
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

        /// <summary>
        /// Удаляет товар из базы данных по артикулу.
        /// </summary>
        /// <param name="article">Артикул товара для удаления.</param>
        /// <returns><c>true</c> при успехе, <c>false</c> если товар не найден или произошла ошибка.</returns>
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

