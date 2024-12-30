using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KalkulatorPaliwka.Controllers
{
    public class FuelDataController : Controller
    {
        private readonly AppDbContext _context;

        public FuelDataController(AppDbContext context)
        {
            _context = context;
        }

        // Sprawdzanie, czy użytkownik jest zalogowany
        private bool IsUserLoggedIn()
        {
            var username = HttpContext.Session.GetString("username");
            return !string.IsNullOrEmpty(username);
        }

        // GET: Add fuel data
        public IActionResult Add()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Przekierowanie do logowania, jeśli użytkownik nie jest zalogowany
            }

            return View(new FuelData());  // Zwróć pusty model dla widoku
        }

        // POST: Add fuel data
        [HttpPost]
        public async Task<IActionResult> Add(FuelData model)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account"); // Przekierowanie do logowania, jeśli użytkownik nie jest zalogowany
            }

            // Jeśli model jest poprawny
            if (ModelState.IsValid)
            {
                // Przypisanie użytkownika do danych
                model.userid = HttpContext.Session.GetString("username");

                // Sprawdzamy, czy mamy poprawny userid
                if (string.IsNullOrEmpty(model.userid))
                {
                    ModelState.AddModelError("userid", "Nie znaleziono użytkownika w sesji.");
                    return View(model);  // Jeśli nie ma usera w sesji, wyświetl błąd
                }

                // Sprawdzamy czy mamy odpowiednie dane
                if (model.Distance <= 0 || model.FuelConsumption <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Proszę podać poprawne wartości dla kilometrów i spalania.");
                    return View(model);
                }

                // Oblicz całkowity koszt
                model.TotalCost = (model.Distance * model.FuelConsumption) / 100 * model.FuelPrice;

                // Dodanie danych do bazy
                _context.FuelData.Add(model);
                await _context.SaveChangesAsync();  // Zapisz dane w bazie

                return RedirectToAction("History");  // Po zapisaniu danych przekierowanie do historii
            }

            // Jeśli model jest niepoprawny, wróć do widoku z błędami
            return View(model);
        }

        // GET: History of fuel data
        public async Task<IActionResult> History()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekieruj do logowania
            }

            var username = HttpContext.Session.GetString("username");

            // Sprawdzanie, czy 'username' jest null
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");  // Jeśli username jest null, przekieruj do logowania
            }

            // Pobieramy dane paliwa dla użytkownika
            var data = await _context.FuelData
                .Where(f => f.userid == username)
                .ToListAsync();

            // Przekazujemy dane do widoku
            return View(data);
        }
    }
}
