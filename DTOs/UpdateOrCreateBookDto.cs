using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
     public class UpdateOrCreateBookDto
    {

        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public bool IsHardPages { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int? PromotionId { get; set; }
    }


    public class PromotionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
           
    }

    public class ManagementBookDto
    {
        public IEnumerable<PromotionDto> Promotions { get; set; } = new List<PromotionDto>();
        public IEnumerable<AuthorDto> Author { get; set; } = new List<AuthorDto>();
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    }

}
