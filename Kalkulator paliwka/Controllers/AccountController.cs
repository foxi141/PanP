using Microsoft.AspNetCore.Mvc;
using Kalkulator_paliwka.Models;
using Kalkulator_paliwka.Data;
using Microsoft.EntityFrameworkCore;

namespace Kalkulator_paliwka.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            // Sprawdzenie, czy użytkownik istnieje w bazie danych
            var user = await _context.Users.Include(u => u.AssignedVehicle)
                                            .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                // Możesz dodać logikę dla nowego użytkownika
                user = new UserModel
                {
                    Username = username,
                    AssignedVehicle = new VehicleModel
                    {
                        Make = "Toyota",
                        Model = "Corolla",
                        RegistrationNumber = "ABC123"
                    }
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // Zapisz obiekt w sesji za pomocą metod rozszerzenia
            HttpContext.Session.SetObject("CurrentUser", user);

            return RedirectToAction("Index", "FuelCalculator");
        }
    }
}
