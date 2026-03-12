namespace shoesMic.Models
{
    /// <summary>
    /// Позиция заказа — конкретный товар с количеством.
    /// Соответствует таблице <c>order_items</c> в базе данных.
    /// </summary>
    public class OrderItem
    {
        /// <summary>Уникальный идентификатор позиции заказа (первичный ключ).</summary>
        public int OrderItemId { get; set; }

        /// <summary>Идентификатор заказа (внешний ключ → <see cref="Order"/>).</summary>
        public int OrderId { get; set; }

        /// <summary>Артикул товара (внешний ключ → <see cref="Product"/>).</summary>
        public string Article { get; set; }

        /// <summary>Количество единиц товара в данной позиции.</summary>
        public int Quantity { get; set; }

        /// <summary>Навигационное свойство — заказ, к которому относится позиция.</summary>
        public Order Order { get; set; }

        /// <summary>Навигационное свойство — товар в позиции заказа.</summary>
        public Product Product { get; set; }
    }
}

