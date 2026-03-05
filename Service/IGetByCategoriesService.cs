using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGetByCategoriesService
    {
        Task<CategoryBooksResult<BookListDto>> GetAllBook(int categoryId, int page, int pageSize);

    }
}
