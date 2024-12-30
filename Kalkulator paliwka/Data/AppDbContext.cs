using Microsoft.EntityFrameworkCore;
using KalkulatorPaliwka.Models;

namespace KalkulatorPaliwka.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } // W zależności od Twojej struktury modelu
    }
}
