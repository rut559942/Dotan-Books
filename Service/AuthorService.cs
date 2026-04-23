using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Service.Caching;

namespace Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _cacheSettings;

        public AuthorService(
            IAuthorRepository authorRepository,
            IDistributedCache cache,
            IOptions<CacheSettings> cacheSettings)
        {
            _authorRepository = authorRepository;
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var cachedAuthors = await _cache.GetRecordAsync<List<AuthorDto>>(CacheKeys.AuthorsAll);
            if (cachedAuthors is not null)
            {
                return cachedAuthors;
            }

            var authors = await _authorRepository.GetAllAsync();
            var result = authors.Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            await _cache.SetRecordAsync(
                CacheKeys.AuthorsAll,
                result,
                TimeSpan.FromSeconds(_cacheSettings.DefaultTtlSeconds));

            return result;
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("שם הסופר אינו יכול להיות ריק");

            var authorEntity = new Author { Name = dto.Name };
            var created = await _authorRepository.CreateAsync(authorEntity);
            await _cache.RemoveAsync(CacheKeys.AuthorsAll);

            return new AuthorDto { Id = created.Id, Name = created.Name };
        }
    }
}
