using Entities;
using Repository;

namespace Service
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingService(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task LogRequestAsync(int? userId, string endpoint, int statusCode)
        {
            var rating = new Rating
            {
                RequestDateTime = DateTime.UtcNow,
                UserId = userId,
                Endpoint = endpoint,
                StatusCode = statusCode
            };

            await _ratingRepository.AddAsync(rating);
        }
    }
}