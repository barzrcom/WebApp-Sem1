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

        // Posting Content to Page on Facebook
        private async Task<string> FacebookPublish(Location location)
        {
            string uri = "https://graph.facebook.com";
            string page = "/264600174386513/feed";
            string message = location.Name + " " + location.Description;
            string accessToken = "EAADz7RiCz5kBAMv0966BKS7AXtWdMwqEztUjZAWQjwBSDm98p2JCPx9J3Vh8Y5wEUIVySxIevZAJOk39VxqYgVKkWYrQIxjI8uVE9r0WZCfNj5eRzKJD0i8mB44WMBZAJ5nnGMsOz1sHE6htlkutAiT5pKBggZAz9BaTxooeCmQZDZD";
            
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
	}
}
