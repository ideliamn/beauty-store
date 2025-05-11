namespace BeautyStore.DTOs
{
    public class CreateOrderTempDto
    {
        public int user_id { get; set; }
        public List<OrderDetailTempDto> order_detail { get; set; }
    }
}