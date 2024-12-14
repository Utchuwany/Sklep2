using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sklep2.Models;

namespace Sklep2.Controllers
{
    public class AdminController : Controller
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var users = _userManager.Users.ToList();
            var userRoles = new Dictionary<string, List<string>>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }
        [HttpGet]

        public async Task<IActionResult> EditUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles.ToList();
            var currentRoles = await _userManager.GetRolesAsync(user);

            var model = new RoleEditorModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleAssignmentModel
                {
                    RoleName = role.Name,
                    IsSelected = currentRoles.Contains(role.Name)
                }).ToList(),
                SelectedRole = currentRoles.FirstOrDefault() // ustawienie obecnej roli
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> EditUserRoles(RoleEditorModel model)
        {
            if (string.IsNullOrEmpty(model.SelectedRole))
            {
                ModelState.AddModelError("", "You must select a role.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Usuń wszystkie istniejące role
            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to remove existing roles.");
                return View(model);
            }

            // Dodaj wybraną rolę
            var addResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Failed to assign the selected role.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}