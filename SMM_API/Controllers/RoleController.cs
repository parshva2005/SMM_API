using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public RoleController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAllRole()
        {
            return await _context.Roles
                .ToListAsync();
        }
        #endregion

        #region GET Role BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRoleById(int id)
        {
            var Role = await _context.Roles
                .FirstOrDefaultAsync(c => c.RoleId == id);

            return Role == null ? NotFound() : Ok(Role);
        }
        #endregion

        #region INSERT Role
        [HttpPost]
        public async Task<IActionResult> InsertRole(Role Role)
        {
            _context.Roles.Add(Role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoleById), new { id = Role.RoleId }, Role);
        }

        #endregion

        #region UPDATE Role
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, Role Role)
        {
            if (id != Role.RoleId) return BadRequest();

            _context.Entry(Role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DELETE Role and Its Users
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.Users)
                    .FirstOrDefaultAsync(r => r.RoleId == id);

                if (role == null)
                    return NotFound($"Role with ID {id} not found.");

                // Delete all users of this role
                if (role.Users.Any())
                {
                    _context.Users.RemoveRange(role.Users);
                }

                // Delete the role itself
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Role and its {role.Users.Count} user(s) deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error deleting role: {ex.Message}");
            }
        }
        #endregion

        #region DropDown Role
        [HttpGet("Dropdown")]
        public async Task<ActionResult<IEnumerable<object>>> GetRole()
        {
            return await _context.Roles
                .Select(c => new { c.RoleId, c.RoleName })
                .ToListAsync();
        }
        #endregion

    }
}
