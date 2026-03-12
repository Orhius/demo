using shoesMic.Models;
using shoesMic.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace shoesMic.Views
{
    /// <summary>
    /// Диалоговое окно добавления и редактирования товара.
    /// Работает в двух режимах:
    /// <list type="bullet">
    ///   <item><description>Добавление — <paramref name="product"/> равен null.</description></item>
    ///   <item><description>Редактирование — <paramref name="product"/> содержит существующий товар.</description></item>
    /// </list>
    /// </summary>
    public partial class AddEditProductWindow : Window
    {
        // ──────────────────────────────────────────────
        //  Поля
        // ──────────────────────────────────────────────

        /// <summary>Сервис для работы с базой данных.</summary>
        private readonly DatabaseService _db;

        /// <summary>Товар, переданный для редактирования (null — режим добавления).</summary>
        private readonly Product? _product;

        /// <summary>Флаг: true — режим редактирования, false — режим добавления.</summary>
        private readonly bool _isEditMode;

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        /// <summary>
        /// Создаёт окно добавления/редактирования товара.
        /// </summary>
        /// <param name="db">Сервис БД.</param>
        /// <param name="product">Товар для редактирования; null — добавление нового.</param>
        public AddEditProductWindow(DatabaseService db, Product? product = null)
        {
            InitializeComponent();

            _db         = db;
            _product    = product;
            _isEditMode = product != null;

            // Заголовок окна зависит от режима
            TitleText.Text = _isEditMode ? "Редактирование товара" : "Добавление товара";

            LoadCategories();

            // Если редактирование — заполняем поля данными товара
            if (_isEditMode)
                LoadProductData();
        }

        // ──────────────────────────────────────────────
        //  Загрузка данных
        // ──────────────────────────────────────────────

        /// <summary>
        /// Заполняет ComboBox категорий актуальными значениями из БД.
        /// </summary>
        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();

            foreach (var cat in _db.GetCategories())
                CategoryComboBox.Items.Add(cat);

            if (CategoryComboBox.Items.Count > 0)
                CategoryComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Заполняет поля формы данными существующего товара (режим редактирования).
        /// Поле артикула блокируется — первичный ключ нельзя менять.
        /// </summary>
        private void LoadProductData()
        {
            ArticleTextBox.Text      = _product!.Article;
            ArticleTextBox.IsReadOnly = true;              // Артикул — первичный ключ, не редактируется
            NameTextBox.Text         = _product.Name;
            PriceTextBox.Text        = _product.Price.ToString();
            SupplierTextBox.Text     = _product.Supplier;
            ManufacturerTextBox.Text = _product.Manufacturer;
            CategoryComboBox.SelectedItem = _product.Category;
            DiscountTextBox.Text     = _product.Discount.ToString();
            StockTextBox.Text        = _product.Stock.ToString();
            UnitTextBox.Text         = _product.Unit;
            PhotoTextBox.Text        = _product.Photo;
            DescriptionTextBox.Text  = _product.Description;
        }

        // ──────────────────────────────────────────────
        //  Обработчики событий
        // ──────────────────────────────────────────────

        /// <summary>
        /// Обрабатывает нажатие кнопки «OK».
        /// Валидирует поля, собирает объект Product и вызывает нужный метод сервиса (Add / Update).
        /// </summary>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // ── Валидация обязательных полей ──────────
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text)      ||
                string.IsNullOrWhiteSpace(NameTextBox.Text)          ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text)         ||
                string.IsNullOrWhiteSpace(SupplierTextBox.Text)      ||
                string.IsNullOrWhiteSpace(ManufacturerTextBox.Text)  ||
                string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (*)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Проверка числовых полей ───────────────
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Некорректная цена", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int.TryParse(DiscountTextBox.Text, out int discount);   // Скидка — необязательная

            if (!int.TryParse(StockTextBox.Text, out int stock))
            {
                MessageBox.Show("Некорректное количество на складе", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ── Сборка объекта Product ────────────────
            var newProduct = new Product
            {
                Article      = ArticleTextBox.Text.Trim(),
                Name         = NameTextBox.Text.Trim(),
                Price        = price,
                Supplier     = SupplierTextBox.Text.Trim(),
                Manufacturer = ManufacturerTextBox.Text.Trim(),
                Category     = CategoryComboBox.SelectedItem.ToString()!,
                Discount     = discount,
                Stock        = stock,
                Unit         = UnitTextBox.Text.Trim(),
                // Необязательные поля: null если пусто
                Photo       = string.IsNullOrWhiteSpace(PhotoTextBox.Text)       ? null : PhotoTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim()
            };

            // ── Сохранение в БД ───────────────────────
            bool success = _isEditMode
                ? _db.UpdateProduct(newProduct.Article, newProduct)
                : _db.AddProduct(newProduct);

            if (success)
            {
                MessageBox.Show("Операция выполнена успешно", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Ошибка при сохранении товара", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки «Отмена» — закрывает окно без сохранения.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

