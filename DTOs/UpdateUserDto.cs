using System.ComponentModel.DataAnnotations;

namespace BeautyStore.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public int id { get; set; }
        public string? name { get; set; } = null;
        public string? email { get; set; } = null;
        public string? password { get; set; } = null;
        public string? phone { get; set; } = null;
        public string? address { get; set; } = null;
    }
}
