using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public record OrderCreateDto
    {
        [Required(ErrorMessage = "חובה להזין כתובת למשלוח")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "עיר לא תקינה")]
        public int ShippingCityId { get; set; }

        [Required(ErrorMessage = "חובה להוסיף מוצרים להזמנה")]
        [MinLength(1, ErrorMessage = "הסל ריק")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        [Range(0.01, double.MaxValue, ErrorMessage = "סכום ההזמנה חייב להיות גדול מ-0")]
        public decimal ClientOrderTotal { get; set; }

        [MaxLength(500, ErrorMessage = "הערות הלקוח ארוכות מדי")]
        public string? CustomerNotes { get; set; }
    }

    public record OrderItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "מזהה ספר לא תקין")]
        public int BookId { get; set; }

        [Range(1, 100, ErrorMessage = "כמות חייבת להיות בין 1 ל-100")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "מחיר פריט חייב להיות גדול מ-0")]
        public decimal ClientUnitPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "סכום שורת פריט חייב להיות גדול מ-0")]
        public decimal ClientLineTotal { get; set; }
    }
}
