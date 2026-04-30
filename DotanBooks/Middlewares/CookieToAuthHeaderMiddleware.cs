namespace DotanBooks.Middlewares
{
    public class CookieToAuthHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieToAuthHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var hasAuthorizationHeader = context.Request.Headers.ContainsKey("Authorization");

            if (!hasAuthorizationHeader && context.Request.Cookies.TryGetValue("auth", out var token) && !string.IsNullOrWhiteSpace(token))
            {
                context.Request.Headers.Authorization = $"Bearer {token}";
            }

            await _next(context);
        }
    }
}