using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public record CategoryBooksResult<T>
    {
        public Category? Category { get; set; }//לשנות ל CategoryDto אסור לחסוף יישות!  שיש בו רק את השדות הרלוונטים ולא את כל השדות של הקטגוריה
        public PagedResponse<T>? Books { get; set; }
    }
}
