using Kalkulator_paliwka.Models;
using Microsoft.EntityFrameworkCore;

namespace Kalkulator_paliwka.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<VehicleModel> Vehicles { get; set; }
        public DbSet<FuelCalculatorModel> FuelCalculations { get; set; }
    }
}
