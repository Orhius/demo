namespace shoesMic.Models
{
    /// <summary>
    /// Пункт выдачи заказа.
    /// Соответствует таблице <c>pickup_points</c> в базе данных.
    /// </summary>
    public class PickupPoint
    {
        /// <summary>Уникальный идентификатор пункта выдачи (первичный ключ).</summary>
        public int PointId { get; set; }

        /// <summary>Адрес пункта выдачи.</summary>
        public string Address { get; set; }
    }
}

