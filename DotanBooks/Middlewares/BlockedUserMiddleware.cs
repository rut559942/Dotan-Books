using System.Globalization;
using Repository;

namespace DotanBooks.Middlewares
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
        {
            if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (IsExcludedPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Query.TryGetValue("userId", out var userIdValues))
            {
                await _next(context);
                return;
            }

            var userIdRaw = userIdValues.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userIdRaw) ||
                !int.TryParse(userIdRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var userId) ||
                userId <= 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"מזהה משתמש לא תקין.\"}");
                return;
            }

            var isBlocked = await userRepository.IsUserBlocked(userId);
            if (!isBlocked)
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"החשבון חסום לצמיתות ולא ניתן לגשת למערכת.\"}");
        }

        private static bool IsExcludedPath(PathString path)
        {
            return path.StartsWithSegments("/api/Users/login", StringComparison.OrdinalIgnoreCase)
                || path.StartsWithSegments("/api/Users/register", StringComparison.OrdinalIgnoreCase);
        }
    }
}
