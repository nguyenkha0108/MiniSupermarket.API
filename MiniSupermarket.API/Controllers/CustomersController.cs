
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/customers/1
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            return Ok(customer);
        }

        // GET: api/customers/search?keyword=nguyen
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa!" });

            var customers = await _context.Customers
                .AsNoTracking()
                .Where(c => c.CustomerName.Contains(keyword)
                         || c.PhoneNumber.Contains(keyword))
                .ToListAsync();

            return Ok(customers);
        }

        // POST: api/customers
        [HttpPost]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Create([FromBody] Customer request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.CustomerId != 0)
                return BadRequest(new { message = "Không nhập CustomerId khi thêm mới!" });

            if (request.RewardPoints < 0)
                return BadRequest(new { message = "Điểm tích lũy không được âm!" });

            var phoneExists = await _context.Customers
                .AnyAsync(c => c.PhoneNumber == request.PhoneNumber);

            if (phoneExists)
                return BadRequest(new { message = "Số điện thoại đã tồn tại!" });

            _context.Customers.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = request.CustomerId },
                request);
        }

        // PUT: api/customers/1
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Update(int id, [FromBody] Customer request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            if (request.RewardPoints < 0)
                return BadRequest(new { message = "Điểm tích lũy không được âm!" });

            var phoneExists = await _context.Customers
                .AnyAsync(c => c.PhoneNumber == request.PhoneNumber
                            && c.CustomerId != id);

            if (phoneExists)
                return BadRequest(new { message = "Số điện thoại đã thuộc khách hàng khác!" });

            customer.CustomerName = request.CustomerName;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;
            customer.RewardPoints = request.RewardPoints;
            customer.MembershipRank = request.MembershipRank;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/customers/1
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng!" });

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}