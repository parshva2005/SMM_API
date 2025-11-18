using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLogController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public UserLogController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL UserLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserLog>>> GetAllUserLog()
        {
            return await _context.UserLogs
                .ToListAsync();
        }
        #endregion

        #region GET UserLog BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserLog>> GetUserLogById(int id)
        {
            var UserLog = await _context.UserLogs
                .FirstOrDefaultAsync(c => c.UserLogId == id);

            return UserLog == null ? NotFound() : Ok(UserLog);
        }
        #endregion

        #region GET UserLog BY USER
        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserLog>> GetUserLogByUserId(int id)
        {
            var UserLog = await _context.UserLogs
                .Where(c => c.UserId == id)
                .ToListAsync();

            return UserLog == null ? NotFound() : Ok(UserLog);
        }
        #endregion
    }
}
