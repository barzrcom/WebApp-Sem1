using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Data;
using MapApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MapApp.Controllers
{
	public class ViewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViewsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Views
        public async Task<IActionResult> Index()
        {
            return Json(await _context.View.ToListAsync());
        }

        // GET: Views/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var view = await _context.View
                .SingleOrDefaultAsync(m => m.Id == id);
            if (view == null)
            {
                return NotFound();
            }

            return Json(view);
        }

        // POST: Views/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,LocationId,Date")] View view)
        {
            if (ModelState.IsValid)
            {
                _context.Add(view);
                await _context.SaveChangesAsync();
            }
            return View(view);
        }

		// POST: Views/Delete/5
		[Authorize(Roles = "Administrator")]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var view = await _context.View.SingleOrDefaultAsync(m => m.Id == id);
            _context.View.Remove(view);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ViewExists(int id)
        {
            return _context.View.Any(e => e.Id == id);
        }
    }
}