using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorsController(IAuthorService authorService) => _authorService = authorService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll()
        {
            return Ok(await _authorService.GetAllAuthorsAsync());
        }

        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
        {
            var newAuthor = await _authorService.CreateAuthorAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = newAuthor.Id }, newAuthor);
        }
    }
}
