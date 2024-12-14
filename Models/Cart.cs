using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Sklep2.Models
{
    public class Cart
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal? TotalPrice { get; set; }
        public bool IsOpen { get; set; }= true;
        public virtual ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
        [MaxLength(50)]
        public string? Name { get; set; }
        public bool IsVisible { get; set; } = false;    

    }
}
