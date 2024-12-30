using Microsoft.AspNetCore.Mvc;
using Kalkulator_paliwka.Models;
using Kalkulator_paliwka;

namespace Kalkulator_paliwka.Controllers
{
    public class FuelCalculatorController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetObject<UserModel>("CurrentUser");

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Vehicle = user.AssignedVehicle;
            return View();
        }

        [HttpPost]
        public IActionResult Index(FuelCalculatorModel model)
        {
            var user = HttpContext.Session.GetObject<UserModel>("CurrentUser");

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                double fuelRequired = (model.Kilometers * model.AverageConsumption / 100) - model.FuelBefore;
                model.TotalFuelNeeded = fuelRequired > 0 ? fuelRequired : 0;
            }

            ViewBag.Vehicle = user.AssignedVehicle;
            return View(model);
        }
    }
}
