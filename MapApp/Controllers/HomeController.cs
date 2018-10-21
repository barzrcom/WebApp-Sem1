using Microsoft.AspNetCore.Mvc;

namespace MapApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "A Story of Success";

            return View();
        }

        public IActionResult Statistics()
        {
            ViewData["Message"] = "Your Statistics page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
