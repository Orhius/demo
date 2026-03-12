namespace shoesMic.Models
{
    /// <summary>
    /// Модель товара (обувь / одежда).
    /// Соответствует таблице <c>products</c> в базе данных.
    /// </summary>
    public class Product
    {
        /// <summary>Уникальный артикул товара (первичный ключ).</summary>
        public string Article { get; set; }

        /// <summary>Наименование товара.</summary>
        public string Name { get; set; }

        /// <summary>Цена товара в рублях.</summary>
        public decimal Price { get; set; }

        /// <summary>Наименование поставщика.</summary>
        public string Supplier { get; set; }

        /// <summary>Производитель товара.</summary>
        public string Manufacturer { get; set; }

        /// <summary>Категория товара (например: "Кроссовки", "Ботинки").</summary>
        public string Category { get; set; }

        /// <summary>Размер скидки в процентах (0–100).</summary>
        public int Discount { get; set; }

        /// <summary>Количество единиц товара на складе.</summary>
        public int Stock { get; set; }

        /// <summary>Описание товара (необязательное поле).</summary>
        public string? Description { get; set; }

        /// <summary>Имя файла фотографии товара (например: "1.jpg"). Необязательное поле.</summary>
        public string? Photo { get; set; }

        /// <summary>Единица измерения (например: "пара", "шт.").</summary>
        public string Unit { get; set; }
    }
}

