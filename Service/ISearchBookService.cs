using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace Service
{
   public interface ISearchBookService
    {
        Task<IEnumerable<BookAutocompleteDto>> GetAutocompleteAsync(string term);
        Task<PagedResponse<BookListDto>> GetFullSearchAsync(string term, int page, int pageSize);
    }
}
