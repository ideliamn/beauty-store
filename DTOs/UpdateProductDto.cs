using System.ComponentModel.DataAnnotations;

namespace BeautyStore.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        public int id { get; set; }
        public string? brand { get; set; } = null;
        public string? name { get; set; } = null;
        public string? size { get; set; } = null;
        public string? description { get; set; } = null;
        public int? price { get; set; } = null;
        public int? stock { get; set; } = null;
        public int? category_id { get; set; } = null;
    }
}
