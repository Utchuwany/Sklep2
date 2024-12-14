using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sklep2.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? DeliveryDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalPrice { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public enum OrderStatus
    {
        Pending,       
        Processing,    
        Shipped,       
        Delivered,
        Cancelled
    }
}
