using System.ComponentModel.DataAnnotations;

namespace Sklep2.Models
{
    public class OrderEditViewModel
    {
        public int OrderId { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }
        public List<OrderItemEditViewModel> Items { get; set; } = new List<OrderItemEditViewModel>();
    }

    public class OrderItemEditViewModel
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal Price { get; set; }
    }
}
