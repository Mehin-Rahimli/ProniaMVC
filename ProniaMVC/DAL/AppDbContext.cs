using Microsoft.EntityFrameworkCore;
using ProniaMVC.Models;

namespace ProniaMVC.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slide> Slides { get; set; }

    }
}
