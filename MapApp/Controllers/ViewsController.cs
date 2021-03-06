using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Data;
using MapApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System;

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

        // GET: Categories
        public async Task<IActionResult> ByLocation(int? limitResults)
        {
            //var query = from l in _context.Location
            //            join v in _context.View on l.ID equals v.LocationId into j1
            //            orderby j1.Count() descending
            //            select new { Id = l.ID, Count = j1.Count(), Name = l.Name };

            var query = _context.Location.Join(
                _context.View, l => l.ID, v => v.LocationId,
                (location, view) => new { id = location.ID, name = location.Name })
                .GroupBy(l => l.id, l => l.name)
                .Select(n => new { Id = n.Key, Count = n.Count(), Name=n })
                .OrderByDescending(l => l.Count);

            if (null != limitResults)
            { 
                var results = await query.Take(limitResults.GetValueOrDefault())
                .ToDictionaryAsync(l => l.Id, l => new { l.Count, Name=l.Name.First() });
                return Json(results);
            }
            else
            {
                var results = await query
               .ToDictionaryAsync(l => l.Id, l => new { l.Count, Name = l.Name.First() });
                return Json(results);
            } 
        }

        public async Task<IActionResult> ByDate()
        {
            var query = await _context.View.GroupBy(l => new DateTime(l.Date.Year, l.Date.Month, l.Date.Day))
                .OrderBy(g => g.Key)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            return Json(query);
        }

        // GET: Views/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var view = await _context.View
                .SingleOrDefaultAsync(m => m.ID == id);
            if (view == null)
            {
                return NotFound();
            }

            return Json(view);
        }

        // POST: Views/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ID,UserId,LocationId,Date")] View view)
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
            var view = await _context.View.SingleOrDefaultAsync(m => m.ID == id);
            _context.View.Remove(view);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ViewExists(int id)
        {
            return _context.View.Any(e => e.ID == id);
        }
	}
}
