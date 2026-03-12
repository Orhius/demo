using shoesMic;
using shoesMic.data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using shoesMic.Services;

namespace shoesMic
{
    public partial class ProductControl : UserControl
    {
        private readonly Product product;
        private readonly string role;
        private readonly DatabaseService db;
        private readonly ProductsWindow parent;

        public ProductControl(Product product, string role,
                              DatabaseService db, ProductsWindow parent)
        {
            InitializeComponent();

            this.product = product;
            this.role = role;
            this.db = db;
            this.parent = parent;

            FillData();
            LoadImage();
        }

        private void FillData()
        {
            NameText.Text = product.Name;
            PriceText.Text = $"Цена: {product.Price} ₽";
            CategoryText.Text = $"Категория: {product.Category}";
            ManufacturerText.Text = $"Производитель: {product.Manufacturer}";
            DescriptionText.Text = product.Description;
        }

        /// <summary>
        /// Загружает изображение товара
        /// </summary>
        private void LoadImage()
        {
            string imagesFolder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Images"
            );

            string imagePath = null;

            if (!string.IsNullOrWhiteSpace(product.Photo))
            {
                imagePath = Path.Combine(imagesFolder, product.Photo);
            }

            if (imagePath != null && File.Exists(imagePath))
            {
                ProductImage.Source = CreateBitmap(imagePath);
            }
            else
            {
                // Заглушка
                ProductImage.Source = CreateBitmap(
                    Path.Combine(imagesFolder, "picture.jpg")
                );
            }
        }

        private BitmapImage CreateBitmap(string path)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
    }
}
