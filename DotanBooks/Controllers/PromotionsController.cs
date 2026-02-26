using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using DTOs;

namespace DotanBooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PromotionDto>>> GetAll()
        {
            var promos = await _promotionService.GetAllPromotionsAsync();
            return Ok(promos);
        }

        [HttpPost]
        public async Task<ActionResult<CreatePromotionDto>> Create([FromBody] CreatePromotionDto promotion)
        {
            var newPromo = await _promotionService.CreatePromotionAsync(promotion);
            return CreatedAtAction(nameof(GetAll), new { id = newPromo.Id }, newPromo);
        }
    }
}
