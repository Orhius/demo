using shoesMic.Models;
using shoesMic.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace shoesMic.Views
{
    /// <summary>
    /// Карточка товара — пользовательский элемент управления (UserControl).
    /// Отображает фото, название, цену, категорию, производителя и описание одного товара.
    /// </summary>
    public partial class ProductControl : UserControl
    {
        // ──────────────────────────────────────────────
        //  Поля
        // ──────────────────────────────────────────────

        /// <summary>Модель товара, данные которого отображаются в карточке.</summary>
        private readonly Product _product;

        /// <summary>Роль текущего пользователя (влияет на видимость кнопок действий).</summary>
        private readonly string _role;

        /// <summary>Сервис для работы с базой данных.</summary>
        private readonly DatabaseService _db;

        /// <summary>Ссылка на родительское окно каталога (для обновления списка).</summary>
        private readonly ProductsWindow _parent;

        // ──────────────────────────────────────────────
        //  Конструктор
        // ──────────────────────────────────────────────

        /// <summary>
        /// Инициализирует карточку товара.
        /// </summary>
        /// <param name="product">Данные товара.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <param name="db">Сервис БД.</param>
        /// <param name="parent">Родительское окно.</param>
        public ProductControl(Product product, string role, DatabaseService db, ProductsWindow parent)
        {
            InitializeComponent();

            _product = product;
            _role    = role;
            _db      = db;
            _parent  = parent;

            FillData();
            LoadImage();
        }

        // ──────────────────────────────────────────────
        //  Заполнение данных
        // ──────────────────────────────────────────────

        /// <summary>
        /// Заполняет текстовые поля карточки данными из модели товара.
        /// </summary>
        private void FillData()
        {
            NameText.Text         = _product.Name;
            PriceText.Text        = $"Цена: {_product.Price} ₽";
            CategoryText.Text     = $"Категория: {_product.Category}";
            ManufacturerText.Text = $"Производитель: {_product.Manufacturer}";
            DescriptionText.Text  = _product.Description;
        }

        // ──────────────────────────────────────────────
        //  Загрузка изображения
        // ──────────────────────────────────────────────

        /// <summary>
        /// Загружает фото товара из папки Images в директории приложения.
        /// Если файл не найден или путь не указан — отображает изображение-заглушку (<c>picture.jpg</c>).
        /// </summary>
        private void LoadImage()
        {
            // Папка с изображениями рядом с исполняемым файлом
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            string? imagePath = null;

            // Если у товара задано имя фото — строим полный путь
            if (!string.IsNullOrWhiteSpace(_product.Photo))
                imagePath = Path.Combine(imagesFolder, _product.Photo);

            // Если файл существует — показываем его, иначе — заглушку
            ProductImage.Source = (imagePath != null && File.Exists(imagePath))
                ? CreateBitmap(imagePath)
                : CreateBitmap(Path.Combine(imagesFolder, "picture.jpg"));
        }

        /// <summary>
        /// Создаёт <see cref="BitmapImage"/> из пути к файлу.
        /// Использует <see cref="BitmapCacheOption.OnLoad"/>, чтобы освободить файл после загрузки.
        /// </summary>
        /// <param name="path">Абсолютный путь к файлу изображения.</param>
        private static BitmapImage CreateBitmap(string path)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource   = new Uri(path, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze(); // Делаем объект неизменяемым для использования в разных потоках
            return bitmap;
        }
    }
}

