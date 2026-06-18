using Entities;
using Repository;

namespace Service
{
    public class BooksService : IBooksService
    {
        private readonly IBooksRepository _booksRepository;

        public BooksService(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _booksRepository.GetAllAsync();
        }
    }
}