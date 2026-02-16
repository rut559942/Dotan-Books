using Azure;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
namespace Repository
{
    public class GetByCategoriesRepository:IGetByCategoriesRepository
    {
        private readonly StoreContext _context;

        public GetByCategoriesRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<CategoryBooksResult<Book>> GetAllBooks(int CategoryId ,int pageSize,int page)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == CategoryId);

            if (category == null)//לשנות ל NotFoundException שתופס מידלוור גלובלי שמחזיר 404
                return null;

            var query = _context.Books
                 .Include(b => b.Author)
                 .Where(b => b.CategoryId == CategoryId);

            var totalCount = await query.CountAsync();


            int skip = (page - 1) * pageSize;

            var books= await query
                .OrderByDescending(b => b.Id)
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            return new CategoryBooksResult<Book>
            {
                Category = category,


               Books = new PagedResponse<Book>
               {
                   Items = books,
                   TotalCount = totalCount,
                   Page = page,
                   PageSize = pageSize
               }
            };

        }

    }
}
