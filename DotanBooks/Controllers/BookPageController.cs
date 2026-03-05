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
        private readonly IBookByIdService _bookByIdService;

        public BookPageController(IBookByIdService bookByIdService)
        {
            _bookByIdService = bookByIdService;
        }
        [HttpGet("{bookId}")]
        public async Task<ActionResult<BookDto>> GetBookById(int bookId)
        {
            var result = await _bookByIdService.GetBookById(bookId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
