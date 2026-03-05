using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IUserRepository
    {
        Task<int> AddUser(Customer customer);
        Task<bool> IsEmailExists(string email);
        Task<Customer?> GetUserByEmail(string email);
        Task UpdateUser(Customer customer);
        Task<Customer?> GetUserById(int id);
        Task<bool> IsUserBlocked(int id);
        Task BlockUser(int id, string reason);
    }
}
