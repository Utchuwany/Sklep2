using System.ComponentModel.DataAnnotations;

namespace Sklep2.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage ="Select at least 1 item")]
        public int Quantity { get; set; } = 1;
        public virtual Product Product { get; set; } = null!;
        public virtual Cart Cart { get; set; } = null!;
    }
}
