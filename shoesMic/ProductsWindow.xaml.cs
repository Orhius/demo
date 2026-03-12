using shoesMic.data;
using shoesMic.Services;
using System.Windows;
using System.Windows.Controls;

namespace shoesMic
{
    public partial class ProductsWindow : Window
    {
        private readonly DatabaseService db;
        private readonly int? userId;
        private readonly string fullName;
        private readonly string role;

        private List<Product> allProducts = new();
        private List<Product> viewProducts = new();

        public ProductsWindow(int? userId, string fullName, string role, DatabaseService db)
        {
            InitializeComponent();

            this.userId = userId;
            this.fullName = fullName;
            this.role = role;
            this.db = db;

            UserLabel.Text = $"{fullName} ({role})";

            if (role != "Администратор")
                AddProductBtn.Visibility = Visibility.Collapsed;

            LoadProductsFromDb();
            LoadCategories();
        }
        private void LoadProductsFromDb()
        {
            allProducts = db.GetAllProducts();
            viewProducts = new List<Product>(allProducts);
            RenderProducts(viewProducts);
        }
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

            foreach (var product in products)
            {
                ProductsPanel.Children.Add(
                    new ProductControl(product, role, db, this)
                );
            }
        }
        private void LoadCategories()
        {
            CategoryComboBox.Items.Clear();
            CategoryComboBox.Items.Add("Все категории");

            foreach (var category in db.GetCategories())
                CategoryComboBox.Items.Add(category);

            CategoryComboBox.SelectedIndex = 0;
        }
        public void RefreshProducts()
        {
            LoadProductsFromDb();
        }
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditProductWindow(db);

            if (dialog.ShowDialog() == true)
            {
                RefreshProducts();
            }
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string search = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(search))
            {
                viewProducts = new List<Product>(allProducts);
                RenderProducts(viewProducts);
                return;
            }

            viewProducts = allProducts.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Article.ToLower().Contains(search) ||
                p.Category.ToLower().Contains(search) ||
                p.Manufacturer.ToLower().Contains(search) ||
                (p.Description?.ToLower().Contains(search) ?? false)
            ).ToList();

            RenderProducts(viewProducts);
        }
        private void FilterByCategory(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedItem == null)
                return;

            string category = CategoryComboBox.SelectedItem.ToString();

            if (category == "Все категории")
            {
                viewProducts = new List<Product>(allProducts);
            }
            else
            {
                viewProducts = allProducts
                    .Where(p => p.Category == category)
                    .ToList();
            }

            RenderProducts(viewProducts);
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}