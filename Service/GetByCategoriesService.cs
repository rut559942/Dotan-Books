using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DTOs;
using Repository;
using DotanBooks.Exceptions;

namespace Service
{
    public class GetByCategoriesService : IGetByCategoriesService
    {
        private readonly IGetByCategoriesRepository _repository;

        private readonly IMapper _mapper;
        public GetByCategoriesService(IGetByCategoriesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper=mapper;
        }

        public async Task<CategoryBooksResult<BookListDto>> GetAllBook(int CategoryId, int pageSize, int page)
        {
            var result = await _repository.GetAllBooks(CategoryId, pageSize, page);

            if (result == null)
                    throw new NotFoundException($"Category with ID {categoryId} was not found.");
                    
                       
            return _mapper.Map<CategoryBooksResult<BookListDto>>(result);


        }

    }
}
