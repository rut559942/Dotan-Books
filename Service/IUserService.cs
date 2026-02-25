using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
  public interface IUserService
    {
        Task<int> Register(NewUserDto newUserDto);
        Task<CustomerDto> Login(LoginDto loginDto);
        Task Update(int id, UpdateUserDto updateUserDto);
    }
}
