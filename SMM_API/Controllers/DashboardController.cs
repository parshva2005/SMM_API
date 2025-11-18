using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SMM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly SuperMarketManagementContext _context;

        public DashboardController(SuperMarketManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                // Calculate total products (excluding removed ones)
                var totalProducts = await _context.Products
                    .Where(p => !p.IsRemoved.HasValue || !p.IsRemoved.Value)
                    .CountAsync();

                // Calculate today's sales (sum of order amounts for today)
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                var todaySales = await _context.Orders
                    .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow && o.Status == "Completed")
                    .SumAsync(o => o.TotalPrice);

                // Count new orders for today (orders with status "Processing")
                var newOrders = await _context.Orders
                    .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow && o.Status == "Processing")
                    .CountAsync();

                // Count low stock items (stock less than or equal to reorder level)
                var lowStockItems = await _context.Products
                    .Where(p => (!p.IsRemoved.HasValue || !p.IsRemoved.Value) &&
                               p.ProductStock <= (p.ReorderLevel ?? 5))
                    .CountAsync();

                // Get recent orders (top 10)
                var recentOrders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .Select(o => new
                    {
                        o.OrderId,
                        Customer = o.User != null ? o.User.UserName : "Unknown",
                        Date = o.OrderDate,
                        Amount = o.TotalPrice,
                        o.Status
                    })
                    .ToListAsync();

                // Get top selling products (top 10)
                var topSellingProducts = await _context.OrderItems
                    .GroupBy(oi => oi.Product)
                    .Select(g => new
                    {
                        ProductName = g.Key.ProductName,
                        Category = g.Key.Category != null ? g.Key.Category.CategoryName : "Uncategorized",
                        SalesCount = g.Sum(oi => oi.ProductQuantity)
                    })
                    .OrderByDescending(x => x.SalesCount)
                    .Take(10)
                    .ToListAsync();

                var dashboardData = new
                {
                    TotalProducts = totalProducts,
                    TodaySales = todaySales,
                    NewOrders = newOrders,
                    LowStockItems = lowStockItems,
                    RecentOrders = recentOrders,
                    TopSellingProducts = topSellingProducts
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving dashboard data", error = ex.Message });
            }
        }
    }
}