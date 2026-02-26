using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace Service
{
    public interface IManagementBookService
    {
        Task<ManagementBookDto> GetManagementDataAsync();
        Task CreateBookAsync(UpdateOrCreateBookDto dto);
        Task UpdateBookAsync(int id, UpdateOrCreateBookDto dto);
    }
}
