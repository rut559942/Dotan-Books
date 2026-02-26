using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class CreatePromotionDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal DiscountedPrice { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
