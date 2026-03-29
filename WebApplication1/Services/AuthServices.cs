using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AuthServices(UserDbContext context, IConfiguration configuration) : IAusthServices
    {
        public async Task<string?> LoginAsync(UserDto requst)
        {
            var user =await context.Users.FirstOrDefaultAsync(u => u.Username == requst.Username);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, requst.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            string token = CreateToken(user);
            return CreateToken(user);
        }

        public async Task<User?> RegistuerAsync(UserDto requst)
        {
            if  (await context.Users.AnyAsync(u => u.Username == requst.Username))
            {
                return null;
            }
            var user = new User();
            
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, requst.Password);
            user.Username = requst.Username;
            user.PasswordHash = hashedPassword;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:token")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSetting:Issuer"),
                audience: configuration.GetValue<string>("AppSetting:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds

                );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
