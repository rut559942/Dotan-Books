using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
  public  class UserRepository:IUserRepository
    {
        private readonly StoreContext _context;

        public UserRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<int> AddUser(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetUserByEmail(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetUserById(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task UpdateUser(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserBlocked(int id)
        {
            return await _context.Customers
                .Where(c => c.Id == id)
                .Select(c => c.IsBlocked)
                .FirstOrDefaultAsync();
        }

        public async Task BlockUser(int id, string reason)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || customer.IsBlocked)
            {
                return;
            }

            customer.IsBlocked = true;
            customer.BlockedAtUtc = DateTime.UtcNow;
            customer.BlockReason = reason;
            await _context.SaveChangesAsync();
        }
    }
}

