using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MapApp.Models;
using MapApp.Models.LocationModels;
using MapApp.Models.CommentsModels;
using MapApp.Models.ViewModels;

namespace MapApp.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<MapApp.Models.LocationModels.Location> Location { get; set; }

        public DbSet<MapApp.Models.CommentsModels.Comment> Comment { get; set; }

        public DbSet<MapApp.Models.ViewModels.View> View { get; set; }

    }
}
