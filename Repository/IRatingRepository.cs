using Entities;

namespace Repository
{
    public interface IRatingRepository
    {
        Task AddAsync(Rating rating);
    }
}