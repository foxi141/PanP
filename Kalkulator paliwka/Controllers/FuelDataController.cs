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
            var userid = GetLoggedInuserid();  // userid from session
            if (string.IsNullOrEmpty(userid))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch vehicles for the logged-in user from the 'Vehicles' table
            var vehicles = _context.Vehicles.Where(v => v.username == userid).ToList();  // Filter vehicles by username (userid)

            // If no vehicles found for the user, show an appropriate message
            if (vehicles.Count == 0)
            {
                TempData["Message"] = "You don't have any vehicles assigned.";
                return View();  // Render the Add view without vehicles
            }

            // Pass vehicles to the view to allow the user to select one
            ViewData["Vehicles"] = vehicles;
            return View(new FuelData());
        }



        [HttpPost]
        public async Task<IActionResult> Add(FuelData model)
        {
            var userid = model.userid;  // Pobieramy userid z formularza (lub z sesji, jeśli pole jest puste)

            if (string.IsNullOrEmpty(userid))
            {
                userid = GetLoggedInuserid();  // Jeśli userid jest puste, pobieramy z sesji
                if (string.IsNullOrEmpty(userid))
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            Console.WriteLine($"Received data: VehicleId={model.Vehicleid}, Distance={model.Distance}, FuelConsumption={model.FuelConsumption}, FuelPrice={model.FuelPrice}, userid={userid}");

            if (ModelState.IsValid)
            {
                // Validate the selected VehicleId
                var vehicle = await _context.Vehicles.FindAsync(model.Vehicleid);
                if (vehicle == null || vehicle.username != userid)  // Sprawdzamy, czy pojazd należy do użytkownika
                {
                    ModelState.AddModelError("Vehicleid", "Invalid vehicle selection.");
                    ViewData["Vehicles"] = _context.Vehicles.Where(v => v.username == userid).ToList();
                    return View(model);
                }

                // Przypisujemy userid do modelu przed zapisem
                model.userid = userid;  // Przypisujemy userid do danych paliwowych

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
                ViewData["Vehicles"] = _context.Vehicles.Where(v => v.username == userid).ToList();
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
