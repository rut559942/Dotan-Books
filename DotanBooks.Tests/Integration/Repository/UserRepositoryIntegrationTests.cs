using Entities;
using Repository;
using DotanBooks.Tests.Infrastructure;

namespace DotanBooks.Tests.Integration.Repository;

[Trait("Category", "Integration")]
public class UserRepositoryIntegrationTests : IClassFixture<SqlServerTestDatabaseFixture>
{
    private readonly SqlServerTestDatabaseFixture _fixture;

    public UserRepositoryIntegrationTests(SqlServerTestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddUser_PersistsUserAndReturnsGeneratedId()
    {
        await using var context = _fixture.CreateContext();
        var repository = new UserRepository(context);
        var email = $"add-{Guid.NewGuid():N}@dotanbooks.com";

        var id = await repository.AddUser(new Customer
        {
            Name = "Integration User",
            Email = email,
            Password = "hash",
            Phone = "0500000000"
        });

        Assert.True(id > 0);
        var loaded = await repository.GetUserByEmail(email);
        Assert.NotNull(loaded);
        Assert.Equal(id, loaded!.Id);
    }

    [Fact]
    public async Task BlockUser_WhenUserExists_UpdatesBlockedFields()
    {
        await using var context = _fixture.CreateContext();
        var repository = new UserRepository(context);
        var email = $"block-{Guid.NewGuid():N}@dotanbooks.com";

        var id = await repository.AddUser(new Customer
        {
            Name = "Block Me",
            Email = email,
            Password = "hash",
            Phone = "0500000001"
        });

        await repository.BlockUser(id, "test reason");
        var updated = await repository.GetUserById(id);

        Assert.NotNull(updated);
        Assert.True(updated!.IsBlocked);
        Assert.Equal("test reason", updated.BlockReason);
        Assert.NotNull(updated.BlockedAtUtc);
    }
}
