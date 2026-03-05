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
        private readonly IGetByCategoriesService _categoriesService;

        public CategoriesController(IGetByCategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }
        [HttpGet("{categoryId}/books")]
        public async Task<ActionResult<CategoryBooksResult<BookDto>>> GetAllBooks(int categoryId, [FromQuery] int page, [FromQuery]  int pageSize)
        {
            var result = await _categoriesService.GetAllBook(categoryId, page, pageSize);
            if (result == null || result.Books == null || !result.Books.Items.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
