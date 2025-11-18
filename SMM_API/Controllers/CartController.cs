using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;
using System.Security.Claims;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase // Removed [Authorize] temporarily
    {
        private readonly SuperMarketManagementContext _context;

        public CartController(SuperMarketManagementContext context)
        {
            _context = context;
        }

        #region GET USER CART
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartByUserId(int userId)
        {
            try
            {
                var cartItems = await _context.Carts
                    .Include(c => c.Product)  // Include Product
                        .ThenInclude(p => p.ProductImages)  // Include ProductImages
                    .Include(c => c.Product)  // Include Product again for Category
                        .ThenInclude(p => p.Category)  // Include Category
                    .Where(c => c.UserId == userId && !c.IsCheckedOut)
                    .AsNoTracking()
                    .Select(c => new CartResponse  // Use a DTO to avoid circular references
                    {
                        CartId = c.CartId,
                        UserId = c.UserId,
                        ProductId = c.ProductId,
                        ProductQuantity = c.ProductQuantity,
                        AddedDate = c.AddedDate,
                        Product = new ProductResponse
                        {
                            ProductId = c.Product.ProductId,
                            ProductName = c.Product.ProductName,
                            ProductPrice = c.Product.ProductPrice,
                            ProductDescription = c.Product.ProductDescription,
                            ProductImage1 = c.Product.ProductImage1,
                            ProductImage2 = c.Product.ProductImage2,
                            ProductImage3 = c.Product.ProductImage3,
                            Category = c.Product.Category != null ? new CategoryResponse
                            {
                                CategoryId = c.Product.Category.CategoryId,
                                CategoryName = c.Product.Category.CategoryName
                            } : null
                        }
                    })
                    .ToListAsync();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving cart: {ex.Message}");
            }
        }
        #endregion

        #region GET CART COUNT
        [HttpGet("count/{userId}")]
        public async Task<ActionResult<int>> GetCartCount(int userId)
        {
            try
            {
                var count = await _context.Carts
                    .Where(c => c.UserId == userId && !c.IsCheckedOut)
                    .CountAsync();

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving cart count: {ex.Message}");
            }
        }
        #endregion

        #region ADD TO CART
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // Validate product exists
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                {
                    return BadRequest(new { success = false, message = "Product not found" });
                }

                // Validate stock
                if (product.TrackInventory && product.ProductStock < request.Quantity)
                {
                    return BadRequest(new { success = false, message = "Insufficient stock" });
                }

                // Check if item already exists in cart
                var existingItem = await _context.Carts
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId &&
                                            c.ProductId == request.ProductId &&
                                            !c.IsCheckedOut);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.ProductQuantity += request.Quantity;
                    existingItem.ModifyDate = DateTime.UtcNow;
                }
                else
                {
                    // Create new cart item
                    var cartItem = new Cart
                    {
                        UserId = request.UserId,
                        ProductId = request.ProductId,
                        ProductQuantity = request.Quantity,
                        AddedDate = DateTime.UtcNow,
                        CreationDate = DateTime.UtcNow,
                        ModifyDate = DateTime.UtcNow,
                        IsCheckedOut = false
                    };

                    _context.Carts.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                // Get updated cart count
                var cartCount = await _context.Carts
                    .Where(c => c.UserId == request.UserId && !c.IsCheckedOut)
                    .CountAsync();

                return Ok(new
                {
                    success = true,
                    message = "Product added to cart successfully",
                    cartCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error adding to cart: {ex.Message}"
                });
            }
        }
        #endregion

        #region UPDATE CART QUANTITY
        [HttpPatch("{cartId}/quantity")]
        public async Task<IActionResult> UpdateCartQuantity(int cartId, [FromBody] UpdateQuantityRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { success = false, message = "Quantity must be greater than 0" });
                }

                var cart = await _context.Carts
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == request.UserId);

                if (cart == null)
                {
                    return NotFound(new { success = false, message = "Cart item not found" });
                }

                // Check stock if increasing quantity
                if (request.Quantity > cart.ProductQuantity && cart.Product.TrackInventory)
                {
                    var quantityIncrease = request.Quantity - cart.ProductQuantity;
                    if (cart.Product.ProductStock < quantityIncrease)
                    {
                        return BadRequest(new { success = false, message = "Insufficient stock" });
                    }
                }

                cart.ProductQuantity = request.Quantity;
                cart.ModifyDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Quantity updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error updating quantity: {ex.Message}"
                });
            }
        }
        #endregion

        #region REMOVE FROM CART
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId, [FromQuery] int userId)
        {
            try
            {
                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

                if (cart == null)
                {
                    return NotFound(new { success = false, message = "Cart item not found" });
                }

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Item removed from cart" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error removing from cart: {ex.Message}"
                });
            }
        }
        #endregion

        #region CLEAR CART
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            try
            {
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == userId && !c.IsCheckedOut)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Ok(new { success = true, message = "Cart is already empty" });
                }

                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error clearing cart: {ex.Message}"
                });
            }
        }
        #endregion
    }

    #region DTO Classes
    public class AddToCartRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class UpdateQuantityRequest
    {
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }
    #endregion

    public class CartResponse
    {
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public DateTime AddedDate { get; set; }
        public ProductResponse Product { get; set; }
    }

    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage1 { get; set; }
        public string ProductImage2 { get; set; }
        public string ProductImage3 { get; set; }
        public CategoryResponse Category { get; set; }
    }

    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}