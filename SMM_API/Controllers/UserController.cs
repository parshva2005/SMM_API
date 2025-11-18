using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Helper;
using SMM_API.Models;
using System.Security.Claims;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public UserController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL User

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
        {
            return await _context.Users
                .Include(u => u.Role)
                .ToListAsync();
        }
        #endregion

        #region GET User BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var User = await _context.Users
                .FirstOrDefaultAsync(c => c.UserId == id);

            return User == null ? NotFound() : Ok(User);
        }
        #endregion

        #region DELETE User
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var User = await _context.Users.FindAsync(id);
            if (User == null) return NotFound();

            if (!string.IsNullOrEmpty(User.FilePath))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", User.FilePath);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }
            _context.Users.Remove(User);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region INSERT User
        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Start transaction to ensure both operations succeed or fail together
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (user.File != null)
                {
                    var relativePath = ImageHelper.SaveImageToFile(user.File, "Images/Users");
                    user.FilePath = relativePath;
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Check if user role is Customer and auto-create customer record
                if (user.RoleId != null)
                {
                    var role = await _context.Roles.FindAsync(user.RoleId);
                    if (role != null && role.RoleName == "Customer")
                    {
                        // Create corresponding customer record
                        var customer = new Customer
                        {
                            Name = user.UserName,
                            Email = user.UserEmailAddress,
                            PhoneNumber = user.UserMobileNumber,
                            Address = user.UserAddress,
                            RegistrationDate = DateTime.UtcNow,
                            // Map other relevant fields from user to customer as needed
                            UserId = user.UserId // Link to the user
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error creating user and customer", error = ex.Message });
            }
        }
        #endregion

        #region UPDATE User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User User)
        {
            if (id != User.UserId) return BadRequest();

            var existingUser = await _context.Users.FindAsync(id);
            _context.Entry(User).State = EntityState.Modified;
            if (User.File != null)
            {
                if (!string.IsNullOrEmpty(existingUser.FilePath))
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingUser.FilePath);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }

                var relativePath = ImageHelper.SaveImageToFile(User.File, "Images/Users");
                existingUser.FilePath = relativePath;
            }
            _context.Entry(User).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region GET User BY USER
        [HttpGet("user/{id}")]
        public async Task<ActionResult<User>> GetUserByUserId(int id)
        {
            var User = await _context.Users
                .Where(c => c.UserId == id)
                .ToListAsync();

            return User == null ? NotFound() : Ok(User);
        }
        #endregion

        #region DropDown User
        [HttpGet("Dropdown")]
        public async Task<ActionResult<IEnumerable<object>>> GetUser()
        {
            return await _context.Users
                .Select(u => new { u.UserId, u.UserName })
                .ToListAsync();
        }
        #endregion

        #region GET USER PROFILE
        [HttpGet("profile/{id}")]
        public async Task<ActionResult<User>> GetUserProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return Ok(user);
        }
        #endregion

        #region UPDATE USER PROFILE
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromForm] UserProfileUpdateDto userUpdate)
        {
            if (id != userUpdate.UserId) return BadRequest();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            // Handle image upload
            if (userUpdate.File != null && userUpdate.File.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingUser.FilePath))
                {
                    string oldFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingUser.FilePath);
                    if (System.IO.File.Exists(oldFullPath))
                        System.IO.File.Delete(oldFullPath);
                }

                // Save new image
                var relativePath = ImageHelper.SaveImageToFile(userUpdate.File, "Images/Users");
                existingUser.FilePath = relativePath;
            }

            // Update other fields
            existingUser.UserName = userUpdate.UserName;
            existingUser.UserAddress = userUpdate.UserAddress;
            existingUser.UserMobileNumber = userUpdate.UserMobileNumber;
            existingUser.UserEmailAddress = userUpdate.UserEmailAddress;
            existingUser.ModifyDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(existingUser);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (existingUser == null) return NotFound();
                else
                    throw;
            }
        }

        // Add this DTO class to your project
        public class UserProfileUpdateDto
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string UserAddress { get; set; }
            public string UserMobileNumber { get; set; }
            public string UserEmailAddress { get; set; }
            public IFormFile File { get; set; }
        }
        #endregion
    }
}