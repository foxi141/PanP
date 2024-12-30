using Microsoft.EntityFrameworkCore;
using KalkulatorPaliwka.Models;
using Kalkulator_paliwka.Models;

namespace KalkulatorPaliwka.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet dla użytkowników
        public DbSet<User> Users { get; set; }

        // DbSet dla danych o paliwie
        public DbSet<FuelData> FuelData { get; set; }
    }
}
