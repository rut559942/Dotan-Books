namespace Service
{
    public interface IRatingService
    {
        Task LogRequestAsync(int? userId, string endpoint, int statusCode);
    }
}