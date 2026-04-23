using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Repository;
using Service.Caching;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _cacheSettings;

        public CategoryService(
            ICategoryRepository repository,
            IMapper mapper,
            IDistributedCache cache,
            IOptions<CacheSettings> cacheSettings)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var cachedCategories = await _cache.GetRecordAsync<List<CategoryDto>>(CacheKeys.CategoriesAll);
            if (cachedCategories is not null)
            {
                return cachedCategories;
            }

            var categories = await _repository.GetAllCategoriesAsync();
            var result = _mapper.Map<List<CategoryDto>>(categories);

            await _cache.SetRecordAsync(
                CacheKeys.CategoriesAll,
                result,
                TimeSpan.FromSeconds(_cacheSettings.DefaultTtlSeconds));

            return result;
        }
    }
}
