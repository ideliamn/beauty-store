namespace BeautyStore.DTOs
{
    public class OrderResponseDto
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public decimal total_amount { get; set; }
        public List<OrderDetailDto> order_detail { get; set; }
    }
}