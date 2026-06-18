using DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DotanBooks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SemanticSearchController : ControllerBase
    {
        private readonly HttpClient _http;
        private readonly IBooksService _booksService;

        public SemanticSearchController(IHttpClientFactory factory, IBooksService booksService)
        {
            _http = factory.CreateClient();
            _booksService = booksService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<SemanticBookResultDto>());

            var books = await _booksService.GetAllAsync();

            var productList = books.Select(b => new
            {
                id = b.Id,
                title = b.Title ?? string.Empty,
                summary = b.Summary ?? string.Empty
            }).ToList();

            var payload = new
            {
                query = term,
                products = productList,
                topK = 5
            };

            var res = await _http.PostAsJsonAsync("http://localhost:8000/search", payload);
            if (!res.IsSuccessStatusCode)
                return StatusCode(500, "AI service unavailable");

            var searchResponse = await res.Content.ReadFromJsonAsync<PythonSearchResponse>();
            if (searchResponse?.Results == null)
                return Ok(new List<SemanticBookResultDto>());

            var bookById = books.ToDictionary(b => b.Id);

            var results = searchResponse.Results
                .Where(r => bookById.ContainsKey(r.Id))
                .Select(r =>
                {
                    var b = bookById[r.Id];
                    return new SemanticBookResultDto
                    {
                        Id = b.Id,
                        Title = b.Title ?? string.Empty,
                        AuthorName = b.Author?.Name ?? string.Empty,
                        Price = b.Price,
                        ImageUrl = b.ImageUrl,
                        CategoryName = b.Category?.Name,
                        Score = r.Score
                    };
                })
                .ToList();

            return Ok(results);
        }
    }

    file record PythonSearchResponse(List<PythonSearchResultItem> Results);
    file record PythonSearchResultItem(int Id, double Score);
}
