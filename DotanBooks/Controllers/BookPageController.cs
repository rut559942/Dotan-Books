using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookPageController : ControllerBase
    {
        private readonly IBookByIdService _Service;

        public BookPageController(IBookByIdService Service)
        {
            _Service = Service;
        }
        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetBookById(int bookId)
        {
            var result = await _Service.GetBookById(bookId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
