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
using MapApp.Models.ViewModels;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using PagedList.Core;

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
		public async Task<IActionResult> Index(string name, string user, string description, string category, int page=1)
		{
            var locations = await _context.Location.ToListAsync();

            if (!String.IsNullOrEmpty(name)) locations = locations.Where(s => s.Name.Contains(name)).ToList();
		    if (!String.IsNullOrEmpty(user)) locations = locations.Where(s => s.User.Contains(user)).ToList();
		    if (!String.IsNullOrEmpty(description)) locations = locations.Where(s => s.Description.Contains(description)).ToList();
		    LocationCategory lc;
            if (!String.IsNullOrEmpty(category) && Enum.TryParse(category, true, out lc)) locations = locations.Where(s => s.Category.Equals(lc)).ToList();

            int pageSize = _configuration.GetValue<int>("Paging:Locations");
            PagedList<Location> model = new PagedList<Location>(locations.ToList().AsQueryable(), page , pageSize);

            return View("Index", model);
        }

        // GET: Locations
        public async Task<IActionResult> Data()
		{
			return Json(await _context.Location.ToListAsync());
		}

        // GET: Categories
        public async Task<IActionResult> Categories()
        {
                var categories = await _context.Location.GroupBy(l => l.Category)
                    .ToDictionaryAsync(g => Enum.GetName(typeof(LocationCategory), g.Key), g => g.Count());

                return Json(categories);
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

            View view_data = new View()
            {
                UserId = User.Identity.Name,
                LocationId = location.ID,
                Date = DateTime.Now
            };
            await new ViewsController(_context).Create(view_data);
            // Increase "View" counter

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
				this.UpdateImage(location, Image);

				// Update user
				location.User = User.Identity.Name;

				// Update CreateDate
				location.CreatedDate = DateTime.Now;

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
			// Don't update CreatedDate and User on Edit
			var excluded = new[] { "CreatedDate", "User" };

			if (id != location.ID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					this.UpdateImage(location, Image);
					var entry = _context.Entry(location);
					entry.State = EntityState.Modified;
					foreach (var name in excluded)
					{
						entry.Property(name).IsModified = false;
					}

					// Don't update Image if not provided from the user
					if (Image == null) entry.Property(m => m.Image).IsModified = false;

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

		[Authorize]
		public async Task<IActionResult> DeleteImage(int id)
		{
			var location = await _context.Location.SingleOrDefaultAsync(m => m.ID == id);
			location.Image = null;

			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		private bool LocationExists(int id)
		{
			return _context.Location.Any(e => e.ID == id);
		}

		private async void UpdateImage(Location location, IFormFile Image)
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
        public async Task<IActionResult> Recommend()
        {
            var comments = await _context.Comment.ToListAsync();
            var locations = await _context.Location.ToListAsync();

            //Get all locations for training set (locations with a comment of current user)
            var resultTrain =
                from c in comments
                join l in locations on c.Location equals l.ID
                where c.User.Equals(User.Identity.Name)
                select new { l.Category, l_rating = l.Rating, c_rating = c.Rating };

            //Create a list of {Category, Location Rating} for ML input
            List<double[]> trainSetInput = new List<double[]>();
            resultTrain.ToList().ForEach(r => trainSetInput.Add(new double[] { r.Category.GetHashCode(), r.l_rating }));

            //Create a list of Comment rating for ML output - if user's location rate > 3 then positive for this instance
            List<int> trainSetOutput = new List<int>();
            resultTrain.ToList().ForEach(r => trainSetOutput.Add((r.c_rating > 3) ? 1 : 0));

            //Get all locations for test set (locations that published by another user and have some comments)
            var resultTest =
                from l in locations
                where !l.User.Equals(User.Identity.Name) && !l.Rating.Equals(0)
                select new { l.Category, l.Rating, l.ID };

            //Create a list of {Category, Location Rating} for test set
            List<double[]> testSet = new List<double[]>();
            resultTest.ToList().ForEach(r => testSet.Add(new double[] { r.Category.GetHashCode(), r.Rating }));

            //Create a list of all potential recommend locations IDs
            List<int> locationsID = new List<int>();
            resultTest.ToList().ForEach(r => locationsID.Add(r.ID));

            List<Location> locationRecommends = new List<Location>();

            int TrainSetMinimumSize = _configuration.GetValue<int>("MachineLearning:TrainSetMinimumSize");
            ViewData["TrainSetMinimumSize"] = TrainSetMinimumSize;
            ViewData["TrainSetCountValidation"] = false;

            //require a valid training set
            if (trainSetOutput.Count() >= TrainSetMinimumSize)
            {
                ViewData["TrainSetCountValidation"] = true;
                bool[] answers = ML_SVM(trainSetInput, trainSetOutput, testSet);

                //Build a recommends location based on SVM result
                for (var i = 0; i < answers.Count(); i++)
                {
                    //check if the classification is ture && filter all the locations the user already visited (leave a comment)
                    if (answers[i] && (comments.Where(c => c.Location == locationsID[i] && c.User.Equals(User.Identity.Name)).Count() == 0))
                    {
                        locationRecommends.Add(locations.Where(s => s.ID == locationsID[i]).SingleOrDefault());
                    }
                }
            }

            return View(locationRecommends);
        }

        // ML Using SVM
        private bool[] ML_SVM(List<double[]> trainSetInput, List<int> trainSetOutput, List<double[]> testSet)
        {
            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true // estimate the kernel from the data
            };

            // Teach the vector machine
            var svm = teacher.Learn(trainSetInput.ToArray(), trainSetOutput.ToArray());

            // Classify the samples using the model
            return svm.Decide(testSet.ToArray());
        }

    }
}
