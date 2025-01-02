using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        // GET: Add fuel data
        public IActionResult Add()
        {
            // Fetch all vehicles for the dropdown
            var vehicles = _context.Vehicles.ToList();
            ViewData["Vehicles"] = vehicles; // Pass vehicles to the view
            return View(new FuelData());
        }

        // POST: Add fuel data
        [HttpPost]
        public async Task<IActionResult> Add(FuelData model)
        {
            if (ModelState.IsValid)
            {
                // Validate the selected VehicleId
                var vehicle = await _context.Vehicles.FindAsync(model.Vehicleid);
                if (vehicle == null)
                {
                    ModelState.AddModelError("Vehicleid", "Invalid vehicle selection.");
                    ViewData["Vehicles"] = _context.Vehicles.ToList(); // Repopulate dropdown
                    return View(model);
                }

                // Calculate the total cost
                model.TotalCost = (model.Distance * model.FuelConsumption) / 100 * model.FuelPrice;

                // Save FuelData to the database
                _context.FuelData.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("History");
            }

            // Repopulate vehicles in case of validation errors
            ViewData["Vehicles"] = _context.Vehicles.ToList();
            return View(model);
        }

        // GET: History of fuel data
        public async Task<IActionResult> History(int? vehicleId)
        {
            // Fetch all Vehicles for the dropdown
            var vehicles = await _context.Vehicles.ToListAsync();
            ViewData["Vehicles"] = vehicles;

            // Fetch FuelData and optionally filter by vehicleId
            var fuelDataQuery = _context.FuelData.AsQueryable();

            if (vehicleId.HasValue)
            {
                fuelDataQuery = fuelDataQuery.Where(f => f.Vehicleid == vehicleId.Value);
            }

            var fuelData = await fuelDataQuery.ToListAsync();

            return View(fuelData);
        }
    }
}
