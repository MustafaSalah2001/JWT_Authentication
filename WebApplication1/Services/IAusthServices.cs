using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuthServices
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponeDto?> LoginAsync(UserDto request);
        Task<TokenResponeDto?> RefreshTokenAsync(RefreshTokenRequsetDto request);
    }
}