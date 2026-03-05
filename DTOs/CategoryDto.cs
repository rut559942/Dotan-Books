using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int BookCount { get; set; } 
    }
}
