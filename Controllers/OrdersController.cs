using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep2.Data;
using Sklep2.Models;
using System.Security.Claims;

namespace Sklep2.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSellerOrAdmin = User.IsInRole("Seller") || User.IsInRole("Admin");

            // pobierz zamówienia 
            IQueryable<Order> ordersQuery = _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product);

            if (!isSellerOrAdmin)
            {
                ordersQuery = ordersQuery.Where(o => o.UserId == userId);
            }

            var orders = await ordersQuery.ToListAsync();

            // pobieranie unikalnych id
            var userIds = orders.Select(o => o.UserId).Distinct();

            // pobieranie id userow
            var users = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email); 

            ViewBag.UserEmails = users;


            var viewModel = new OrderListViewModel
            {
                Orders = orders,
            };

            return View(viewModel);
        }




        // GET: Orders/Edit/5
        [Authorize(Roles ="Seller, Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // pobranie listy produktów do wyboru w formularzu
            var products = await _context.Product.ToListAsync();

            var viewModel = new OrderEditViewModel
            {
                OrderId = order.Id,
                Status = order.Status,
                DeliveryDate = (DateTime)order.DeliveryDate,
                Items = order.OrderItems.Select(oi => new OrderItemEditViewModel
                {
                    OrderItemId = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            // Dodanie listy produktów do ViewBag
            ViewBag.Products = products;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = model.Status;
            order.DeliveryDate = model.DeliveryDate;


            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }




        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSellerOrAdmin = User.IsInRole("Seller") || User.IsInRole("Admin");

            var order = await _context.Order
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // kontrola roli
            if (!isSellerOrAdmin && order.UserId != userId)
            {
                return Forbid(); 
            }

            var viewModel = new OrderEditViewModel
            {
                OrderId = order.Id,
                Status = order.Status,
                DeliveryDate = order.DeliveryDate ?? DateTime.MinValue,
                Items = order.OrderItems.Select(oi => new OrderItemEditViewModel
                {
                    OrderItemId = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Product.Price
                }).ToList()
            };

            return View(viewModel);
        }


    }
}
