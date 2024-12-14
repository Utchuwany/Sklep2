namespace Sklep2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
        public string? ImageUrl { get; set; }
    }
}
