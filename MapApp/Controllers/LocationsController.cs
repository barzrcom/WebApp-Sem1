using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MapApp.Models;
using MapApp.Models.LocationModels;
using MapApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace MapApp.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;    
        }

		// GET: Locations
		public async Task<IActionResult> Index()
        {			
            return View(await _context.Location.ToListAsync());
        }

        // GET: Locations
        public async Task<IActionResult> Data()
        {
            return Json(await _context.Location.ToListAsync());
        }

        [Authorize]
		// GET: MyLocationsIndex
		public async Task<IActionResult> MyLocationsIndex()
		{
			return View((await _context.Location.ToListAsync()).Where(s => s.User == User.Identity.Name));
		}

		[Authorize]
		// GET: Locations/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

		[Authorize]
		// GET: Locations/Create
		public IActionResult Create()
        {
            return View();
        }

		[Authorize]
		// POST: Locations/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description,Type,Latitude,Longitude,District,Seasons,OpenTime,CloseTime,LastVisit,Duration")] Location location)
        {
            if (ModelState.IsValid)
            {
				location.User = User.Identity.Name;
				_context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(location);
        }

		[Authorize]
		// GET: Locations/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

		[Authorize]
		// POST: Locations/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Type,Latitude,Longitude,District,Seasons,OpenTime,CloseTime,LastVisit,Duration")] Location location)
        {
            if (id != location.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
					location.User = User.Identity.Name;
					_context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(location);
        }

		[Authorize]
		// GET: Locations/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .SingleOrDefaultAsync(m => m.ID == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

		[Authorize]
		// POST: Locations/Delete/5
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Location.SingleOrDefaultAsync(m => m.ID == id);
            _context.Location.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.ID == id);
        }
    }
}