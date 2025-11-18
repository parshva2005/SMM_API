using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMM_API.Models;

namespace SMM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region Configuration
        private readonly SuperMarketManagementContext _context;

        public CustomerController(SuperMarketManagementContext context)
        {
            _context = context;
        }
        #endregion

        #region GetAllCustomer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            return await _context.Customers.ToListAsync();
        }
        #endregion

        #region INSERT Customer
        [HttpPost]
        public async Task<IActionResult> InsertCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
        }
        #endregion

        #region UPDATE Customer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId) return BadRequest();

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region DELETE Customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        #endregion

        #region Get Customer By ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            return customer == null ? NotFound() : Ok(customer);
        }
        #endregion

        #region Get Customer By User ID
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Customer>> GetByUserId(int userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            return customer == null ? NotFound() : Ok(customer);
        }
        #endregion

        #region Get Orders by Customer id
        [HttpGet("{id}/orders")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int id)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
        }
        #endregion

        #region DropDown Customer
        [HttpGet("Dropdown")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomer()
        {
            return await _context.Customers
                .Select(u => new { u.CustomerId, u.Name })
                .ToListAsync();
        }
        #endregion
    }
}