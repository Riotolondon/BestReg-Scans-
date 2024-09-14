using BestReg.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                ViewBag.Roles = roles;
            }
        }
        return View();
    }

    // Add the Privacy Action here
    public IActionResult Privacy()
    {
        return View();
    }
}

