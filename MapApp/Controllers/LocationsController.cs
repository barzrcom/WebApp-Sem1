using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Models.LocationModels;
using MapApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using MapApp.Models.CommentsModels;

namespace MapApp.Controllers
{
	public class LocationsController : Controller
	{
		private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LocationsController(ApplicationDbContext context, IConfiguration configuration)
		{
			_context = context;
            _configuration = configuration;
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

            // Gets the location's comments
            var comments = await _context.Comment.ToListAsync();
            var result =
                from com in comments
                where com.Location.Equals(id)
                select com;

            ViewBag.Doubled = false;
            if (CommentsController.IsDoubled(comments, id, User.Identity.Name))
                ViewBag.Doubled = true;

            ViewBag.Comments = result;
            ViewBag.User = User.Identity.Name;
            ViewBag.Admin = User.IsInRole("Administrator");

            return View(location);

		}


		[Authorize]
		// GET: Locations/Create
		public IActionResult Create()
		{
			return View();
		}

        // Posting Content to Page on Facebook
        private async Task<string> FacebookPublish(Location location)
        {
            string uri = _configuration.GetValue<string>("Facebook:uri");
            string page = _configuration.GetValue<string>("Facebook:page");
            string accessToken = _configuration.GetValue<string>("Facebook:accessToken");
            string message = location.Name + " " + location.Description;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("message", message),
                        new KeyValuePair<string, string>("access_token", accessToken)
                });
                var result = await client.PostAsync(page, content);
                return await result.Content.ReadAsStringAsync();

            }
        }

		[Authorize]
		// POST: Locations/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,Name,Description,Category,Latitude,Longitude")] Location location, IFormFile Image)
		{
			if (ModelState.IsValid)
			{
				this.UpdateCustomFields(location, Image);
				_context.Add(location);
				await _context.SaveChangesAsync();
                if (Image != null && Image.Length > 0)
                {
                    await Task.Run(() => FacebookPublish(location));
                }
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

			bool isAdmin = User.IsInRole("Administrator");
			if (location.User != User.Identity.Name && !(isAdmin))
			{
				return RedirectToAction("AccessDenied", "Account");
			}

			return View(location);
		}

		[Authorize]
		// POST: Locations/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Category,Latitude,Longitude")] Location location, IFormFile Image)
		{
			if (id != location.ID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					this.UpdateCustomFields(location, Image);
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

			bool isAdmin = User.IsInRole("Administrator");
			if (location.User != User.Identity.Name && !(isAdmin))
			{
				return RedirectToAction("AccessDenied", "Account");
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

            // Delete all the comments along the Location
            var comments = from com in _context.Comment
                          where com.Location.Equals(id)
                          select com;
            foreach(var c in comments)
            {
                _context.Comment.Remove(c);
            }

            await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool LocationExists(int id)
		{
			return _context.Location.Any(e => e.ID == id);
		}

		private async void UpdateCustomFields(Location location, IFormFile Image)
		{
			// TODO: Convert Image file to byte here.
			if (Image != null && Image.Length > 0)
			{
				using (var stream = new MemoryStream())
				{
					await Image.CopyToAsync(stream);
					location.Image = stream.ToArray();
				}
			}

			//location.Image = Convert.ToBase64String(location.Image);
			location.User = User.Identity.Name;
		}

        public IActionResult DoubleComment()
        {
            return View();
        }

    }
}
