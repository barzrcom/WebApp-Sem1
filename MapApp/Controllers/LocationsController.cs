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
		public async Task<IActionResult> Index(string name, string user, string description, string category)
		{
		    var locations = await _context.Location.ToListAsync();

		    if (!String.IsNullOrEmpty(name)) locations = locations.Where(s => s.Name.Contains(name)).ToList();
		    if (!String.IsNullOrEmpty(user)) locations = locations.Where(s => s.User.Contains(user)).ToList();
		    if (!String.IsNullOrEmpty(description)) locations = locations.Where(s => s.Description.Contains(description)).ToList();
		    LocationCategory lc;
            if (!String.IsNullOrEmpty(category) && Enum.TryParse(category, true, out lc)) locations = locations.Where(s => s.Category.Equals(lc)).ToList();

            return View(locations);
		}

        // GET: Locations
        public async Task<IActionResult> Data()
		{
			return Json(await _context.Location.ToListAsync());
		}

		[Authorize]
		// GET: Locations/Details/5
		public async Task<IActionResult> Details(int? id, string header, string content, string user)
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
            var comments = await _context.Comment.Where(c => c.Location == id).ToListAsync();

            if (!String.IsNullOrEmpty(header)) comments = comments.Where(s => s.Header.Contains(header)).ToList();
		    if (!String.IsNullOrEmpty(content)) comments = comments.Where(s => s.Content.Contains(content)).ToList();
		    if (!String.IsNullOrEmpty(user)) comments = comments.Where(s => s.User.Contains(user)).ToList();


            ViewBag.Doubled = false;
            if (CommentsController.IsDoubled(comments, id, User.Identity.Name))
                ViewBag.Doubled = true;

            ViewBag.Comments = comments;
            ViewBag.User = User.Identity.Name;
            ViewBag.Admin = User.IsInRole("Administrator");

            // Updating rating value base on comments average
            UpdateRating(comments, location);

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
		public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description,Category,Latitude,Longitude,Rating")] Location location, IFormFile Image)
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

        // Updating rating value base on comments average
        public void UpdateRating(IEnumerable<Comment> comments, Location location)
        {
            var result =
                from com in comments
                where com.Location.Equals(location.ID)
                select com.Rating;

            float rate_avg;
            if (result.Any())
            {
                rate_avg = (float)result.Sum() / result.Count();
                // Round the result to 0.5 jumps
                if ((rate_avg * 10 % 10) < 3)
                    rate_avg = (int)rate_avg;
                else if ((rate_avg * 10 % 10) > 7)
                    rate_avg = (int)rate_avg + 1;
                else
                    rate_avg = (int)rate_avg + (float)0.5;
            }
            else
                rate_avg = 0;


            location.Rating = rate_avg;
            _context.SaveChanges();
        }

    }
}
