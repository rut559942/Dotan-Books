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

            if (savedOrder.OrderNumber == null)//צריך טיפול, לא לזרוק הערה כך
                throw new Exception("Order number was not generated.");

            return savedOrder.OrderNumber;
        }
    }
}

