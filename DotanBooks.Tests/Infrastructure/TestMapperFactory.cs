using AutoMapper;
using DTOs;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotanBooks.Tests.Infrastructure;

public static class TestMapperFactory
{
    public static IMapper CreateMapper()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfiles>(),
            NullLoggerFactory.Instance);
        return configuration.CreateMapper();
    }
}
