
using System.ComponentModel.DataAnnotations;

namespace MiniSupermarket.API.Models
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Tên nhóm hàng không được để trống!")]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }
}