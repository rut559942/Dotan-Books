using Entities;

namespace Repository
{
    public interface IBooksRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
    }
}