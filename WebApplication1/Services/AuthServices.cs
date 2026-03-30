using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Data;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserDbContext context;
        private readonly IConfiguration configuration;

        public AuthServices(UserDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        // تسجيل المستخدم
        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
                return null;

            var user = new User
            {
                Username = request.Username,
                PasswordHash = new PasswordHasher<User>().HashPassword(null, request.Password),
                Role = "User" // الدور الافتراضي
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        // تسجيل الدخول
        public async Task<TokenResponeDto?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return null;

            var passwordCheck = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (passwordCheck == PasswordVerificationResult.Failed) return null;

            return await CreateTokenResponse(user);
        }

        // تجديد التوكن
        public async Task<TokenResponeDto?> RefreshTokenAsync(RefreshTokenRequsetDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null) return null;

            return await CreateTokenResponse(user);
        }

        // التحقق من صلاحية Refresh Token
        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return null;

            return user;
        }

        // توليد Access Token و Refresh Token
        private async Task<TokenResponeDto> CreateTokenResponse(User user)
        {
            var accessToken = CreateAccessToken(user);
            var refreshToken = await GenerateAndSaveRefreshToken(user);

            return new TokenResponeDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        // توليد Access Token
        private string CreateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["AppSettings:Issuer"],
                audience: configuration["AppSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        // توليد Refresh Token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        // حفظ Refresh Token في قاعدة البيانات
        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await context.SaveChangesAsync();
            return refreshToken;
        }
    }
}