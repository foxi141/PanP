using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace KalkulatorPaliwka.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekierowanie do logowania
            }

            ViewBag.Username = username;  // Przekazanie nazwy użytkownika do widoku
            return View();
        }
    }
}
