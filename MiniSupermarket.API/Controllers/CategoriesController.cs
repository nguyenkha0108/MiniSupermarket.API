
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using System.Security.Claims;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public CategoriesController(SupermarketDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL
        // GET: api/categories
        // =====================================================
        [HttpGet]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryId)
                .ToListAsync();

            return Ok(categories);
        }

        // =====================================================
        // GET BY ID
        // GET: api/categories/1
        // =====================================================
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhóm hàng!"
                });
            }

            return Ok(category);
        }

        // =====================================================
        // SEARCH
        // GET: api/categories/search?keyword=rau
        // =====================================================
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa!"
                });
            }

            keyword = keyword.Trim();

            var result = await _context.Categories
                .AsNoTracking()
                .Where(c => c.CategoryName.Contains(keyword))
                .OrderBy(c => c.CategoryId)
                .ToListAsync();

            return Ok(result);
        }

        // =====================================================
        // CREATE
        // POST: api/categories
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create(
            [FromBody] CreateCategoryRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Tên nhóm hàng không được để trống!"
                });
            }

            string categoryName = request.CategoryName.Trim();

            // Không cho thêm tên danh mục đã tồn tại
            bool exists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryName.ToLower() == categoryName.ToLower());

            if (exists)
            {
                return Conflict(new
                {
                    message = "Danh mục này đã tồn tại, vui lòng chọn tên khác!"
                });
            }

            var newCategory = new Category
            {
                CategoryName = categoryName,
                Description = request.Description?.Trim()
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = newCategory.CategoryId },
                newCategory);
        }

        // =====================================================
        // UPDATE
        // PUT: api/categories/1
        // =====================================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Category updateCategory)
        {
            if (updateCategory == null)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu danh mục không hợp lệ!"
                });
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhóm hàng cần sửa!"
                });
            }

            if (string.IsNullOrWhiteSpace(updateCategory.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Tên nhóm hàng không được để trống!"
                });
            }

            string categoryName = updateCategory.CategoryName.Trim();

            // Không cho sửa thành tên của danh mục khác
            bool exists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryId != id &&
                    c.CategoryName.ToLower() == categoryName.ToLower());

            if (exists)
            {
                return Conflict(new
                {
                    message = "Tên danh mục này đã được sử dụng!"
                });
            }

            category.CategoryName = categoryName;
            category.Description = updateCategory.Description?.Trim();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // DELETE
        // DELETE: api/categories/1
        // =====================================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhóm hàng cần xóa!"
                });
            }

            // Kiểm tra danh mục có sản phẩm hay không
            bool hasProducts = await _context.Products
                .AnyAsync(p => p.CategoryId == id);

            if (hasProducts)
            {
                return Conflict(new
                {
                    message = "Danh mục đang có sản phẩm, vui lòng chuyển sản phẩm sang danh mục khác trước khi xóa!"
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // =====================================================
        // ADMIN DASHBOARD
        // GET: api/categories/admin-dashboard
        // =====================================================
        [HttpGet("admin-dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var totalCategories = await _context.Categories.CountAsync();

            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description
                })
                .OrderBy(c => c.CategoryId)
                .ToListAsync();

            return Ok(new
            {
                message = "Chào mừng Admin đến với Dashboard!",
                totalCategories,
                categories
            });
        }

        // =====================================================
        // STAFF POS
        // GET: api/categories/staff-pos
        // =====================================================
        [HttpGet("staff-pos")]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public IActionResult StaffPos()
        {
            return Ok(new
            {
                message = "Màn hình POS Thu ngân sẵn sàng phục vụ bán hàng.",
                username = User.Identity?.Name,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
