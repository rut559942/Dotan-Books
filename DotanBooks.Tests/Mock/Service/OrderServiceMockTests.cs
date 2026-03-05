using System.Reflection;
using DTOs;
using Entities;
using Moq;
using Repository;
using Service;
using Utils.Exceptions;
using DotanBooks.Tests.Infrastructure;

namespace DotanBooks.Tests.Mock.Service;

public class OrderServiceMockTests
{
    [Fact]
    public async Task PlaceOrderAsync_WhenClientUnitPriceTampered_BlocksUserAndThrows()
    {
        var orderRepository = new Mock<IOrderRepository>();
        var bookRepository = new Mock<IBookByIdRepository>();
        var emailService = new Mock<IEmailService>();
        var userRepository = new Mock<IUserRepository>();

        bookRepository
            .Setup(r => r.GetBookById(11))
            .ReturnsAsync(new Book
            {
                Id = 11,
                Title = "Clean Code",
                Price = 50m,
                StockQuantity = 4
            });

        var sut = new OrderService(
            orderRepository.Object,
            bookRepository.Object,
            emailService.Object,
            userRepository.Object,
            TestMapperFactory.CreateMapper());

        var dto = new OrderCreateDto
        {
            ShippingAddress = "Herzl 1",
            ShippingCityId = 1,
            ClientOrderTotal = 100m,
            Items =
            [
                new OrderItemDto
                {
                    BookId = 11,
                    Quantity = 2,
                    ClientUnitPrice = 40m,
                    ClientLineTotal = 80m
                }
            ]
        };

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => sut.PlaceOrderAsync(dto, 15));

        userRepository.Verify(r => r.BlockUser(15, It.IsAny<string>()), Times.Once);
        orderRepository.Verify(r => r.CreateOrderAsync(It.IsAny<Order>()), Times.Never);
        emailService.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenRequestIsValid_CreatesOrderAndSendsEmail()
    {
        var orderRepository = new Mock<IOrderRepository>();
        var bookRepository = new Mock<IBookByIdRepository>();
        var emailService = new Mock<IEmailService>();
        var userRepository = new Mock<IUserRepository>();

        bookRepository
            .Setup(r => r.GetBookById(20))
            .ReturnsAsync(new Book
            {
                Id = 20,
                Title = "Domain-Driven Design",
                Price = 70m,
                StockQuantity = 10
            });

        userRepository
            .Setup(r => r.GetUserById(9))
            .ReturnsAsync(new Customer
            {
                Id = 9,
                Email = "user@dotanbooks.com",
                Name = "User"
            });

        orderRepository
            .Setup(r => r.CreateOrderAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order order) =>
            {
                typeof(Order)
                    .GetField("<OrderNumber>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .SetValue(order, "DS999999");
                return order;
            });

        var sut = new OrderService(
            orderRepository.Object,
            bookRepository.Object,
            emailService.Object,
            userRepository.Object,
            TestMapperFactory.CreateMapper());

        var dto = new OrderCreateDto
        {
            ShippingAddress = "Ben Yehuda 5",
            ShippingCityId = 2,
            ClientOrderTotal = 140m,
            Items =
            [
                new OrderItemDto
                {
                    BookId = 20,
                    Quantity = 2,
                    ClientUnitPrice = 70m,
                    ClientLineTotal = 140m
                }
            ]
        };

        var orderNumber = await sut.PlaceOrderAsync(dto, 9);

        Assert.Equal("DS999999", orderNumber);
        bookRepository.Verify(r => r.UpdateBook(It.Is<Book>(b => b.Id == 20 && b.StockQuantity == 8)), Times.Once);
        orderRepository.Verify(r => r.CreateOrderAsync(It.IsAny<Order>()), Times.Once);
        emailService.Verify(r => r.SendEmailAsync("user@dotanbooks.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        userRepository.Verify(r => r.BlockUser(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
