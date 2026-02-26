using Entities;
using Repository;
using DTOs;
using Utils.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Service
{
    public class ManagementBookService : IManagementBookService
    {
        private readonly IManagementBookRepository _bookRepository;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IPromotionService _promotionService;

        public ManagementBookService(
            IManagementBookRepository bookRepository,
            IAuthorService authorService,
            ICategoryService categoryService,
            IPromotionService promotionService)
        {
            _bookRepository = bookRepository;
            _authorService = authorService;
            _categoryService = categoryService;
            _promotionService = promotionService;
        }

        public async Task<ManagementBookDto> GetManagementDataAsync()
        {
            var authorsTask = _authorService.GetAllAuthorsAsync();
            var categoriesTask = _categoryService.GetAllCategoriesAsync();
            var promotionsTask = _promotionService.GetAllPromotionsAsync();

            await Task.WhenAll(authorsTask, categoriesTask, promotionsTask);

            return new ManagementBookDto
            {
                Author = await authorsTask,
                Categories = await categoriesTask,
                Promotions = await promotionsTask
            };
        }

        public async Task CreateBookAsync(UpdateOrCreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Summary = dto.Summary,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                StockQuantity = dto.StockQuantity,
                IsHardPages = dto.IsHardPages,
                AuthorId = dto.AuthorId,
                CategoryId = dto.CategoryId,
                PromotionId = dto.PromotionId
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(int id, UpdateOrCreateBookDto dto)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                throw new NotFoundException("הספר לא נמצא במערכת");
            }

            book.Title = dto.Title;
            book.Summary = dto.Summary;
            book.Price = dto.Price;
            book.ImageUrl = dto.ImageUrl;
            book.StockQuantity = dto.StockQuantity;
            book.IsHardPages = dto.IsHardPages;
            book.AuthorId = dto.AuthorId;
            book.CategoryId = dto.CategoryId;
            book.PromotionId = dto.PromotionId;

            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveChangesAsync();
        }
    }
}