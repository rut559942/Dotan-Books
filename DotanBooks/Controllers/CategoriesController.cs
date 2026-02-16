using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IGetByCategoriesService _Service;

        public CategoriesController(IGetByCategoriesService Service)
        {
            _Service = Service;
        }
        [HttpGet("{categoryId}/books")]
        public async Task<ActionResult<CategoryBooksResult<BookDto>>> GetAllBooks(int categoryId, [FromQuery] int page, [FromQuery]  int pageSize)
        {
            var result = await _Service.GetAllBook(categoryId, page, pageSize);
            if (result == null || result.Books == null || !result.Books.Items.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
