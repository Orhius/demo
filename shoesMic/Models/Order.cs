using System;
using System.Collections.Generic;

namespace shoesMic.Models
{
    /// <summary>
    /// Заказ клиента.
    /// Соответствует таблице <c>orders</c> в базе данных.
    /// </summary>
    public class Order
    {
        /// <summary>Уникальный идентификатор заказа (первичный ключ).</summary>
        public int OrderId { get; set; }

        /// <summary>Дата оформления заказа.</summary>
        public DateTime OrderDate { get; set; }

        /// <summary>Ожидаемая дата доставки.</summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>Идентификатор клиента (внешний ключ → <see cref="User"/>).</summary>
        public int ClientId { get; set; }

        /// <summary>Идентификатор пункта выдачи (внешний ключ → <see cref="PickupPoint"/>).</summary>
        public int PointId { get; set; }

        /// <summary>Уникальный код заказа для отслеживания.</summary>
        public string Code { get; set; }

        /// <summary>Текущий статус заказа (например: "Новый", "Выдан").</summary>
        public string Status { get; set; }

        /// <summary>Навигационное свойство — клиент, оформивший заказ.</summary>
        public User Client { get; set; }

        /// <summary>Навигационное свойство — пункт выдачи заказа.</summary>
        public PickupPoint PickupPoint { get; set; }

        /// <summary>Список позиций заказа.</summary>
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}

