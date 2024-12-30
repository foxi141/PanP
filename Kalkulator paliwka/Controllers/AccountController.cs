using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using KalkulatorPaliwka.Models;
using KalkulatorPaliwka.Data;

namespace KalkulatorPaliwka.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Widok logowania
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Obsługa logowania
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Sprawdzanie poprawności logowania
            var user = _context.Users.FirstOrDefault(u => u.username == username && u.passwordhash == password);
            if (user != null)
            {
                // Przechowanie nazwy użytkownika w sesji
                HttpContext.Session.SetString("Username", user.username);
                return RedirectToAction("Add", "FuelData");  // Przekierowanie do strony dodawania danych paliwa
            }

            // Jeśli dane są niepoprawne, zwróć widok logowania
            ModelState.AddModelError("", "Nieprawidłowy login lub hasło");
            return View();
        }

        // Wylogowanie
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Username");
            return RedirectToAction("Login", "Account");
        }
    }
}
