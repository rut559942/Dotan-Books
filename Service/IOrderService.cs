using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Entities;
using Repository;

namespace Service
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(OrderCreateDto dto, int customerId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<OrderTrackingDto?> GetOrderForTrackingAsync(int orderId, int userId);
        Task<IEnumerable<OrderTrackingDto>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<OrderTrackingDto>> GetPendingOrdersForAdminAsync();
    }
}
