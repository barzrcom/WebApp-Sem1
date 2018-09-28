using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MapApp.Data;
using MapApp.Models.CommentsModels;
using Microsoft.AspNetCore.Authorization;

namespace MapApp.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        [Authorize(Roles = "Administrator")]
        // GET: Comments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .SingleOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create/2
        public async Task<IActionResult> Create(int? loc_id)
        {
            if (loc_id == null)
            {
                return NotFound();
            }

            var isLocExist = await _context.Location
                .SingleOrDefaultAsync(m => m.ID == loc_id);
            if (isLocExist == null)
            {
                return NotFound();
            }

            // Check if this user wrote more than one comment for this location
            if (IsDoubled(await _context.Comment.ToListAsync(), loc_id, User.Identity.Name))
                return RedirectToAction("DoubleComment");

            ViewBag.loc_id = loc_id;
            ViewBag.User = User.Identity.Name;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Header,Content,User,Location,Rating")] Comment comment)
        {
            var comments = await _context.Comment.ToListAsync();
            if (IsDoubled(comments, comment.Location, User.Identity.Name))
                return RedirectToAction("DoubleComment");


            if (ModelState.IsValid)
            {
                comment.EditTime = comment.CreateTime = DateTime.Now;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Locations", new { id = comment.Location });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Header,Content,Location,Rating")] Comment comment)
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
                    _context.Entry(comment).Property(c => c.Rating).IsModified = true;
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
                return RedirectToAction("Details", "Locations", new { id = comment.Location });
            }
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.ID == id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Locations", new { id = comment.Location });
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.ID == id);
        }

        public IActionResult DoubleComment()
        {
            return View();
        }

        // Check if there's more than one comment for a location of the same user
        public static bool IsDoubled(IEnumerable<Comment> comments, int? loc_id, string user)
        {
            var result =
                from com in comments
                where com.Location.Equals(loc_id) && com.User.Equals(user)
                select com;
            if (result.Any())
                return true;
            else
                return false;
        }

    }
}
