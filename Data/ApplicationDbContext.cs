using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sklep2.Models;

namespace Sklep2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Sklep2.Models.Product> Product { get; set; } = default!;
        public DbSet<Sklep2.Models.Cart> Cart { get; set; } = default!;
        public DbSet<Sklep2.Models.CartItem> CartItem { get; set; } = default!;
        public DbSet<Sklep2.Models.Order> Order { get; set; } = default!;
        public DbSet<Sklep2.Models.OrderItem> OrderItem { get; set; } = default!;
    }
}
