using Microsoft.AspNetCore.Http;

namespace DotanBooks.Models
{
    public class ManagementBookUpsertRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsHardPages { get; set; }
        public int AuthorId { get; set; }
        public int CategoryId { get; set; }
        public int? PromotionId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}