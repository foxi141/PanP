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
            // Fetch all users and vehicles
            var users = await _context.Users.ToListAsync();
            var vehicles = await _context.Vehicles.ToListAsync();

            // Pass data to the view
            ViewData["Users"] = users;
            ViewData["Vehicles"] = vehicles;

            return View();
        }

        // POST: Admin/AssignVehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignVehicle(string userId, int vehicleId)
        {
            // Find the selected user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.userid == userId);
            if (user == null)
            {
                ModelState.AddModelError("User", "Selected user does not exist.");
                return RedirectToAction("AssignVehicle");
            }

            // Find the selected vehicle
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                ModelState.AddModelError("Vehicle", "Selected vehicle does not exist.");
                return RedirectToAction("AssignVehicle");
            }

            // Assign the vehicle to the user
            vehicle.username = user.userid;

            // Save the changes
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction("VehicleList");
        }

        // GET: Admin/VehicleList
        public async Task<IActionResult> VehicleList()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return View(vehicles);
        }

        // GET: Admin/AddVehicle
        public async Task<IActionResult> AddVehicle(Vehicles vehicle)
        {
            if (!ModelState.IsValid)
            {
                // Log specific model state errors for debugging
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

            // Generate a random username
            vehicle.username = GenerateRandomUsername();

            // Add the vehicle to the database
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(VehicleList));
        }

        // Helper method to generate a random username
        private string GenerateRandomUsername()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // GET: Admin/EditVehicle
        public async Task<IActionResult> EditVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
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
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(updatedVehicle);
            }

            _context.Entry(updatedVehicle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Vehicles.Any(v => v.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(VehicleList));
        }

        // GET: Admin/DeleteVehicle
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
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
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(AssignVehicle));
        }
    }
}
