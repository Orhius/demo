using Npgsql;
using shoesMic.data;
using System;
using System.Collections.Generic;
using System.Windows;

namespace shoesMic.Services
{
    public class DatabaseService
    {
        private readonly string connectionString;

        public DatabaseService(string host, int port, string database, string username, string password)
        {
            connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        private NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(connectionString);
            try
            {
                conn.Open();
                //MessageBox.Show("Подключение успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
            
            //conn.Open();
            return conn;
        }

        #region Users & Authentication
        public (int userId, string fullName, string role)? AuthenticateUser(string login, string password)
        {
            using var conn = GetConnection(); // важно: conn должен быть открыт
            using var cmd = new NpgsqlCommand(@"
        SELECT u.user_id, u.full_name, r.role_name
        FROM users u
        JOIN roles r ON u.role_id = r.role_id
        WHERE u.login = @login AND u.password = @password
        LIMIT 1;
    ", conn);

            cmd.Parameters.Add(new NpgsqlParameter("login", NpgsqlTypes.NpgsqlDbType.Text) { Value = login });
            cmd.Parameters.Add(new NpgsqlParameter("password", NpgsqlTypes.NpgsqlDbType.Text) { Value = password });

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return (
                reader.GetInt32(0),
                reader.IsDBNull(1) ? "" : reader.GetString(1),
                reader.IsDBNull(2) ? "" : reader.GetString(2)
            );
        }
        #endregion

        #region Products
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT article, name, price, supplier, manufacturer, category, discount, stock, description, photo, unit
                    FROM products
                    ORDER BY name
                ", conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(ReadProduct(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения товаров: {ex.Message}");
            }
            return products;
        }

        public List<Product> GetProductsByCategory(string category)
        {
            var products = new List<Product>();
            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT article, name, price, supplier, manufacturer, category, discount, stock, description, photo, unit
                    FROM products
                    WHERE category = @category
                    ORDER BY name
                ", conn);
                cmd.Parameters.AddWithValue("category", category);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(ReadProduct(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка фильтрации по категории: {ex.Message}");
            }
            return products;
        }

        public List<string> GetCategories()
        {
            var categories = new List<string>();
            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand("SELECT DISTINCT category FROM products ORDER BY category", conn);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    {
                        categories.Add(reader.GetString(0));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения категорий: {ex.Message}");
            }
            return categories;
        }

        public List<Product> SearchProducts(string searchText)
        {
            var products = new List<Product>();
            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    SELECT article, name, price, supplier, manufacturer, category, discount, stock, description, photo, unit
                    FROM products
                    WHERE name ILIKE @text OR article ILIKE @text OR category ILIKE @text
                       OR manufacturer ILIKE @text OR description ILIKE @text
                    ORDER BY name
                ", conn);
                cmd.Parameters.AddWithValue("text", $"%{searchText}%");

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(ReadProduct(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска товаров: {ex.Message}");
            }
            return products;
        }

        public bool AddProduct(Product p)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    INSERT INTO products
                        (article, name, price, supplier, manufacturer, category, discount, stock, description, photo, unit)
                    VALUES
                        (@article, @name, @price, @supplier, @manufacturer, @category, @discount, @stock, @description, @photo, @unit)
                ", conn);

                AddProductParameters(cmd, p);
                cmd.ExecuteNonQuery();
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
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE products
                    SET name=@name, price=@price, supplier=@supplier, manufacturer=@manufacturer,
                        category=@category, discount=@discount, stock=@stock, description=@description,
                        photo=@photo, unit=@unit
                    WHERE article=@article
                ", conn);

                AddProductParameters(cmd, p);
                cmd.Parameters.AddWithValue("article", article);
                cmd.ExecuteNonQuery();
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
                using var conn = GetConnection();
                using var cmd = new NpgsqlCommand("DELETE FROM products WHERE article=@article", conn);
                cmd.Parameters.AddWithValue("article", article);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления товара: {ex.Message}");
                return false;
            }
        }

        private void AddProductParameters(NpgsqlCommand cmd, Product p)
        {
            cmd.Parameters.AddWithValue("article", p.Article);
            cmd.Parameters.AddWithValue("name", p.Name);
            cmd.Parameters.AddWithValue("price", p.Price);
            cmd.Parameters.AddWithValue("supplier", p.Supplier);
            cmd.Parameters.AddWithValue("manufacturer",
p.Manufacturer);
            cmd.Parameters.AddWithValue("category", p.Category);
            cmd.Parameters.AddWithValue("discount", p.Discount);
            cmd.Parameters.AddWithValue("stock", p.Stock);
            cmd.Parameters.AddWithValue("description", (object)p.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("photo", (object)p.Photo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("unit", p.Unit);
        }

        private Product ReadProduct(NpgsqlDataReader reader)
        {
            return new Product
            {
                Article = reader.GetString(0),
                Name = reader.GetString(1),
                Price = reader.GetDecimal(2),
                Supplier = reader.GetString(3),
                Manufacturer = reader.GetString(4),
                Category = reader.GetString(5),
                Discount = reader.GetInt32(6),
                Stock = reader.GetInt32(7),
                Description = reader.IsDBNull(8) ? null : reader.GetString(8),
                Photo = reader.IsDBNull(9) ? null : reader.GetString(9),
                Unit = reader.GetString(10)
            };
        }

        #endregion
    }
}