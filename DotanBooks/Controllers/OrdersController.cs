using DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;
using Entities;

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
            var orderNumber = await _orderService.PlaceOrderAsync(dto, userId);
            return Ok(new { OrderNumber = orderNumber });
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] OrderStatus newStatus)
        {
            await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
            return Ok(new { message = "סטטוס עודכן בהצלחה" });
        }

        [HttpGet("{orderId}/track")]
        public async Task<ActionResult<OrderTrackingDto>> GetTracking(int orderId, [FromQuery] int userId)
        {
            var trackingInfo = await _orderService.GetOrderForTrackingAsync(orderId, userId);
            return Ok(trackingInfo);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<IEnumerable<OrderTrackingDto>>> GetMyOrders([FromQuery] int userId)
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("all-pending")]
        public async Task<ActionResult<IEnumerable<OrderTrackingDto>>> GetAllPendingOrders()
        {
            var orders = await _orderService.GetPendingOrdersForAdminAsync();
            return Ok(orders);
        }
    }
}