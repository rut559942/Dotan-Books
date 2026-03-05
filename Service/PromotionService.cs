using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs;
using Entities;
using Repository;
using Utils.Exceptions;

namespace Service
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(IPromotionRepository promotionRepository, IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PromotionDto>> GetAllPromotionsAsync()
        {
            var promos = await _promotionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PromotionDto>>(promos);
        }

        public async Task<PromotionDto> CreatePromotionAsync(CreatePromotionDto dto)
        {
            if (dto.DiscountedPrice < 0 || dto.DiscountedPrice > 100)
                throw new UnprocessableEntityException("אחוז הנחה חייב להיות בין 0 ל-100");

            if (dto.EndDate.HasValue && dto.EndDate.Value.Date < DateTime.UtcNow.Date)
                throw new UnprocessableEntityException("תאריך סיום מבצע לא יכול להיות בעבר");

            var promotionEntity = _mapper.Map<Promotion>(dto);

            var createdPromo = await _promotionRepository.CreateAsync(promotionEntity);

            return _mapper.Map<PromotionDto>(createdPromo);
        }
    }
}
