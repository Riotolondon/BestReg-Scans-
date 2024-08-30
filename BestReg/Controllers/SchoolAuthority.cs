using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestReg.Controllers
{
    [Authorize(Roles = "SchoolSecurity")]
    public class SchoolAuthorityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ScanIn()
        {
            // Logic for scanning learners in
            return View();
        }

        public IActionResult ScanOut()
        {
            // Logic for scanning learners out
            return View();
        }
    }
}
