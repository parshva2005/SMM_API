using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductLogController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public ProductLogController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL ProductLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductLog>>> GetAllProductLog()
        {
            return await _context.ProductLogs
                .ToListAsync();
        }
        #endregion

        #region GET ProductLog BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductLog>> GetProductLogById(int id)
        {
            var ProductLog = await _context.ProductLogs
                .FirstOrDefaultAsync(c => c.ProductLogId == id);

            return ProductLog == null ? NotFound() : Ok(ProductLog);
        }
        #endregion

        #region GET ProductLog BY USER
        [HttpGet("user/{id}")]
        public async Task<ActionResult<ProductLog>> GetProductLogByUserId(int id)
        {
            var ProductLog = await _context.ProductLogs
                .Where(c => c.UserId == id)
                .ToListAsync();

            return ProductLog == null ? NotFound() : Ok(ProductLog);
        }
        #endregion
    }
}
