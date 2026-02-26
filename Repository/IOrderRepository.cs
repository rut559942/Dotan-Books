using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, OrderStatus newStatus);
        Task SaveChangesAsync();
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetActiveOrdersAsync();

    }
}
