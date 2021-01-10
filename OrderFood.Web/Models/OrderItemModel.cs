namespace OrderFood.Web.Models
{
    public class OrderItemModel
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
