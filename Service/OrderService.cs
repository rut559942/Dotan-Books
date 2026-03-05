using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs;
using Entities;
using Repository;
using Utils.Exceptions;

namespace Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBookByIdRepository _bookRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IBookByIdRepository bookRepository,
            IEmailService emailService,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _bookRepository = bookRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<string> PlaceOrderAsync(OrderCreateDto dto, int customerId)
        {
            if (customerId <= 0)
            {
                throw new ValidationException("משתמש לא מזוהה");
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new ValidationException("הסל ריק");
            }

            var newOrder = _mapper.Map<Order>(dto);
            newOrder.CustomerId = customerId;
            newOrder.OrderDate = DateTime.Now;
            newOrder.Status = OrderStatus.Received;
            newOrder.OrderItems.Clear();

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var book = await _bookRepository.GetBookById(item.BookId);
                if (book == null)
                {
                    throw new NotFoundException($"ספר עם מזהה {item.BookId} לא נמצא");
                }

                if (NormalizeCurrency(item.ClientUnitPrice) != NormalizeCurrency(book.Price))
                {
                    await _userRepository.BlockUser(customerId, "Tampering detected: client unit price mismatch.");
                    throw new UnprocessableEntityException($"זוהתה חריגה במחיר עבור הספר {book.Title}");
                }

                var expectedLineTotal = NormalizeCurrency(book.Price * item.Quantity);
                if (NormalizeCurrency(item.ClientLineTotal) != expectedLineTotal)
                {
                    await _userRepository.BlockUser(customerId, "Tampering detected: client line total mismatch.");
                    throw new UnprocessableEntityException($"זוהתה חריגה בסכום השורה עבור הספר {book.Title}");
                }

                if (book.StockQuantity < item.Quantity)
                {
                    throw new ConflictException($"אין מספיק מלאי לספר {book.Title}");
                }

                total += expectedLineTotal;

                var orderItem = _mapper.Map<OrderItem>(item);
                orderItem.PriceAtPurchase = book.Price;
                newOrder.OrderItems.Add(orderItem);

                book.StockQuantity -= item.Quantity;
                await _bookRepository.UpdateBook(book);
            }

            var expectedOrderTotal = NormalizeCurrency(total);
            if (NormalizeCurrency(dto.ClientOrderTotal) != expectedOrderTotal)
            {
                await _userRepository.BlockUser(customerId, "Tampering detected: client order total mismatch.");
                throw new UnprocessableEntityException("זוהתה חריגה בסכום הכולל של ההזמנה");
            }

            newOrder.TotalAmount = expectedOrderTotal;
            var savedOrder = await _orderRepository.CreateOrderAsync(newOrder);

            if (string.IsNullOrWhiteSpace(savedOrder.OrderNumber))
            {
                throw new ConflictException("שגיאה ביצירת מספר הזמנה");
            }

            var user = await _userRepository.GetUserById(customerId);
            string emailBody = $@"
               <h3>שלום,</h3>
               <p>הזמנתך שמספרה <b>{savedOrder.OrderNumber}</b> התקבלה בהצלחה!</p>
               <p>תודה שבחרת בדותן ספרים.</p>
               <p>בשביל הזמנה אמיתית, היכנס לקישור הבא:<br/>
               <a href='https://www.dotansfarim.co.il'>www.dotansfarim.co.il</a></p>";
            if (user != null && !string.IsNullOrEmpty(user.Email))

            {
                await _emailService.SendEmailAsync(user.Email, "אישור הזמנה - דוטן ספרים", emailBody);

            }

            return savedOrder.OrderNumber!;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            if (orderId <= 0)
            {
                throw new ValidationException("מזהה הזמנה לא תקין");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("הזמנה לא נמצאה");
            }

            if (!IsValidStatusTransition(order.Status, newStatus))
            {
                throw new UnprocessableEntityException("מעבר סטטוס לא תקין עבור ההזמנה");
            }

            await _orderRepository.UpdateStatusAsync(orderId, newStatus);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task<OrderTrackingDto> GetOrderForTrackingAsync(int orderId, int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException("משתמש לא מזוהה");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException("הזמנה לא נמצאה");
            }

            if (order.CustomerId != userId)
            {
                throw new ForbiddenException("אין הרשאה לצפות בהזמנה זו");
            }

            return _mapper.Map<OrderTrackingDto>(order);
        }

        public async Task<IEnumerable<OrderTrackingDto>> GetUserOrdersAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ValidationException("משתמש לא מזוהה");
            }

            var orders = await _orderRepository.GetByCustomerIdAsync(userId);
            return _mapper.Map<IEnumerable<OrderTrackingDto>>(orders);
        }

        public async Task<IEnumerable<OrderTrackingDto>> GetPendingOrdersForAdminAsync()
        {
            var orders = await _orderRepository.GetActiveOrdersAsync();
            return _mapper.Map<IEnumerable<OrderTrackingDto>>(orders);
        }

        private static decimal NormalizeCurrency(decimal amount)
        {
            return Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus nextStatus)
        {
            if (currentStatus == nextStatus)
            {
                return true;
            }

            return currentStatus switch
            {
                OrderStatus.Received => nextStatus == OrderStatus.Packed,
                OrderStatus.Packed => nextStatus == OrderStatus.Shipped,
                OrderStatus.Shipped => nextStatus == OrderStatus.InTransit,
                OrderStatus.InTransit => nextStatus == OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                _ => false
            };
        }
    }
}
