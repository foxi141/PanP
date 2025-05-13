using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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

        public IActionResult Login() => View();

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

        [HttpGet]
        public IActionResult Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Users.ToList());
        }

        [HttpGet]
        public IActionResult Vehiclelist()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(_context.Vehicles.ToList());
        }

      

        [HttpGet]
        public IActionResult EditUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(IFormFile Photo, string RegistrationNumber, string Brand, string Model, string username)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            string photoPath = "/images/default_car.png";
            if (Photo != null && Photo.Length > 0)
            {
                var fileName = Path.GetFileName(Photo.FileName);
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(stream);
                }
                photoPath = $"/images/{fileName}";
            }

            var vehicle = new Vehicles
            {
                RegistrationNumber = RegistrationNumber,
                Brand = Brand,
                Model = Model,
                username = username,
                photopath = photoPath
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Pojazd został dodany.";
            return RedirectToAction("Vehiclelist");
        }

        [HttpGet]
        public IActionResult DeleteVehicle(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null) return NotFound();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVehicle(Vehicles model)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == model.Id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
            }
            return RedirectToAction("Vehiclelist");
        }

        [HttpGet]
        public IActionResult AssignVehicle()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var userList = _context.Users
                .Select(u => new SelectListItem { Text = u.username, Value = u.userid })
                .ToList();

            var vehicleList = _context.Vehicles
                .Select(v => new SelectListItem { Text = $"{v.RegistrationNumber} - {v.Model}", Value = v.Id.ToString() })
                .ToList();

            ViewBag.Users = userList;
            ViewBag.Vehicles = vehicleList;

            return View();
        }

        [HttpPost]
        public IActionResult AssignVehicle(string UserId, int VehicleId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var vehicle = _context.Vehicles.FirstOrDefault(v => v.Id == VehicleId);
            if (vehicle != null)
            {
                vehicle.username = UserId;
                _context.SaveChanges();
                TempData["Message"] = "Pojazd został przypisany.";
            }

            return RedirectToAction("Vehiclelist");
        }
        [HttpGet]
        public IActionResult UserHistory()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.userid,
                    Text = u.username
                }).ToList();

            ViewBag.SelectedUser = null;
            return View(new List<FuelData>());
        }

        [HttpPost]
        public IActionResult UserHistory(string UserId)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.userid,
                    Text = u.username
                }).ToList();

            var history = _context.FuelData
                .Where(f => f.userid == UserId)
                .ToList();

            var vehicleBrands = _context.Vehicles
                .ToDictionary(v => v.Id, v => v.Brand); // Id to Vehicleid

            ViewBag.Users = users;
            ViewBag.SelectedUser = UserId;
            ViewBag.VehicleBrands = vehicleBrands;

            return View(history);
        }
        [HttpGet]
        public IActionResult AddUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User model, IFormFile avatar)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string avatarPath = "/images/avatar.png";

            if (avatar != null && avatar.Length > 0)
            {
                var fileName = Path.GetFileName(avatar.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await avatar.CopyToAsync(stream);
                }
                avatarPath = $"/images/{fileName}";
            }

            model.createdat = DateTime.UtcNow;
            model.avatarpath = avatarPath;

            _context.Users.Add(model);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Użytkownik został dodany.";
            return RedirectToAction("Users");
        }


        [HttpGet]
        public IActionResult DeleteUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.userid,
                    Text = u.username
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(string userid)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.userid == userid);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Users");
        }




        [HttpGet]
        public IActionResult EditVehicle() => IsAdmin() ? View() : RedirectToAction("Login");

        [HttpGet]
        public IActionResult Logs() => IsAdmin() ? View() : RedirectToAction("Login");

     

        private bool IsAdmin() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("adminid"));
    }
}
