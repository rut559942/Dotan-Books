
using DotanBooks.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;
using Entities;

namespace DotanBooks.Controllers { 
[ApiController] 
[Route("api/[controller]")] 
public class ChatController : ControllerBase 
{ 

    private readonly HttpClient _http; 
    private readonly IBooksService _booksService;

 
    public ChatController(IHttpClientFactory factory, IBooksService booksService) 
    {
        _http = factory.CreateClient();
        _booksService = booksService;
    }

 

    [HttpPost] 
    public async Task<IActionResult> Post([FromBody] ChatRequest req) 
    { 
        // Fetch real products from YOUR existing DB service 

    var books = await _booksService.GetAllAsync(); 


    // Only send what the AI needs — keep the payload small 

    var bookList = books.Select(b => new { 

        b.Title, 

        b.Price, 

        Summary = b.Summary, 

        CategoryName = b.Category.Name, 

        InStock = b.StockQuantity > 0 

    }).ToList(); 

 

    var payload = new { 

        message  = req.Message, 

        history  = req.History, 

        products = bookList   // real data from DB 

    }; 

 

        var res = await _http.PostAsJsonAsync( 

            "http://localhost:8000/chat", payload); 

 

        if (!res.IsSuccessStatusCode) 

            return StatusCode(500, "AI service unavailable"); 

 

        var data = await res.Content 

            .ReadFromJsonAsync<ChatResponse>(); 

        return Ok(data); 

    } 

} 

 

public record ChatRequest( 

    string Message, 

    List<HistoryItem> History, 

    List<object> Products);   // empty for now 

 

public record HistoryItem(string Role, string Content); 

public record ChatResponse(string Reply);


} 
