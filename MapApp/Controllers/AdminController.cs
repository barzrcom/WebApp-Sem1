using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Models.LocationModels;
using MapApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using MapApp.Models.CommentsModels;
using System;

namespace MapApp.Controllers
{
	[Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            
            return View(await _context.Users.ToListAsync());
        }

        // GET: Admin/Locations
        public async Task<IActionResult> Locations()
        {

            return View(await _context.Location.ToListAsync());
        }

        // GET: Admin/Comments
        public async Task<IActionResult> Comments()
        {
            return View(await _context.Comment.ToListAsync());
        }

        [Authorize]
        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Users");
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

        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.ID == id);
        }

        [Authorize]
        // GET: Admin/EditLocation/5
        public async Task<IActionResult> EditLocation(int? id)
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
        // POST: Admin/EditLocation/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocation(int id, [Bind("ID,Name,Description,Category,Latitude,Longitude")] Location location, IFormFile Image)
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
                return RedirectToAction("Locations");
            }
            return View(location);
        }

        [Authorize]
        // GET: Admin/DeleteLocation/5
        public async Task<IActionResult> DeleteLocation(int? id)
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
        // POST: Admin/DeleteLocation/5
        [HttpPost, ActionName("DeleteLocation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLocationConfirmed(int id)
        {
            var location = await _context.Location.SingleOrDefaultAsync(m => m.ID == id);
            _context.Location.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction("Locations");
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> EditComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.ID == id);
            // Nor found if comment null or user is not owned the comment and not an admin
            if (comment == null || (comment.User != User.Identity.Name && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, [Bind("ID,Header,Content,Location")] Comment comment)
        {
            if (id != comment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.EditTime = DateTime.Now;
                    _context.Entry(comment).Property(c => c.Header).IsModified = true;
                    _context.Entry(comment).Property(c => c.EditTime).IsModified = true;
                    _context.Entry(comment).Property(c => c.Content).IsModified = true;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Locations", "Details", new { id = comment.Location });
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .SingleOrDefaultAsync(m => m.ID == id);
            // Nor found if comment null or user is not owned the comment and not an admin
            if (comment == null || (comment.User != User.Identity.Name && !User.IsInRole("Administrator")))
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("DeleteComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int id)
        {
            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.ID == id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Locations", "Details", new { id = comment.Location });
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.ID == id);
        }
    }


}
