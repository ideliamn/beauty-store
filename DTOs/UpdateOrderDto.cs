namespace BeautyStore.DTOs
{
    public class UpdateOrderDto
    {
        public string id { get; set; }
        public CreateOrderTempDto order { get; set; }
    }
}