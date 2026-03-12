using System;
using System.Collections.Generic;
using System.Text;

namespace shoesMic.data
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int ClientId { get; set; }
        public int PointId { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }

        public User Client { get; set; }
        public PickupPoint PickupPoint { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
