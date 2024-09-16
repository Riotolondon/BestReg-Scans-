using BestReg.Data;
using BestReg.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BestReg.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

     

        public async Task<IActionResult> Index()
        {
            var model = new AdminUserViewModel
            {
                Users = _userManager.Users.ToList(),
                UserManager = _userManager
            };
            return View(model);
        }

        public IActionResult CreateUser()
        {
            ViewData["Roles"] = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await SeedData.EnsureRoles(_roleManager, new string[] { model.Role }); // Ensure the role exists

                    var roleAssignResult = await _userManager.AddToRoleAsync(user, model.Role);
                    if (roleAssignResult.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Failed to assign role: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> SyncUsers([FromServices] UserManager<ApplicationUser> userManager)
        {
            await DbInitializer.SyncExistingUsersToFirebase(userManager);
            return RedirectToAction("Index", "Admin");
        }
    }
}
