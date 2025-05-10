using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeautyStore.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("brand")]
        public string Brand { get; set; }

        [Required, MaxLength(150)]
        [Column("name")]
        public string Name { get; set; }

        [MaxLength(100)]
        [Column("size")]
        public string? Size { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Category? Category { get; set; }

        public ICollection<OrderDetail> OrderItems { get; set; } = new List<OrderDetail>();
    }

}
