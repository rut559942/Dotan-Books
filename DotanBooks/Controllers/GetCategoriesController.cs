using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DTOs;
using Service;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var result = await _categoryService.GetAllCategoriesAsync();

            if (result == null || !result.Any())
            {
                return NoContent(); // או return Ok(new List<CategoryDto>());לטפל בהחזרות ריקות בצורה מתאימה 
            }

            return Ok(result);
        }
    }
}
