using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Kalkulator_paliwka.Models;

namespace KalkulatorPaliwka.Controllers
{
    public class FuelDataController : Controller
    {
        private readonly AppDbContext _context;

        public FuelDataController(AppDbContext context)
        {
            _context = context;
        }

        // Check if the user is logged in and return their userid
        private string GetLoggedInuserid()
        {
            var username = HttpContext.Session.GetString("username");  // Pobieramy 'username' z sesji
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // Pobieramy userid na podstawie username z tabeli Users
            var user = _context.Users.FirstOrDefault(u => u.username == username);
            if (user != null)
            {
                return user.userid.ToString();  // Zwracamy userid (typ string)
            }

            return null;
        }


        // GET: Add fuel data
        // GET: Add fuel data
        public IActionResult Add()
        {
            // Pobieramy userid z sesji
            var userid = GetLoggedInuserid();  // userid z sesji

            // Jeśli userid jest pusty (użytkownik nie jest zalogowany), przekierowujemy do logowania
            if (string.IsNullOrEmpty(userid))
            {
                return RedirectToAction("Login", "Account");
            }

            // Pobieramy pojazdy przypisane do użytkownika na podstawie userid
            var vehicles = _context.Vehicles.Where(v => v.username == userid).ToList();  // Filtrujemy pojazdy po username (userid)

            // Jeśli użytkownik nie ma przypisanych pojazdów, wyświetlamy odpowiedni komunikat
            if (vehicles.Count == 0)
            {
                TempData["Message"] = "Nie masz przypisanych pojazdów. Proszę dodać pojazd w sekcji pojazdów.";
                return View();  // Renderujemy widok Add bez pojazdów
            }

            // Przekazujemy pojazdy do widoku, aby użytkownik mógł wybrać pojazd
            ViewData["Vehicles"] = vehicles;

            // Tworzymy nowy obiekt FuelData z przypisanym userid
            var model = new FuelData { userid = userid };

            // Renderujemy widok z modelem FuelData
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Add(FuelData model)
        {
            // Pobieramy userid z sesji
            var userid = GetLoggedInuserid();

            Console.WriteLine($"Userid from session: {userid}");

            // Sprawdzamy, czy userid jest pusty, jeśli tak, przekierowujemy do logowania
            if (string.IsNullOrEmpty(userid))
            {
                return RedirectToAction("Login", "Account");  // Jeśli userid nie jest ustawiony w sesji, przekierowujemy do logowania
            }

            // Przypisujemy userid do modelu przed walidacją
            model.userid = userid;
            Console.WriteLine($"userid set in model: {model.userid}");

            // Jeśli userid nie jest ustawione w modelu, dodajemy błąd walidacji
            if (string.IsNullOrEmpty(model.userid))
            {
                Console.WriteLine("Error: userid is not set in the model!");
                ModelState.AddModelError("userid", "UserId is required.");
            }

            Console.WriteLine($"Received data: VehicleId={model.Vehicleid}, Distance={model.Distance}, FuelConsumption={model.FuelConsumption}, FuelPrice={model.FuelPrice}, userid={model.userid}");

            // Przed przejściem do walidacji modelu, sprawdzamy, czy userid jest przypisane
            if (string.IsNullOrEmpty(model.userid))
            {
                // Logujemy błąd jeśli userid jest puste
                Console.WriteLine("Error: userid is missing before validation.");
                ModelState.AddModelError("userid", "UserId is required.");
                return View(model);
            }

            // Jeśli model jest poprawny
            if (ModelState.IsValid)
            {
                // Sprawdzamy, czy pojazd należy do użytkownika
                var vehicle = await _context.Vehicles.FindAsync(model.Vehicleid);
                if (vehicle == null || vehicle.username != model.userid)
                {
                    ModelState.AddModelError("Vehicleid", "Invalid vehicle selection.");
                    ViewData["Vehicles"] = _context.Vehicles.Where(v => v.username == model.userid).ToList();
                    return View(model);
                }

                // Obliczamy całkowity koszt
                model.TotalCost = (model.Distance * model.FuelConsumption) / 100 * model.FuelPrice;

                Console.WriteLine($"Calculated TotalCost: {model.TotalCost}");

                // Zapisujemy dane paliwowe do bazy
                _context.FuelData.Add(model);
                await _context.SaveChangesAsync();

                Console.WriteLine("Data successfully saved to database");

                return RedirectToAction("History");
            }
            else
            {
                // Logujemy błędy walidacji
                Console.WriteLine("Model is invalid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Repopulujemy pojazdy w przypadku błędów walidacji
                ViewData["Vehicles"] = _context.Vehicles.Where(v => v.username == model.userid).ToList();
                return View(model);
            }
        }



        // GET: History of fuel data
        public async Task<IActionResult> History(int? Vehicleid)
        {
            var userid = GetLoggedInuserid();  // userid from session, saved as 'username'
            if (string.IsNullOrEmpty(userid))
            {
                return RedirectToAction("Login", "Account");
            }
            Console.WriteLine($"Fetching vehicles for userid: {userid}");

            // Fetch vehicles for the logged-in user from the 'Vehicles' table
            var vehicles = await _context.Vehicles
                .Where(v => v.username == userid)  // Fetch vehicles where username equals the logged-in userid
                .ToListAsync();
            ViewData["Vehicles"] = vehicles;  // Pass vehicles to the view

            // Fetch fuel data for the logged-in user and filter by userid
            var fuelDataQuery = _context.FuelData
                .Where(f => f.userid == userid);  // Filter fuel data by the logged-in userid

            // If a Vehicleid is provided, filter fuel data by Vehicleid
            if (Vehicleid.HasValue)
            {
                fuelDataQuery = fuelDataQuery.Where(f => f.Vehicleid == Vehicleid.Value);  // Filter by Vehicleid if provided
            }

            // Fetch the data asynchronously
            var fuelData = await fuelDataQuery.ToListAsync();

            // If no data is found, provide a message
            if (!fuelData.Any())
            {
                TempData["Message"] = "No fuel data available for the selected vehicle.";
            }

            return View(fuelData);  // Return fuel data to the view
        }
    }
}
