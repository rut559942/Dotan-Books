using System.Net;
using System.Text.Json;
using DotanBooks.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace DotanBooks.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // המשך הזרימה הרגילה של הבקשה
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                // טיפול ספציפי בשגיאת 404 שזרקנו מהסרביס
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                // טיפול בשגיאות לא צפויות (500)
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                // הצגת פרטים טכניים רק בסביבת פיתוח
                Message = _env.IsDevelopment() ? exception.Message : "Resource not found",
                Detailed = _env.IsDevelopment() ? exception.StackTrace : null
    
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}