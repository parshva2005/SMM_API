using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly SuperMarketManagementContext _context;

        public OrderItemController(SuperMarketManagementContext context)
        {
            _context = context;
        }

        #region GET OrderItems by Order ID
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            return Ok(orderItems);
        }
        #endregion

        #region CREATE OrderItem
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] OrderItem orderItem)
        {
            orderItem.CreationDate = DateTime.UtcNow;

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderItemsByOrderId),
                new { orderId = orderItem.OrderId }, orderItem);
        }
        #endregion

        #region DELETE OrderItem
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return NotFound();

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion
    }
}