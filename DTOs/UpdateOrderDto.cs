namespace BeautyStore.DTOs
{
    public class UpdateOrderDto
    {
        public string key { get; set; }
        public CreateOrderDto order { get; set; }
    }
}