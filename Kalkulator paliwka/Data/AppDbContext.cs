using KalkulatorPaliwka.Models;
using Microsoft.EntityFrameworkCore;

namespace KalkulatorPaliwka.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        public DbSet<FuelData> FuelData { get; set; }

        public DbSet<Vehicles> Vehicles { get; set; }
        public DbSet<UserHistory> UserHistories { get; set; }
    }
}





