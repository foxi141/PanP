using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
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

        // GET: Admin/AddUser
        public IActionResult AddUser()
        {
            return View(); // Formularz do dodawania użytkownika
        }

        // POST: Admin/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState jest niepoprawny: " + string.Join(", ", ModelState.Values.Select(v => v.Errors.FirstOrDefault()?.ErrorMessage)));
                return View(user);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(user.userid))
                {
                    ModelState.AddModelError("userid", "Pole 'ID użytkownika' jest wymagane.");
                    return View(user);
                }

                user.createdat = DateTime.UtcNow;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Użytkownik został dodany: " + user.username);
                TempData["SuccessMessage"] = "Użytkownik został pomyślnie dodany.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError("Błąd przy dodawaniu użytkownika: " + ex.Message);
                TempData["ErrorMessage"] = "Błąd bazy danych: " + ex.Message;
                return View(user);
            }
        }

        // GET: Admin/AddVehicle
        public IActionResult AddVehicle()
        {
            return View(); // Formularz do dodawania pojazdu
        }

        // POST: Admin/AddVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(Vehicles vehicle)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState jest niepoprawny: " + string.Join(", ", ModelState.Values.Select(v => v.Errors.FirstOrDefault()?.ErrorMessage)));
                return View(vehicle);
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.username == vehicle.username);
                if (user == null)
                {
                    ModelState.AddModelError("username", "Użytkownik o tym username nie istnieje.");
                    return View(vehicle);
                }

                

                // Dodanie pojazdu do bazy danych
                await _context.Vehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Pojazd {vehicle.Model} ({vehicle.RegistrationNumber}) został dodany.");
                TempData["SuccessMessage"] = "Pojazd został pomyślnie dodany.";
                return RedirectToAction("AddVehicle");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd przy dodawaniu pojazdu: {ex.Message}");
                TempData["ErrorMessage"] = "Błąd bazy danych: " + ex.Message;
                return View(vehicle);
            }
        }
    }
}
