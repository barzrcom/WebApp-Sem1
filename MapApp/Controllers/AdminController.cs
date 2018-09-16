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
using Microsoft.AspNetCore.Http;
using System.IO;

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
    }
}
