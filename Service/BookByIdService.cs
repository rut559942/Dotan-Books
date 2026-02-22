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
    public class BookByIdService: IBookByIdService
    {
        private readonly IBookByIdRepository _repository;
        private readonly IMapper _mapper;
        public BookByIdService(IBookByIdRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BookDto?> GetBookById(int bookId)
        {
            var book = await _repository.GetBookById(bookId);
            if (book == null)
            {
                throw new NotFoundException($"Book with id {bookId} was not found");

            }
            return _mapper.Map<BookDto>(book);
        }
    }
}
