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
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.username == model.username && u.passwordhash == model.passwordhash);

                if (user != null)
                {
                    // Ustawiamy userid w sesji
                    HttpContext.Session.SetString("userid", user.userid);  // Zapisujemy `userid` w sesji
                    HttpContext.Session.SetString("username", user.username);  // Możesz także zapisać `username`

                    Console.WriteLine($"User logged in: {user.username}");  // Debugowanie

                    // Po zalogowaniu przekierowujemy na dashboard
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                }
            }

            return View(model);  // Zwracamy widok z błędami walidacji
        }

        // Log out
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Usunięcie sesji
            return RedirectToAction("Login");  // Po wylogowaniu przekierowanie na stronę logowania
        }
    }
}
