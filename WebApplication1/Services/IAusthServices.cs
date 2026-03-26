using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAusthServices
    {
        Task<User?> RegistuerAsync(UserDto requst);
        Task<string?> LoginAsync(UserDto requst);

    }
}
