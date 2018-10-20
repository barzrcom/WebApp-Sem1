using Microsoft.AspNetCore.Mvc;
using MapApp.Data;

namespace MapApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
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
