using Entities;
using Repository;
using DotanBooks.Tests.Infrastructure;

namespace DotanBooks.Tests.Integration.Repository;

[Trait("Category", "Integration")]
public class OrderRepositoryIntegrationTests : IClassFixture<SqlServerTestDatabaseFixture>
{
    private readonly SqlServerTestDatabaseFixture _fixture;

    public OrderRepositoryIntegrationTests(SqlServerTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateOrderAsync_PersistsOrderAndGeneratesOrderNumber()
    {
        await using var context = _fixture.CreateContext();
        var customer = new Customer
        {
            Name = "Order Customer",
            Email = $"order-{Guid.NewGuid():N}@dotanbooks.com",
            Password = "hash",
            Phone = "0500001000"
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        var repository = new OrderRepository(context);
        var order = await repository.CreateOrderAsync(new Order
        {
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = "Street 1",
            ShippingCity = 4,
            Status = OrderStatus.Received,
            TotalAmount = 99.5m
        });

        Assert.True(order.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(order.OrderNumber));
        Assert.StartsWith("DS", order.OrderNumber);
    }

    [Fact]
    public async Task GetActiveOrdersAsync_ExcludesDeliveredOrders()
    {
        await using var context = _fixture.CreateContext();
        var customer = new Customer
        {
            Name = "Active Customer",
            Email = $"active-{Guid.NewGuid():N}@dotanbooks.com",
            Password = "hash",
            Phone = "0500002000"
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();

        await context.Orders.AddRangeAsync(
            new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow.AddMinutes(-10),
                Status = OrderStatus.Received,
                TotalAmount = 40m
            },
            new Order
            {
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Delivered,
                TotalAmount = 70m
            });
        await context.SaveChangesAsync();

        var repository = new OrderRepository(context);
        var activeOrders = (await repository.GetActiveOrdersAsync()).ToList();

        Assert.NotEmpty(activeOrders);
        Assert.DoesNotContain(activeOrders, o => o.Status == OrderStatus.Delivered);
    }
}
