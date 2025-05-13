using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using System.Linq;
using System.Threading.Tasks;

namespace KalkulatorPaliwka.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("adminid", admin.Id.ToString());
                HttpContext.Session.SetString("avatar", "/images/avatar.png");
                return RedirectToAction("Index", "AdminDashboard");
            }

            ModelState.AddModelError("", "Nieprawidłowy login lub hasło.");
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Przykładowa akcja chroniona
        public IActionResult Users()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("adminid")))
                return RedirectToAction("Login");

            var users = _context.Users.ToList();
            return View(users);
        }
    }
}
