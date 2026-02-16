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
        Task<CategoryBooksResult<BookListDto>> GetAllBook(int CategoryId, int pageSize, int page);

    }
}
