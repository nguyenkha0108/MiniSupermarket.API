using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private static readonly List<Category> _categories = new()
        {
            new Category
            {
                CategoryId = 1,
                CategoryName = "Bánh kẹo & Đồ ăn vặt",
                Description = "Snack, bánh quy, kẹo dẻo"
            },

            new Category
            {
                CategoryId = 2,
                CategoryName = "Nước giải khát & Trà",
                Description = "Nước ngọt, nước khoáng, trà"
            },

            new Category
            {
                CategoryId = 3,
                CategoryName = "Sữa & Sản phẩm từ sữa",
                Description = "Sữa tươi, sữa chua, phô mai"
            },

            new Category
            {
                CategoryId = 4,
                CategoryName = "Mì gói & Thực phẩm ăn liền",
                Description = "Mì ăn liền, phở khô, cháo gói"
            },

            new Category
            {
                CategoryId = 5,
                CategoryName = "Gia vị & Dầu ăn",
                Description = "Nước mắm, hạt nêm, dầu thực vật"
            }
        };


        // =====================================================
        // GET ALL
        // Admin + Cashier đều được xem
        // GET: api/categories
        // =====================================================
        [HttpGet]
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult GetAll()
        {
            return Ok(_categories);
        }


        // =====================================================
        // GET BY ID
        // Admin + Cashier đều được xem
        // GET: api/categories/1
        // =====================================================
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult GetById(int id)
        {
            var category = _categories
                .FirstOrDefault(c => c.CategoryId == id);

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
        // Admin + Cashier đều được tìm kiếm
        // GET: api/categories/search?keyword=banh
        // =====================================================
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa!"
                });
            }

            var result = _categories
                .Where(c => c.CategoryName.Contains(
                    keyword,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(result);
        }


        // =====================================================
        // CREATE
        // CHỈ ADMIN
        // POST: api/categories
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] Category newCategory)
        {
            if (string.IsNullOrWhiteSpace(newCategory.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Tên nhóm hàng không được để trống!"
                });
            }

            newCategory.CategoryId =
                _categories.Count > 0
                    ? _categories.Max(c => c.CategoryId) + 1
                    : 1;

            _categories.Add(newCategory);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newCategory.CategoryId },
                newCategory);
        }


        // =====================================================
        // UPDATE
        // CHỈ ADMIN
        // PUT: api/categories/1
        // =====================================================
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(
            int id,
            [FromBody] Category updateCategory)
        {
            var category = _categories
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhóm hàng cần sửa!"
                });
            }

            if (string.IsNullOrWhiteSpace(
                updateCategory.CategoryName))
            {
                return BadRequest(new
                {
                    message = "Tên nhóm hàng không được để trống!"
                });
            }

            category.CategoryName =
                updateCategory.CategoryName;

            category.Description =
                updateCategory.Description;

            return NoContent();
        }


        // =====================================================
        // DELETE
        // CHỈ ADMIN
        // DELETE: api/categories/1
        // =====================================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var category = _categories
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhóm hàng cần xóa!"
                });
            }

            _categories.Remove(category);

            return NoContent();
        }


        // =====================================================
        // ADMIN DASHBOARD
        // CHỈ ADMIN
        // GET: api/categories/admin-dashboard
        // =====================================================
        [HttpGet("admin-dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return Ok(new
            {
                message = "Chào mừng Admin đến với Dashboard!",

                totalCategories = _categories.Count,

                categories = _categories.Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.Description
                })
            });
        }


        // =====================================================
        // STAFF POS
        // ADMIN + CASHIER
        // GET: api/categories/staff-pos
        // =====================================================
        [HttpGet("staff-pos")]
        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult StaffPos()
        {
            return Ok(new
            {
                message = "Màn hình POS Thu ngân sẵn sàng phục vụ bán hàng.",

                username = User.Identity?.Name,

                role = User.FindFirst(
                    System.Security.Claims.ClaimTypes.Role
                )?.Value
            });
        }
    }
}