using System.Globalization;
using Service;

namespace DotanBooks.Middlewares
{
    public class RatingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RatingMiddleware> _logger;

        public RatingMiddleware(RequestDelegate next, ILogger<RatingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IRatingService ratingService)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                var endpoint = $"{context.Request.Method} {context.Request.Path}";
                var userId = TryGetUserIdFromQuery(context.Request.Query["userId"]);
                var statusCode = context.Response.StatusCode;

                try
                {
                    await ratingService.LogRequestAsync(userId, endpoint, statusCode);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to write request rating log for endpoint {Endpoint}", endpoint);
                }
            }
        }

        private static int? TryGetUserIdFromQuery(string? userIdRaw)
        {
            if (string.IsNullOrWhiteSpace(userIdRaw))
            {
                return null;
            }

            if (!int.TryParse(userIdRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId) || userId <= 0)
            {
                return null;
            }

            return userId;
        }
    }
}