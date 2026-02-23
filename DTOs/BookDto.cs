using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set;}
        public bool? IsHardPages { get; set; }
        public int StockQuantity { get; set; }
        public string? AuthorName { get; set; } 
        public string? CategoryName { get; set; } 
        public string? PromotionTitle { get; set; }
    }
}
