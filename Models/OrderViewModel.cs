using System.ComponentModel.DataAnnotations;

namespace Sklep2.Models
{
    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
    }

}
