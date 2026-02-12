using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public int RequiredQuantity { get; set; } 
        public decimal DiscountedPrice { get; set; } 
        public DateTime? EndDate { get; set; } 

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
