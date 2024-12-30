using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace KalkulatorPaliwka.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login page
        public IActionResult Login()
        {
            // Upewniamy się, że widok otrzymuje pusty model
            return View(new LoginViewModel());
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Sprawdzanie, czy użytkownik istnieje w bazie danych PostgreSQL
                var user = _context.Users
                    .FirstOrDefault(u => u.username == model.username && u.passwordhash == model.passwordhash);

                if (user != null)
                {
                    // Ustawienie sesji użytkownika
                    HttpContext.Session.SetString("username", user.username);

                    // Przekierowanie do kalkulatora
                    return RedirectToAction("Add", "FuelData");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Niepoprawna nazwa użytkownika lub hasło.");
                }
            }

            return View(model); // Przekazywanie modelu z błędami walidacji do widoku
        }

        // Log out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Usunięcie sesji
            return RedirectToAction("Login");  // Po wylogowaniu przekierowanie na stronę logowania
        }
    }
}
