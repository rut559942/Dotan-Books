using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs;
using Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Repository;
using Service.Caching;
using Utils.Exceptions;

namespace Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly CacheSettings _cacheSettings;

        public PromotionService(
            IPromotionRepository promotionRepository,
            IMapper mapper,
            IDistributedCache cache,
            IOptions<CacheSettings> cacheSettings)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _cache = cache;
            _cacheSettings = cacheSettings.Value;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
        {
            var cachedPromotions = await _cache.GetRecordAsync<List<PromotionDto>>(CacheKeys.PromotionsAll);
            if (cachedPromotions is not null)
            {
                return cachedPromotions;
            }

            var promos = await _promotionRepository.GetAllAsync();
            var result = _mapper.Map<List<PromotionDto>>(promos);

            await _cache.SetRecordAsync(
                CacheKeys.PromotionsAll,
                result,
                TimeSpan.FromSeconds(_cacheSettings.DefaultTtlSeconds));

            return result;
        }

        public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto)
        {
            if (dto.DiscountedPrice < 0 || dto.DiscountedPrice > 100)
                throw new UnprocessableEntityException("אחוז הנחה חייב להיות בין 0 ל-100");

            if (dto.EndDate.HasValue && dto.EndDate.Value.Date < DateTime.UtcNow.Date)
                throw new UnprocessableEntityException("תאריך סיום מבצע לא יכול להיות בעבר");

            var promotionEntity = _mapper.Map<Promotion>(dto);

            var createdPromo = await _promotionRepository.CreateAsync(promotionEntity);
            await _cache.RemoveAsync(CacheKeys.PromotionsAll);

            return _mapper.Map<PromotionDto>(createdPromo);
        }
    }
}
