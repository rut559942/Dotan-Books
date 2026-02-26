using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Entities;

namespace Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return authors.Select(a => new AuthorDto
            {
                Id = a.Id,
                Name = a.Name
            });
        }

        public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ValidationException("שם הסופר אינו יכול להיות ריק");

            var authorEntity = new Author { Name = dto.Name };
            var created = await _authorRepository.CreateAsync(authorEntity);

            return new AuthorDto { Id = created.Id, Name = created.Name };
        }
    }
}
