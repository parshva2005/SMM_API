using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Helper;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public ProductController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL Product
        // Endpoint for customers - only active products
        [HttpGet("Customer")]
        public async Task<IActionResult> GetProductsForCustomer()
        {
            var activeProducts = await _context.Products
                .Where(p => !p.IsRemoved.Value) // Filter out deleted products
                .ToListAsync();
            return Ok(activeProducts);
        }

        // Endpoint for admin - all products including deleted
        [HttpGet("Admin")]
        public async Task<IActionResult> GetProductsForAdmin()
        {
            var allProducts = await _context.Products.ToListAsync();
            return Ok(allProducts);
        }
        #endregion

        #region GET Product BY ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var Product = await _context.Products
                .Include(ca => ca.Category)
                .Include(pi => pi.ProductImages)
                .FirstOrDefaultAsync(c => c.ProductId == id);

            return Product == null ? NotFound() : Ok(Product);
        }
        #endregion

        #region inventory management
        [HttpGet("{id}/stock")]
        public async Task<ActionResult<int>> GetProductStock(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // Calculate current stock from logs
            var stock = await _context.ProductLogs
                .Where(pl => pl.ProductId == id)
                .SumAsync(pl => pl.QuantityChange);

            return Ok(stock);
        }
        #endregion

        #region GET Top 10 Product
        [HttpGet("Top10")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetTop10Product()
        {
            var Product = await _context.Products.OrderBy(p => p.ProductStock).Take(10).ToListAsync();

            return Product == null ? NotFound() : Ok(Product);
        }
        #endregion

        #region Get Product By Category Id
        [HttpGet("Category/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetProductByCategoryId(int id)
        {
            var products = await _context.Products
                .Include(ca => ca.Category)
                .Where(c => c.CategoryId == id)
                .ToListAsync();

            return products == null || !products.Any() ? NotFound() : Ok(products);
        }
        #endregion

        #region DELETE Product
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return NoContent();
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"Error deleting product with ID {id}: {ex.Message}");
            }
            
        }
        #endregion

        #region INSERT Product
        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            product.CreationDate = DateTime.Now;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }
        #endregion

        /*#region Adjust Inventory
        [HttpPost("{id}/stock/adjust")]
        public async Task<IActionResult> AdjustInventory(int id, [FromBody] InventoryAdjustment adjustment)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var log = new ProductLog
            {
                ProductId = id,
                UserId = adjustment.UserId,
                QuantityChange = adjustment.Quantity,
                LogType = adjustment.Quantity > 0 ? "StockIn" : "StockOut",
                Reference = adjustment.Reference,
                Notes = adjustment.Notes,
                LogDate = DateTime.UtcNow
            };

            _context.ProductLogs.Add(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion*/

        #region UPDATE Product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] Product product)
        {
            if (id != product.ProductId) return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DropDown Product
        [HttpGet("Dropdown")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetProduct()
    {
        return await _context.Products
            .Select(c => new { c.ProductId, c.ProductName })
            .ToListAsync();
    }
        #endregion

        #region Remove Product
        [HttpPatch("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleProductStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.IsRemoved = !product.IsRemoved; // Toggle status
            await _context.SaveChangesAsync();

            return Ok();
        }
        #endregion

        #region Search and Filter Products
        [HttpGet("Search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string searchTerm = "",
            [FromQuery] int? categoryId = null,
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] string priceRange = "")
        {
            try
            {
                IQueryable<Product> query = _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsRemoved != true); // Handle nullable boolean

                // Search term filter
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(p =>
                        p.ProductName.ToLower().Contains(searchTerm) ||
                        (p.ProductDescription != null && p.ProductDescription.ToLower().Contains(searchTerm)) ||
                        (p.Sku != null && p.Sku.ToLower().Contains(searchTerm)));
                }

                // Category filter
                if (categoryId.HasValue && categoryId > 0)
                {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                }

                // Price range filter
                if (!string.IsNullOrEmpty(priceRange))
                {
                    switch (priceRange)
                    {
                        case "0-50":
                            query = query.Where(p => p.ProductPrice <= 50);
                            break;
                        case "50-100":
                            query = query.Where(p => p.ProductPrice > 50 && p.ProductPrice <= 100);
                            break;
                        case "100-500":
                            query = query.Where(p => p.ProductPrice > 100 && p.ProductPrice <= 500);
                            break;
                        case "500-1000":
                            query = query.Where(p => p.ProductPrice > 500 && p.ProductPrice <= 1000);
                            break;
                        case "1000+":
                            query = query.Where(p => p.ProductPrice > 1000);
                            break;
                    }
                }

                // Sorting
                query = sortBy.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(p => p.ProductName),
                    "price_asc" => query.OrderBy(p => p.ProductPrice),
                    "price_desc" => query.OrderByDescending(p => p.ProductPrice),
                    "newest" => query.OrderByDescending(p => p.CreationDate),
                    "oldest" => query.OrderBy(p => p.CreationDate),
                    _ => query.OrderBy(p => p.ProductName), // Default: name_asc
                };

                var products = await query.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                // Log the actual error for debugging
                Console.WriteLine($"Search error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    message = "Error searching products",
                    error = ex.Message
                });
            }
        }
        #endregion
    }
}
