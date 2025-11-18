using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SMM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        private readonly JWTServices _jwtService;
        public LoginController(SuperMarketManagementContext context, JWTServices jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserEmailAddress == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            user.LastLoginDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.Now.AddMinutes(60),
                User = new UserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.UserEmailAddress,
                    Role = user.Role?.RoleName
                }
            };
        }
        #endregion

        #region Register
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.UserEmailAddress == user.UserEmailAddress))
                return BadRequest("Email already exists");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.Now.AddMinutes(60),
                User = new UserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.UserEmailAddress,
                    Role = user.Role?.RoleName
                }
            };
        }
        #endregion
    }
}
