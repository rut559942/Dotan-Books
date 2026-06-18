using Entities;

namespace Service
{
    public interface IBooksService
    {
        Task<IEnumerable<Book>> GetAllAsync();
    }
}