namespace BeautyStore.DTOs
{
    public class CreateOrderDto
    {
        public int user_id { get; set; }
        public List<OrderDetailDto> order_detail { get; set; }
    }
}