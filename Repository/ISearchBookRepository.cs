using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
   public interface ISearchBookRepository
    {
        Task<IEnumerable<Book>> GetSuggestionsAsync(string term, int count);
        Task<(IEnumerable<Book> Items, int TotalCount)> SearchBooksAsync(string term, int page, int pageSize);
    }
}
