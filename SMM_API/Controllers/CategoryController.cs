using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        #region Configuration Fields 
        private readonly SuperMarketManagementContext _context;
        public CategoryController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GET ALL Category
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategory()
        {
            return await _context.Categories
                .ToListAsync();
        }
        #endregion

        #region GET Category BY ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var Category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);
                

            return Category == null ? NotFound() : Ok(Category);
        }
        #endregion

        #region INSERT Category
        [HttpPost]
        public async Task<IActionResult> InsertCategory(Category Category)
        {
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoryById), new { id = Category.CategoryId }, Category);
        }

        #endregion

        #region UPDATE Category
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category Category)
        {
            if (id != Category.CategoryId) return BadRequest();

            _context.Entry(Category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region GET Category BY USER
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Category>> GetCategoryByUserId(int id)
        {
            var Category = await _context.Categories
                .Where(c => c.UserId == id)
                .ToListAsync();

            return Category == null ? NotFound() : Ok(Category);
        }
        #endregion

        #region DropDown Category
        [HttpGet("Dropdown")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetCategory()
        {
            return await _context.Categories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();
        }
        #endregion

        #region DELETE Category
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products) // Load related products
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                    return NotFound($"Category with ID {id} not found.");

                // Delete all products in this category
                if (category.Products.Any())
                {
                    _context.Products.RemoveRange(category.Products);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"Category and its {category.Products.Count} product(s) deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error deleting category: {ex.Message}");
            }
        }
        #endregion

    }
}
