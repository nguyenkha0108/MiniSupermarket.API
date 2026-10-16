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
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // GET: api/products?categoryId=1
        // Lấy tất cả sản phẩm, có thể lọc theo CategoryId
        [HttpGet]
        [Authorize(Roles = "Admin,Cashier,Warehouse")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? categoryId)
        {
            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == categoryId.Value);

                if (!categoryExists)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy danh mục!"
                    });
                }

                query = query.Where(
                    p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();

            return Ok(products);
        }

        // GET: api/products/1
        // Lấy sản phẩm theo ProductId
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm!"
                });
            }

            return Ok(product);
        }

        // GET: api/products/search?keyword=ca&categoryId=1
        // Tìm kiếm sản phẩm, có thể kết hợp lọc danh mục
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Search(
            [FromQuery] string keyword,
            [FromQuery] int? categoryId)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa!"
                });
            }

            var query = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p =>
                    p.ProductName.Contains(keyword) ||
                    p.Barcode.Contains(keyword))
                .AsQueryable();

            if (categoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.CategoryId == categoryId.Value);

                if (!categoryExists)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy danh mục!"
                    });
                }

                query = query.Where(
                    p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();

            return Ok(products);
        }

        // GET: api/products/category/1
        // Lấy sản phẩm theo CategoryId
        [HttpGet("category/{categoryId:int}")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == categoryId);

            if (!categoryExists)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy danh mục!"
                });
            }

            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return Ok(products);
        }

        // POST: api/products
        // Thêm sản phẩm vào danh mục
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (product.ProductId != 0)
            {
                return BadRequest(new
                {
                    message = "Không nhập ProductId khi thêm mới!"
                });
            }

            if (product.Price < 0 || product.StockQuantity < 0)
            {
                return BadRequest(new
                {
                    message = "Giá và số lượng không được âm!"
                });
            }

            var barcodeExists = await _context.Products
                .AnyAsync(p => p.Barcode == product.Barcode);

            if (barcodeExists)
            {
                return BadRequest(new
                {
                    message = "Mã vạch đã tồn tại!"
                });
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == product.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "CategoryId không hợp lệ!"
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.ProductId },
                product);
        }

        // PUT: api/products/1
        // Sửa thông tin sản phẩm, bao gồm CategoryId
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Product request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm!"
                });
            }

            if (request.Price < 0 || request.StockQuantity < 0)
            {
                return BadRequest(new
                {
                    message = "Giá và số lượng không được âm!"
                });
            }

            var barcodeExists = await _context.Products
                .AnyAsync(p =>
                    p.Barcode == request.Barcode &&
                    p.ProductId != id);

            if (barcodeExists)
            {
                return BadRequest(new
                {
                    message = "Mã vạch đã thuộc sản phẩm khác!"
                });
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == request.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "CategoryId không hợp lệ!"
                });
            }

            product.Barcode = request.Barcode;
            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy sản phẩm!"
                });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

