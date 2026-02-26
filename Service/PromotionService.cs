using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Entities;
using Repository;
using Utils.Exceptions;

namespace Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;

        public PromotionService(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
        {
            var promos = await _promotionRepository.GetAllAsync();

            return promos.Select(p => new PromotionDto
            {
                Id = p.Id,
                Name = p.Title
            });
        }

        public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto)
        {
            if (dto.DiscountedPrice < 0 || dto.DiscountedPrice > 100)
                throw new ValidationException("אחוז הנחה חייב להיות בין 0 ל-100");

            var promotionEntity = new Promotion
            {
                Title = dto.Name,
                DiscountedPrice = dto.DiscountedPrice,
                EndDate = dto.EndDate
            };

            var createdPromo = await _promotionRepository.CreateAsync(promotionEntity);

            return new PromotionDto
            {
                Id = createdPromo.Id,
                Name = createdPromo.Title
            };
        }
    }
}
