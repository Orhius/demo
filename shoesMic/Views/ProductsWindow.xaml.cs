using shoesMic.Models;
using shoesMic.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace shoesMic.Views
{
    /// <summary>
    /// Главное окно каталога товаров.
    /// Отображает список товаров с возможностью поиска, фильтрации по категории
    /// и добавления новых товаров (только для администратора).
    /// </summary>
    public partial class ProductsWindow : Window
    {
        // ──────────────────────────────────────────────
        //  Поля
        // ──────────────────────────────────────────────

        /// <summary>Сервис для работы с базой данных.</summary>
        private readonly DatabaseService _db;

        /// <summary>Идентификатор текущего пользователя (null — гость).</summary>
        private readonly int? _userId;

        /// <summary>Полное имя текущего пользователя.</summary>
        private readonly string _fullName;

        /// <summary>Роль текущего пользователя (например: "Администратор", "Гость").</summary>
        private readonly string _role;

        /// <summary>Полный список товаров, загруженных из БД.</summary>
        private List<Product> _allProducts = new();

        /// <summary>Список товаров, отображаемых на экране (после фильтрации/поиска).</summary>
        private List<Product> _viewProducts = new();

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        /// <summary>
        /// Инициализирует окно каталога.
        /// </summary>
        /// <param name="userId">ID пользователя (null для гостя).</param>
        /// <param name="fullName">Имя пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <param name="db">Экземпляр сервиса БД.</param>
        public ProductsWindow(int? userId, string fullName, string role, DatabaseService db)
        {
            InitializeComponent();

            _userId   = userId;
            _fullName = fullName;
            _role     = role;
            _db       = db;

            // Отображаем имя и роль пользователя в шапке
            UserLabel.Text = $"{fullName} ({role})";

            // Кнопку «Добавить товар» показываем только администратору
            if (role != "Администратор")
                AddProductBtn.Visibility = Visibility.Collapsed;

            LoadProductsFromDb();
            LoadCategories();
        }

        // ──────────────────────────────────────────────
        //  Загрузка данных
        // ──────────────────────────────────────────────

        /// <summary>
        /// Загружает все товары из базы данных и отображает их.
        /// </summary>
        private void LoadProductsFromDb()
        {
            _allProducts  = _db.GetAllProducts();
            _viewProducts = new List<Product>(_allProducts);
            RenderProducts(_viewProducts);
        }

        /// <summary>
        /// Загружает список категорий в выпадающий список.
        /// Первый элемент — «Все категории».
        /// </summary>
        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add("Все категории");

            foreach (var category in _db.GetCategories())
                CategoryComboBox.Items.Add(category);

            CategoryComboBox.SelectedIndex = 0;
        }

        // ──────────────────────────────────────────────
        //  Отображение товаров
        // ──────────────────────────────────────────────

        /// <summary>
        /// Очищает панель и заново рендерит карточки товаров.
        /// Если список пуст — выводит сообщение «Товары не найдены».
        /// </summary>
        private void RenderProducts(List<Product> products)
        {
            ProductsPanel.Children.Clear();

            if (products.Count == 0)
            {
                ProductsPanel.Children.Add(new TextBlock
                {
                    Text = "Товары не найдены",
                    FontSize = 16,
                    Margin = new Thickness(10)
                });
                return;
            }

            // Создаём карточку ProductControl для каждого товара
            foreach (var product in products)
            {
                ProductsPanel.Children.Add(
                    new ProductControl(product, _role, _db, this)
                );
            }
        }

        /// <summary>
        /// Публичный метод для обновления списка товаров извне
        /// (вызывается после добавления / редактирования / удаления товара).
        /// </summary>
        public void RefreshProducts()
        {
            LoadProductsFromDb();
        }

        // ──────────────────────────────────────────────
        //  Обработчики событий
        // ──────────────────────────────────────────────

        /// <summary>
        /// Открывает диалог добавления нового товара.
        /// После успешного сохранения обновляет список.
        /// </summary>
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditProductWindow(_db);

            if (dialog.ShowDialog() == true)
                RefreshProducts();
        }

        /// <summary>
        /// Выполняет поиск по тексту в полях товара на стороне клиента.
        /// Если строка пустая — показывает все товары.
        /// </summary>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                _viewProducts = new List<Product>(_allProducts);
                RenderProducts(_viewProducts);
                return;
            }

            // Фильтруем по нескольким полям товара
            _viewProducts = _allProducts.FindAll(p =>
                p.Name.ToLower().Contains(search) ||
                p.Article.ToLower().Contains(search) ||
                p.Category.ToLower().Contains(search) ||
                p.Manufacturer.ToLower().Contains(search) ||
                (p.Description?.ToLower().Contains(search) ?? false)
            );

            RenderProducts(_viewProducts);
        }

        /// <summary>
        /// Фильтрует список товаров при смене выбранной категории.
        /// Выбор «Все категории» сбрасывает фильтр.
        /// </summary>
        private void FilterByCategory(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem == null) return;

            string category = CategoryComboBox.SelectedItem.ToString()!;

            _viewProducts = category == "Все категории"
                ? new List<Product>(_allProducts)
                : _allProducts.FindAll(p => p.Category == category);

            RenderProducts(_viewProducts);
        }

        /// <summary>
        /// Выполняет выход из учётной записи: закрывает текущее окно и открывает окно авторизации.
        /// </summary>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}

