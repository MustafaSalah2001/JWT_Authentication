using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AuthServices(UserDbContext context, IConfiguration configuration) : IAusthServices
    {
        public Task<string?> LoginAsync(UserDto requst)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> RegistuerAsync(UserDto requst)
        {
            if  (await context.Users.AnyAsync(u => u.Username == requst.Username))
            {
                return null;
            }
            var user = new User();
            
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, userDto.Password);
            user.Username = userDto.Username;
            user.PasswordHash = hashedPassword;

            return Ok(user);
        }
    }
}
