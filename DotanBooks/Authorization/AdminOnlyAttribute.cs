using Microsoft.AspNetCore.Authorization;

namespace DotanBooks.Authorization
{
    public class AdminOnlyAttribute : AuthorizeAttribute
    {
        public AdminOnlyAttribute()
        {
            Roles = "Admin";
        }
    }
}