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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MapApp.Models;
using System.Collections.Generic;

namespace MapApp.Controllers
{
	[Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var userRoles = _context.Roles.Include(r => r.Users).ToList();

            ViewBag.RolesList = _context.Roles.ToDictionary(k => k.Id);
            return View(await _context.Users.ToListAsync());
        }

        // GET: Admin/Locations
        public IActionResult Locations()
        {
            return RedirectToAction("Index", "Locations");
        }

        // GET: Admin/Comments
        public IActionResult Comments()
        {
            return RedirectToAction("Index", "Comments");
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

        [Authorize]
        // GET: Admin/ManageUserRole/5
        public async Task<IActionResult> ManageUserRoles(string id)
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
            var userRoles = _context.Roles.Include(r => r.Users).ToList();

            ViewBag.RolesList = _context.Roles.ToDictionary(k => k.Id);

            return View(user);
        }

        // GET: Admin/AddRole/5
        public async Task<IActionResult> AddRole(string id, string role)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Users");
        }

        // GET: Admin/RemoveRole/5
        public async Task<IActionResult> RemoveRole(string id, string role)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, role);
            }
            else
            {
                return NotFound();
            }
            return RedirectToAction("Users");
        }
    }
}
