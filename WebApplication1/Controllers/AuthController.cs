using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _austhServices;

        public AuthController(IAuthServices austhServices)
        {
            _austhServices = austhServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto userDto)
        {
            var user = await _austhServices.RegisterAsync(userDto);
            if (user == null)
            {
                return BadRequest("User already exists.");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponeDto>> Login([FromBody] UserDto userDto)
        {
            var token = await _austhServices.LoginAsync(userDto);
            if (token == null)
            {
                return BadRequest("Invalid username or password.");
            }
            return Ok(token);
        }
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticationOnlyEndPoint()
        {
            return Ok("You Are Authorization");
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequsetDto request)
        {
            var token = await _austhServices.RefreshTokenAsync(request);
            if (token == null)
                return Unauthorized(new { message = "التوكن غير صالح أو منتهي الصلاحية" });

            return Ok("Refrech Token Done");
        }
        [Authorize(Roles ="Admin")]
        [HttpGet ("only-admin")]
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You Are Admin");
        }
    }
}