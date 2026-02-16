using DTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IGetByCategoriesRepository
    {
        Task<CategoryBooksResult<Book>> GetAllBooks(int CategoryId, int pageSize, int page);
    }
}
