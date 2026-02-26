using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface IManagementBookService
    {
        Task<ManagementBookDto> GetManagementDataAsync();
        Task CreateBookAsync(UpdateOrCreateBookDto dto, IFormFile? imageFile);
        Task UpdateBookAsync(int id, UpdateOrCreateBookDto dto, IFormFile? imageFile);
    }
}
