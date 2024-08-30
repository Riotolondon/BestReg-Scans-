using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestReg.Controllers
{
    [Authorize(Roles = "Student")]
    public class ParentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewQrCode()
        {
            // Implementation to show QR code
            return View();
        }

        public IActionResult ViewChildData()
        {
            // Implementation to show child's past data
            return View();
        }
    }
}
