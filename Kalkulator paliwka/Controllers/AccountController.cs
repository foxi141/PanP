using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
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

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.username == model.username && u.passwordhash == model.passwordhash);

                if (user != null)
                {
                    HttpContext.Session.SetString("userid", user.userid);
                    HttpContext.Session.SetString("username", user.username);
                    HttpContext.Session.SetString("avatar", user.avatarpath ?? "/images/avatar.png");
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError("", "Nieprawidłowy login lub hasło.");
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
