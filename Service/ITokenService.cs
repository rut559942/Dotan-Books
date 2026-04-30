using Entities;

namespace Service
{
    public interface ITokenService
    {
        string GenerateToken(Customer customer);
    }
}