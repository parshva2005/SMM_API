using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SMM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SalesReportController : ControllerBase
    {
        #region Configuration
        private readonly SuperMarketManagementContext _context;

        public SalesReportController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET All Sales Reports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesReport>>> GetAllSalesReport()
        {
            return await _context.SalesReports.ToListAsync();
        }
        #endregion

        #region GET Single Sales Report
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesReport>> GetById(int id)
        {
            var report = await _context.SalesReports.FindAsync(id);
            return report == null ? NotFound() : Ok(report);
        }
        #endregion

        #region CREATE Sales Report
        [HttpPost]
        public async Task<ActionResult<SalesReport>> Create(SalesReport report)
        {
            _context.SalesReports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = report.OrderId }, report);
        }
        #endregion

        #region UPDATE Sales Report
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SalesReport report)
        {
            if (id != report.OrderId)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(report).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }
        #endregion

        #region DELETE Sales Report
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.SalesReports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            _context.SalesReports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region Helper Method
        private bool ReportExists(int id)
        {
            return _context.SalesReports.Any(e => e.OrderId == id);
        }
        #endregion
    }
}
