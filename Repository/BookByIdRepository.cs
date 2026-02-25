using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
namespace Repository
{
    public class BookByIdRepository: IBookByIdRepository
    {
        private readonly StoreContext _context;

        public BookByIdRepository(StoreContext context)
        {
            _context = context;
        }
         public async Task<Book?> GetBookById(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Promotion)
                .FirstOrDefaultAsync(b => b.Id == bookId);
            return book;
        }

        public async Task UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
