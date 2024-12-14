using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep2.Data;
using Sklep2.Models;

namespace Sklep2.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var carts = await _context.Cart
                .Where(c => c.UserId == userId && c.IsVisible)
                .ToListAsync();

            var openCart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsOpen);

            //tworzenie koszyka jeśli ich nie ma
            if (openCart == null)
            {
                openCart = new Cart
                {
                    Name = "Default cart",
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    IsOpen = true,
                    IsVisible = true,
                    CartItems = new List<CartItem>()
                };

                _context.Cart.Add(openCart);
                await _context.SaveChangesAsync();

                carts = await _context.Cart
                    .Where(c => c.UserId == userId && c.IsVisible)
                    .ToListAsync();
            }

            var products = await _context.Product.ToListAsync();

            return View(new CartListViewModel
            {
                Carts = carts,
                OpenCart = openCart,
                Products = products
            });
        }


        // POST: Carts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Cart name cannot be empty.");
                return RedirectToAction(nameof(Index));
            }

            var newCart = new Cart
            {
                UserId = userId,
                Name = name,
                IsVisible = true
            };

            _context.Cart.Add(newCart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Carts/Switch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Switch(int cartId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentOpenCart = await _context.Cart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsOpen);

            if (currentOpenCart != null)
            {
                currentOpenCart.IsOpen = false;
                _context.Cart.Update(currentOpenCart);
            }

            var newOpenCart = await _context.Cart
                .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);

            if (newOpenCart == null)
            {
                return NotFound();
            }

            newOpenCart.IsOpen = true;
            _context.Cart.Update(newOpenCart);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // POST: Carts/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            var product = await _context.Product.FindAsync(productId);
            if (product == null || product.StockQuantity < quantity)
            {
                return BadRequest("Product is unavailable or not enough in stock.");
            }

            // pobieranie otwartego koszyka
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsOpen);

            if (cart == null)
            {
                return NotFound("No open cart found for the user.");
            }

            // dodawanie lub aktualizacja koszyka
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            _context.Cart.Update(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.IsOpen && c.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier)); 

            if (cart == null)
            {
                return NotFound();
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); 
        }




        // POST: Carts/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int cartId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Cart
                .FirstOrDefaultAsync(c => c.Id == cartId && c.UserId == userId);

            if (cart == null)
            {
                return NotFound();
            }

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // POST: Carts/CreateOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // pobieranie otwartego koszyka
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsOpen);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("No open cart found or the cart is empty");
            }

            // nowe zamowienie
            var order = new Order
            {
                UserId = userId,
                CreatedDate = DateTime.Now,
                DeliveryDate = DateTime.Now.AddDays(7), 
                Status = OrderStatus.Pending,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
            };

            _context.Order.Add(order);

            // zmniejszanie stanu
            foreach (var cartItem in cart.CartItems)
            {
                var product = await _context.Product.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= cartItem.Quantity;
                    _context.Product.Update(product);
                }
            }

            cart.IsOpen = false;
            _context.Cart.Update(cart);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Orders");
        }


    }
}
