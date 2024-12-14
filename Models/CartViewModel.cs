namespace Sklep2.Models
{
    public class CartListViewModel
    {
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public Cart? OpenCart { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
