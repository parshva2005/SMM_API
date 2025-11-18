using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;
using System.Text;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly SuperMarketManagementContext _context;

        public OrderController(SuperMarketManagementContext context)
        {
            _context = context;
        }

        #region GET ALL Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Customer)
                .ToListAsync();
        }
        #endregion

        #region GET Orders by User ID
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(int userId)
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }
        #endregion

        #region GET Order BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            return order == null ? NotFound() : Ok(order);
        }
        #endregion

        #region CREATE Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.CreationDate = DateTime.UtcNow;
            order.OrderNumber = GenerateOrderNumber();

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }
        #endregion

        #region UPDATE Order Status
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (order == null) return NotFound();

            order.ModifyDate = DateTime.UtcNow;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region UPDATE Order Status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            order.ModifyDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DELETE Order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Checkout/Buy Now
        [HttpPost("buynow")]
        public async Task<IActionResult> BuyNow([FromBody] BuyNowRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create order
                var order = new Order
                {
                    UserId = request.UserId,
                    CustomerId = request.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = request.TotalAmount,
                    TaxAmount = request.TaxAmount,
                    DiscountAmount = request.DiscountAmount,
                    PaymentMethod = "Buy Now",
                    Status = "Completed",
                    OrderNumber = GenerateOrderNumber(),
                    CreationDate = DateTime.UtcNow,
                    Notes = "Order placed via Buy Now option"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add order items
                foreach (var item in request.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        ProductQuantity = item.Quantity,
                        QuantityWisePrice = item.Price * item.Quantity,
                        UserId = request.UserId,
                        CreationDate = DateTime.UtcNow
                    };
                    _context.OrderItems.Add(orderItem);

                    // Update product stock
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.ProductStock -= item.Quantity;
                        if (product.ProductStock < 0) product.ProductStock = 0;
                    }
                }

                // Clear user's cart
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == request.UserId)
                    .ToListAsync();

                _context.Carts.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    OrderId = order.OrderId,
                    OrderNumber = order.OrderNumber,
                    Message = "Order placed successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Order processing failed", error = ex.Message });
            }
        }
        #endregion

        private string GenerateOrderNumber()
        {
            return "ORD" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }
    }

    public class BuyNowRequest
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<BuyNowItem> Items { get; set; }
    }

    public class BuyNowItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}