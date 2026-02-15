using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;


namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchBooksController : ControllerBase
    {
        private readonly ISearchBookService _searchBookService;

        public SearchBooksController(ISearchBookService _searchBookService)
        {
            _searchBookService = _searchBookService;
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string term)
        {
            var results = await _searchBookService.GetAutocompleteAsync(term);
            return Ok(results);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var results = await _searchBookService.GetFullSearchAsync(term, page, pageSize);
            return Ok(results);
        }
    }
}

