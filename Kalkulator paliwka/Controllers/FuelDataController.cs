using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace KalkulatorPaliwka.Controllers
{
    public class FuelDataController : Controller
    {
        private readonly AppDbContext _context;

        public FuelDataController(AppDbContext context)
        {
            _context = context;
        }

        // Strona dodawania danych paliwa
        [HttpGet]
        public IActionResult Add()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekierowanie do logowania
            }

            // Tworzenie nowego obiektu FuelData z domyślnymi wartościami
            var model = new FuelData
            {
                FuelConsumed = 0  // Ustawienie domyślnej wartości dla spalonego paliwa
            };

            return View(model);
        }

        // Obsługa formularza dodawania danych paliwa
        [HttpPost]
        public async Task<IActionResult> Add(FuelData model)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekierowanie do logowania
            }

            // Obliczanie spalonego paliwa na podstawie średniego spalania i przebytego dystansu
            if (model.AverageConsumption > 0 && model.Distance > 0)
            {
                model.FuelConsumed = CalculateFuelConsumption(model.AverageConsumption, model.Distance);
            }
            else
            {
                // Jeśli dane są nieprawidłowe, ustawienie komunikatu
                TempData["Error"] = "Podaj poprawne wartości dla średniego spalania i przebytego dystansu.";
                return View(model);
            }

            // Przypisanie użytkownika
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);
            if (user != null)
            {
                model.userid = user.userid;
                _context.FuelData.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("History");
            }

            return View(model);
        }

        // Wyliczenie spalonego paliwa
        private double CalculateFuelConsumption(double averageConsumption, double distance)
        {
            return (averageConsumption / 100) * distance;
        }

        // Widok z historią danych paliwa
        public IActionResult History()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekierowanie do logowania
            }

            var user = _context.Users.FirstOrDefault(u => u.username == username);
            var fuelData = _context.FuelData.Where(f => f.userid == user.userid).ToList();

            return View(fuelData);
        }
    }
}
