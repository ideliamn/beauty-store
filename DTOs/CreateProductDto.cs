namespace BeautyStore.DTOs
{
    public class CreateProductDto
    {
        public string brand { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string description { get; set; }
        public int price { get; set; }
        public int stock { get; set; }
        public int category_id { get; set; }
    }
}
