using Microsoft.EntityFrameworkCore;
using Kalkulator_paliwka.Models;

namespace Kalkulator_paliwka.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<VehicleModel> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Określamy, że jeden użytkownik może mieć wiele pojazdów
            modelBuilder.Entity<VehicleModel>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles) // Jeden użytkownik może mieć wiele pojazdów
                .HasForeignKey(v => v.UserModelId);  // Klucz obcy
        }
    }
}
