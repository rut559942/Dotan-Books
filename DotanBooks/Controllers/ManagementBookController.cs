using DTOs;
using DotanBooks.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Utils.Exceptions;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementBookController : ControllerBase
    {
        private readonly IManagementBookService _bookService;

        public ManagementBookController(IManagementBookService bookService)
        {
            _bookService = bookService;
        }
        
        [HttpGet("management-data")]

        public async Task<ActionResult<ManagementBookDto>> GetManagementData()
        {
            var data = await _bookService.GetManagementDataAsync();

            if (data == null) throw new NotFoundException("לא ניתן היה לטעון נתוני ניהול");

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] ManagementBookUpsertRequest request)
        {
            if (request == null) throw new ValidationException("נתוני הספר אינם תקינים");

            var dto = new UpdateOrCreateBookDto
            {
                Title = request.Title,
                Summary = request.Summary,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsHardPages = request.IsHardPages,
                AuthorId = request.AuthorId,
                CategoryId = request.CategoryId,
                PromotionId = request.PromotionId
            };

            await _bookService.CreateBookAsync(dto, request.ImageFile);
            return Ok(new { message = "הספר נוסף בהצלחה למערכת" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] ManagementBookUpsertRequest request)
        {
            if (request == null) throw new ValidationException("נתוני הספר אינם תקינים");

            var dto = new UpdateOrCreateBookDto
            {
                Title = request.Title,
                Summary = request.Summary,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsHardPages = request.IsHardPages,
                AuthorId = request.AuthorId,
                CategoryId = request.CategoryId,
                PromotionId = request.PromotionId
            };

            await _bookService.UpdateBookAsync(id, dto, request.ImageFile);
            return Ok(new { message = "פרטי הספר עודכנו בהצלחה" });
        }

    }
}
