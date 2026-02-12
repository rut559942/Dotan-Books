using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsBestSeller { get; set; }

        
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int? PromotionId { get; set; } 

        public virtual Author Author { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual Promotion? Promotion { get; set; }
    }
}
