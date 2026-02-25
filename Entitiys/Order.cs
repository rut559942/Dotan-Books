using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string? OrderNumber { get; private set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Received;
        public string? ShippingAddress { get; set; }
        public int ShippingCity { get; set; }
        public string? CustomerNotes { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}
