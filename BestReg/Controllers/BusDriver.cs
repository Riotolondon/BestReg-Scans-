using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestReg.Controllers
{
    [Authorize(Roles = "BusDriver")]
    public class BusDriverController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ScanOn()
        {
            // Logic for scanning learners onto the bus
            return View();
        }

        public IActionResult ScanOff()
        {
            // Logic for scanning learners off the bus
            return View();
        }
    }
}
