using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;  // Dodajemy przestrzeń nazw dla obsługi sesji
using System.Linq;

namespace KalkulatorPaliwka.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Sprawdzenie, czy użytkownik istnieje w bazie danych
            var user = _context.Users.SingleOrDefault(u => u.username == username && u.passwordhash == password);

            if (user != null)
            {
                // Zapisujemy ID użytkownika w sesji
                HttpContext.Session.SetString("UserId", user.userid.ToString());
                return RedirectToAction("Index", "Home");  // Przekierowanie na stronę główną
            }

            // Jeśli dane są błędne, wyświetlamy komunikat o błędzie
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Usuwamy dane z sesji
            HttpContext.Session.Clear();
            return RedirectToAction("Login");  // Przekierowanie do strony logowania
        }
    }
}
