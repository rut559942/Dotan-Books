using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace DotanBooks.Tests.Infrastructure;

public sealed class SqlServerTestDatabaseFixture : IAsyncLifetime
{
    private readonly string _databaseName = $"DotanBooksTests_{Guid.NewGuid():N}";
    private DbContextOptions<StoreContext>? _options;

    public string ActiveConnectionString { get; private set; } = string.Empty;

    public StoreContext CreateContext()
    {
        if (_options is null)
        {
            throw new InvalidOperationException("Test database is not initialized.");
        }

        return new StoreContext(_options);
    }

    public async Task InitializeAsync()
    {
        var candidates = BuildConnectionCandidates().ToList();
        var errors = new List<Exception>();

        foreach (var candidate in candidates)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(candidate)
                {
                    InitialCatalog = _databaseName,
                    TrustServerCertificate = true
                };

                ActiveConnectionString = builder.ConnectionString;
                _options = new DbContextOptionsBuilder<StoreContext>()
                    .UseSqlServer(ActiveConnectionString)
                    .EnableSensitiveDataLogging()
                    .Options;

                await using var context = new StoreContext(_options);
                await context.Database.MigrateAsync();
                return;
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }

        throw new InvalidOperationException(
            "Could not initialize SQL Server test database. Configure DOTANBOOKS_TEST_CONNECTION or install LocalDB/SQLEXPRESS.",
            new AggregateException(errors));
    }

    public async Task DisposeAsync()
    {
        if (_options is null)
        {
            return;
        }

        await using var context = new StoreContext(_options);
        await context.Database.EnsureDeletedAsync();
    }

    private static IEnumerable<string> BuildConnectionCandidates()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("DOTANBOOKS_TEST_CONNECTION");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            yield return fromEnvironment;
        }

        yield return "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
        yield return "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    }
}
