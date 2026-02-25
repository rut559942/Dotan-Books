using DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) => _orderService = orderService;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] OrderCreateDto dto, [FromQuery] int userId)
        {
            if (userId <= 0) return BadRequest("משתמש לא מזוהה");

            try
            {
                var orderNumber = await _orderService.PlaceOrderAsync(dto, userId);
                return Ok(new { OrderNumber = orderNumber });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);//לטפל בהחזרות
            }
        }
    }
}