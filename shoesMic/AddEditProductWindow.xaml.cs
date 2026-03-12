using shoesMic.data;
using System;
using System.Windows;
using System.Windows.Controls;
using shoesMic.Services;

namespace shoesMic
{
    public partial class AddEditProductWindow : Window
    {
        private readonly DatabaseService db;
        private readonly Product product;
        private readonly bool isEditMode;

        public AddEditProductWindow(DatabaseService db, Product product = null)
        {
            InitializeComponent();
            this.db = db;
            this.product = product;
            isEditMode = product != null;

            TitleText.Text = isEditMode ? "Редактирование товара" : "Добавление товара";

            LoadCategories();

            if (isEditMode)
                LoadProductData();
        }

        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();
            foreach (var cat in db.GetCategories())
                CategoryComboBox.Items.Add(cat);
            if (CategoryComboBox.Items.Count > 0)
                CategoryComboBox.SelectedIndex = 0;
        }

        private void LoadProductData()
        {
            ArticleTextBox.Text = product.Article;
            ArticleTextBox.IsReadOnly = true;
            NameTextBox.Text = product.Name;
            PriceTextBox.Text = product.Price.ToString();
            SupplierTextBox.Text = product.Supplier;
            ManufacturerTextBox.Text = product.Manufacturer;
            CategoryComboBox.SelectedItem = product.Category;
            DiscountTextBox.Text = product.Discount.ToString();
            StockTextBox.Text = product.Stock.ToString();
            UnitTextBox.Text = product.Unit;
            PhotoTextBox.Text = product.Photo;
            DescriptionTextBox.Text = product.Description;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(SupplierTextBox.Text) ||
                string.IsNullOrWhiteSpace(ManufacturerTextBox.Text) ||
                string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля (*)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Некорректная цена", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int discount = 0;
            int.TryParse(DiscountTextBox.Text, out discount);

            if (!int.TryParse(StockTextBox.Text, out int stock))
            {
                MessageBox.Show("Некорректное количество на складе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newProduct = new Product
            {
                Article = ArticleTextBox.Text.Trim(),
                Name = NameTextBox.Text.Trim(),
                Price = price,
                Supplier = SupplierTextBox.Text.Trim(),
                Manufacturer = ManufacturerTextBox.Text.Trim(),
                Category = CategoryComboBox.SelectedItem.ToString(),
                Discount = discount,
                Stock = stock,
                Unit = UnitTextBox.Text.Trim(),
                Photo = string.IsNullOrWhiteSpace(PhotoTextBox.Text) ? null : PhotoTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ? null : DescriptionTextBox.Text.Trim()
            };

            bool result = isEditMode ? db.UpdateProduct(newProduct.Article, newProduct) : db.AddProduct(newProduct);

            if (result)
            {
                MessageBox.Show("Операция выполнена успешно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Ошибкапри сохранении товара", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}