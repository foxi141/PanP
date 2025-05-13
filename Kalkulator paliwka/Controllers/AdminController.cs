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
using System.Text.RegularExpressions;

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
        public IActionResult AddVehicle()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle(Vehicles vehicle, string croppedPhoto)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            Console.WriteLine("🔧 Rozpoczęto dodawanie pojazdu...");
            Console.WriteLine($"➡ Brand: {vehicle.Brand}");
            Console.WriteLine($"➡ Model: {vehicle.Model}");
            Console.WriteLine($"➡ Registration: {vehicle.RegistrationNumber}");
            Console.WriteLine($"➡ CroppedPhoto present: {!string.IsNullOrEmpty(croppedPhoto)}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ Błędy walidacji modelu:");
                foreach (var entry in ModelState)
                {
                    if (entry.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"Pole: {entry.Key}");
                        foreach (var error in entry.Value.Errors)
                        {
                            Console.WriteLine($"   ⛔ {error.ErrorMessage}");
                        }
                    }
                }

                return View(vehicle);
            }

            // Obsługa zdjęcia z Croppie
            if (!string.IsNullOrEmpty(croppedPhoto))
            {
                try
                {
                    var base64Data = Regex.Match(croppedPhoto, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                    var bytes = Convert.FromBase64String(base64Data);

                    var fileName = $"{Guid.NewGuid()}.png";
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vehiclephotos");
                    var filePath = Path.Combine(folderPath, fileName);

                    Directory.CreateDirectory(folderPath); // upewnij się, że folder istnieje
                    await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                    vehicle.photopath = $"/vehiclephotos/{fileName}";
                    Console.WriteLine($"✅ Zdjęcie zapisano jako: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Błąd przy zapisie zdjęcia: " + ex.Message);
                    ModelState.AddModelError("", "Nie udało się zapisać zdjęcia pojazdu.");
                    return View(vehicle);
                }
            }
            else
            {
                Console.WriteLine("⚠ Brak zdjęcia – użyto domyślnego.");
                vehicle.photopath = "/vehiclephotos/default.png";
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Pojazd dodany do bazy.");
            return RedirectToAction("VehicleList");
        }


        [HttpGet]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Pojazd został usunięty.";
            return RedirectToAction("VehicleList");
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
        public async Task<IActionResult> AddUser(User model, string croppedAvatar)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            string avatarPath = "/images/avatar.png"; // domyślny avatar

            if (!string.IsNullOrEmpty(croppedAvatar))
            {
                var base64Data = Regex.Match(croppedAvatar, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);
                var fileName = $"{Guid.NewGuid()}.png";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars", fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, bytes);
                avatarPath = $"/avatars/{fileName}";
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
        public async Task<IActionResult> EditVehicle(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            // jawne wskazanie ścieżki widoku (dla pewności)
            return View("~/Views/Admin/EditVehicle.cshtml", vehicle);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle(int id, Vehicles updatedVehicle, string croppedPhoto)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (id != updatedVehicle.Id) return BadRequest();

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound();

            vehicle.Brand = updatedVehicle.Brand;
            vehicle.Model = updatedVehicle.Model;
            vehicle.RegistrationNumber = updatedVehicle.RegistrationNumber;

            if (!string.IsNullOrEmpty(croppedPhoto))
            {
                var base64Data = Regex.Match(croppedPhoto, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);
                var fileName = $"{Guid.NewGuid()}.png";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/vehiclephotos", fileName);
                await System.IO.File.WriteAllBytesAsync(path, bytes);
                vehicle.photopath = $"/vehiclephotos/{fileName}";
            }

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction("VehicleList");
        }
        // GET: wybór użytkownika do edycji, lub przekierowanie jeśli podano userid
        [HttpGet]
        public async Task<IActionResult> EditUser(string userid)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (string.IsNullOrEmpty(userid)) return RedirectToAction("Users");

            var u = await _context.Users.FindAsync(userid);
            if (u == null) return NotFound();

            return View(u);
        }

        // POST: zapis edycji
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(string userid, User model, string croppedAvatar)
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            if (userid != model.userid) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            // pobierz z bazy
            var u = await _context.Users.FindAsync(userid);
            if (u == null) return NotFound();

            // zaktualizuj pola
            u.username = model.username;
            u.email = model.email;
            u.passwordhash = model.passwordhash;

            // obsługa nowego avatara
            if (!string.IsNullOrEmpty(croppedAvatar))
            {
                var data = Regex.Match(croppedAvatar, @"data:image/.+;base64,(.+)").Groups[1].Value;
                var bytes = Convert.FromBase64String(data);
                var fn = $"{Guid.NewGuid()}.png";
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars");
                Directory.CreateDirectory(dir);
                var fp = Path.Combine(dir, fn);
                await System.IO.File.WriteAllBytesAsync(fp, bytes);
                u.avatarpath = $"/avatars/{fn}";
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Użytkownik zaktualizowany.";
            return RedirectToAction("Users");
        }
        [HttpGet]
        public IActionResult Logs() => IsAdmin() ? View() : RedirectToAction("Login");

     

        private bool IsAdmin() =>
            !string.IsNullOrEmpty(HttpContext.Session.GetString("adminid"));
    }
}
