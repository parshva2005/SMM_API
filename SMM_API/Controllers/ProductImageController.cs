// Controllers/ProductImageController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SUPER_MARKET_MANAGEMENT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly SuperMarketManagementContext _context;

        public ProductImageController(SuperMarketManagementContext context)
        {
            _context = context;
        }

        #region GET Product Images by Product ID
        [HttpGet("Product/{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductImage>>> GetProductImagesByProductId(int productId)
        {
            var productImages = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            return productImages == null || !productImages.Any() ? NotFound() : Ok(productImages);
        }
        #endregion

        #region CREATE Product Image
        [HttpPost]
        public async Task<ActionResult<ProductImage>> CreateProductImage([FromBody] ProductImage productImage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Clear navigation property to avoid circular reference issues
            productImage.Product = null;

            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductImage), new { id = productImage.ProductImageId }, productImage);
        }
        #endregion

        #region GET Product Image by ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductImage>> GetProductImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            return productImage == null ? NotFound() : Ok(productImage);
        }
        #endregion

        #region DELETE Product Image
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null) return NotFound();

            // Delete the physical file if it's a local file
            if (!productImage.ImageUrl.StartsWith("http"))
            {
                var imagePath = Path.Combine("wwwroot", productImage.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        #region SET Primary Image
        [HttpPatch("{id}/SetPrimary")]
        public async Task<IActionResult> SetPrimaryImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null) return NotFound();

            // Reset all images for this product to non-primary
            var allProductImages = await _context.ProductImages
                .Where(pi => pi.ProductId == productImage.ProductId)
                .ToListAsync();

            foreach (var image in allProductImages)
            {
                image.IsPrimary = false;
            }

            // Set the selected image as primary
            productImage.IsPrimary = true;

            await _context.SaveChangesAsync();

            return Ok();
        }
        #endregion
    }
}