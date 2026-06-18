using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BooksRepository : IBooksRepository
    {
        private readonly StoreContext _context;

        public BooksRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}