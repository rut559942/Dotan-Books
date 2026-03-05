using Entities;

namespace Repository
{
    public class RatingRepository : IRatingRepository
    {
        private readonly StoreContext _context;

        public RatingRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }
    }
}