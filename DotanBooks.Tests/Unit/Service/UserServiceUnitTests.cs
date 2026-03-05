using DTOs;
using Entities;
using Moq;
using Repository;
using Service;
using Utils.Exceptions;
using DotanBooks.Tests.Infrastructure;

namespace DotanBooks.Tests.Unit.Service;

public class UserServiceUnitTests
{
    [Fact]
    public async Task Register_WhenPasswordIsWeak_ThrowsValidationException()
    {
        var repository = new Mock<IUserRepository>();
        var sut = new UserService(repository.Object, TestMapperFactory.CreateMapper());
        var dto = new NewUserDto
        {
            Name = "Test User",
            Email = "test@dotanbooks.com",
            Password = "1234",
            Phone = "0501234567"
        };

        await Assert.ThrowsAsync<ValidationException>(() => sut.Register(dto));
        repository.Verify(r => r.IsEmailExists(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_WhenUserIsBlocked_ThrowsForbiddenException()
    {
        var repository = new Mock<IUserRepository>();
        repository
            .Setup(r => r.GetUserByEmail("blocked@dotanbooks.com"))
            .ReturnsAsync(new Customer
            {
                Id = 7,
                Email = "blocked@dotanbooks.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Password#123"),
                IsBlocked = true
            });

        var sut = new UserService(repository.Object, TestMapperFactory.CreateMapper());
        var dto = new LoginDto
        {
            Email = "blocked@dotanbooks.com",
            Password = "Password#123"
        };

        await Assert.ThrowsAsync<ForbiddenException>(() => sut.Login(dto));
    }
}
