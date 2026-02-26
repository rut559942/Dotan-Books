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
    public class OrderService : IOrderService
    {
        private readonly Repository.IOrderRepository _orderRepository;
        private readonly IBookByIdRepository _bookRepository;

        public OrderService(Repository.IOrderRepository orderRepository, IBookByIdRepository bookRepository)
        {
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
        }

        public async Task<string> PlaceOrderAsync(OrderCreateDto dto, int customerId)
        {
            var newOrder = new Order
            {
                CustomerId = customerId,
                ShippingAddress = dto.ShippingAddress,
                ShippingCity = dto.ShippingCityId,
                CustomerNotes = dto.CustomerNotes,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Received
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var book = await _bookRepository.GetBookById(item.BookId);
                if (book == null) throw new Exception("ספר לא נמצא");

                if (book.StockQuantity < item.Quantity)
                    throw new Exception($"אין מספיק מלאי לספר {book.Title}");

                total += book.Price * item.Quantity;

                newOrder.OrderItems.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = book.Price
                });

                book.StockQuantity -= item.Quantity;
                await _bookRepository.UpdateBook(book);
            }

            newOrder.TotalAmount = total;
            var savedOrder = await _orderRepository.CreateOrderAsync(newOrder);

            if (savedOrder.OrderNumber == null)
                throw new Exception("שגיאה ביצירת מספר הזמנה");

            return savedOrder.OrderNumber;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return false;

            await _orderRepository.UpdateStatusAsync(orderId, newStatus);
            await _orderRepository.SaveChangesAsync();
            return true;
        }

        public async Task<OrderTrackingDto?> GetOrderForTrackingAsync(int orderId, int userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.CustomerId != userId)
                return null;

            return new OrderTrackingDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
            };
        }

        public async Task<IEnumerable<OrderTrackingDto>> GetUserOrdersAsync(int userId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(userId);
            return orders.Select(order => new OrderTrackingDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
            });
        }

        public async Task<IEnumerable<OrderTrackingDto>> GetPendingOrdersForAdminAsync()
        {
            var orders = await _orderRepository.GetActiveOrdersAsync();
            return orders.Select(order => new OrderTrackingDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString()
            });
        }
    }
}

