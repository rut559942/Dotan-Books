using Entities;
using Repository;
using DTOs;
using Utils.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using AutoMapper;

namespace Service
{
    public class ManagementBookService : IManagementBookService
    {
        private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".avif"];
        private const long MaxImageSizeInBytes = 5 * 1024 * 1024;

        private readonly IManagementBookRepository _bookRepository;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IPromotionService _promotionService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public ManagementBookService(
            IManagementBookRepository bookRepository,
            IAuthorService authorService,
            ICategoryService categoryService,
            IPromotionService promotionService,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _authorService = authorService;
            _categoryService = categoryService;
            _promotionService = promotionService;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
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

        public async Task CreateBookAsync(UpdateOrCreateBookDto dto, IFormFile? imageFile)
        {
            ValidateBookBusinessRules(dto);

            var imageUrl = await SaveImageAsync(imageFile, isRequired: true);

            var book = _mapper.Map<Book>(dto);
            book.ImageUrl = imageUrl;

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(int id, UpdateOrCreateBookDto dto, IFormFile? imageFile)
        {
            ValidateBookBusinessRules(dto);

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                throw new NotFoundException("הספר לא נמצא במערכת");
            }

            _mapper.Map(dto, book);

            if (imageFile != null)
            {
                book.ImageUrl = await SaveImageAsync(imageFile, isRequired: false);
            }

            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveChangesAsync();
        }

        private static void ValidateBookBusinessRules(UpdateOrCreateBookDto dto)
        {
            if (dto.Price <= 0)
            {
                throw new UnprocessableEntityException("מחיר הספר חייב להיות גדול מ-0");
            }

            if (dto.StockQuantity < 0)
            {
                throw new UnprocessableEntityException("כמות מלאי לא יכולה להיות שלילית");
            }
        }

        private async Task<string> SaveImageAsync(IFormFile? imageFile, bool isRequired)
        {
            if (imageFile == null)
            {
                if (isRequired)
                {
                    throw new ValidationException("יש להעלות תמונה לספר");
                }

                return string.Empty;
            }

            if (imageFile.Length <= 0)
            {
                throw new ValidationException("קובץ התמונה אינו תקין");
            }

            if (imageFile.Length > MaxImageSizeInBytes)
            {
                throw new ValidationException("גודל קובץ התמונה חייב להיות עד 5MB");
            }

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
            {
                throw new ValidationException("סוג הקובץ אינו נתמך. ניתן להעלות רק jpg/jpeg/png/webp/avif");
            }

            var webRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                throw new ValidationException("נתיב אחסון התמונות בשרת אינו זמין");
            }

            var targetDirectory = Path.Combine(webRootPath, "images", "books");
            Directory.CreateDirectory(targetDirectory);

            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var filePath = Path.Combine(targetDirectory, uniqueFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/books/{uniqueFileName}";
        }
    }
}