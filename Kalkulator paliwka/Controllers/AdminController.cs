using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
            return View(); // Form to add a user
        }

        // POST: Admin/AddUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user model.");
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(VehicleList));
        }

        // GET: Admin/UserFuelHistory
        public async Task<IActionResult> UserHistory(string userId)
        {
            var users = await _context.Users.ToListAsync();
            ViewData["Users"] = users;

            if (!string.IsNullOrEmpty(userId))
            {
                var fuelData = await _context.FuelData
                    .Where(fd => fd.userid == userId)
                    .ToListAsync();

                ViewData["SelectedUserId"] = userId;
                return View(fuelData);
            }

            ViewData["SelectedUserId"] = null;
            return View(new List<FuelData>());
        }

        // GET: Admin/AssignVehicle
        public async Task<IActionResult> AssignVehicle()
        {
            var users = await _context.Users.ToListAsync();
            var vehicles = await _context.Vehicles.ToListAsync();

            ViewData["Users"] = users;
            ViewData["Vehicles"] = vehicles;

            return View();
        }

        // POST: Admin/AssignVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignVehicle(string userId, int vehicleId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userid == userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "Selected user does not exist.");
                return RedirectToAction("AssignVehicle");
            }

            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("Vehicle", "Selected vehicle does not exist.");
                return RedirectToAction("AssignVehicle");
            }

            vehicle.username = user.userid;
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction("VehicleList");
        }

        // GET: Wyświetlanie formularza dodawania pojazdu
        public IActionResult AddVehicle()
        {
            var vehicle = new Vehicles(); // Tworzymy nowy pusty obiekt
            return View(vehicle); // Przekazujemy pusty model do widoku
        }

        // POST: Dodawanie pojazdu
        [HttpPost]
        public async Task<IActionResult> AddVehicle(Vehicles vehicle)
        {
            _logger.LogInformation($"Brand: {vehicle.Brand}, Model: {vehicle.Model}, RegistrationNumber: {vehicle.RegistrationNumber}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                _logger.LogWarning("Invalid vehicle model.");
                return View(vehicle);
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(VehicleList));
        }

        // Akcja, która wyświetla listę pojazdów
        public IActionResult VehicleList()
        {
            var vehicles = _context.Vehicles.ToList();
            return View(vehicles);
        }

        // GET: Admin/EditVehicle
        public async Task<IActionResult> EditVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound(); // Jeśli pojazd nie istnieje, zwróć stronę 404
            }

            return View(vehicle);
        }

        // POST: Admin/EditVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, Vehicles updatedVehicle)
        {
            if (id != updatedVehicle.Id)
            {
                return BadRequest(); // Jeśli ID nie pasuje, zwróć błąd
            }

            if (!ModelState.IsValid)
            {
                return View(updatedVehicle); // Jeśli walidacja nie przechodzi, zwróć formularz
            }

            _context.Entry(updatedVehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(); // Zapisz zmiany w bazie
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Vehicles.Any(v => v.Id == id))
                {
                    return NotFound(); // Jeśli pojazd nie istnieje, zwróć stronę 404
                }
                else
                {
                    throw; // Jeśli wystąpił inny błąd, rzuć wyjątek
                }
            }

            return RedirectToAction(nameof(VehicleList)); // Po zapisaniu przekieruj do listy pojazdów
        }

        // GET: Admin/DeleteVehicle
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound(); // Jeśli pojazd nie istnieje, zwróć stronę 404
            }

            return View(vehicle);
        }

        // POST: Admin/DeleteVehicle
        [HttpPost, ActionName("DeleteVehicle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicleConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle); // Usuwanie pojazdu
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(VehicleList)); // Po usunięciu przekieruj do listy pojazdów
        }
    }
}
