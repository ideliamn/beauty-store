using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeautyStore.Models
{
    public class Payment
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = null!;

        [Required, MaxLength(100)]
        [Column("payment_status")]
        public string? PaymentStatus { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public Orders? Order { get; set; }
    }

}
