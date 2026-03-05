using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public record CreateAuthorDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
