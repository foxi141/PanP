using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using System.Linq;

namespace KalkulatorPaliwka.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Ensure Admin is logged in
        private IActionResult EnsureAdminLoggedIn()
        {
            if (HttpContext.Session.GetString("AdminLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Admin");
            }
            return null;
        }

        // Dashboard Overview
        public IActionResult Index()
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var dashboardStats = new
            {
                TotalUsers = _context.Users.Count(),
                TotalVehicles = _context.Vehicles.Count(),
                TotalFuelConsumption = _context.FuelData.Sum(f => f.FuelConsumption),
                RecentActivities = _context.UserHistories
                    .OrderByDescending(h => h.Date)
                    .Take(5)
                    .ToList()
            };

            return View("~/Views/Admin/Index.cshtml", dashboardStats); // Corrected to pass the model to the view
        }

        // List All Users
        public IActionResult Users()
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var users = _context.Users.ToList();
            return View("~/Views/Admin/Users.cshtml", users); // Explicitly specifying view path
        }

        // Add User (GET)
        public IActionResult AddUser()
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            return View("~/Views/Admin/AddUser.cshtml");
        }

        // Add User (POST)
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View("~/Views/Admin/AddUser.cshtml", user);
        }

        // Edit User (GET)
        public IActionResult EditUser(string id)
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var user = _context.Users.FirstOrDefault(u => u.userid == id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/EditUser.cshtml", user);
        }

        // Edit User (POST)
        [HttpPost]
        public IActionResult EditUser(User user)
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }
            return View("~/Views/Admin/EditUser.cshtml", user);
        }

        // Delete User
        public IActionResult DeleteUser(string id)
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var user = _context.Users.FirstOrDefault(u => u.userid == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        // List All Vehicles
        public IActionResult Vehicles()
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var vehicles = _context.Vehicles.ToList();
            return View("~/Views/Admin/Vehicles.cshtml", vehicles);
        }

        // Add Vehicle
        [HttpPost]
        public IActionResult AddVehicle(Vehicles vehicle)
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            if (ModelState.IsValid)
            {
                _context.Vehicles.Add(vehicle);
                _context.SaveChanges();
                return RedirectToAction("Vehicles");
            }
            return View("~/Views/Admin/AddVehicle.cshtml", vehicle);
        }

        // Recent User Activities
        public IActionResult Logs()
        {
            var redirect = EnsureAdminLoggedIn();
            if (redirect != null) return redirect;

            var logs = _context.UserHistories
                .OrderByDescending(h => h.Date)
                .ToList();
            return View("~/Views/Admin/Logs.cshtml", logs);
        }
    }
}
