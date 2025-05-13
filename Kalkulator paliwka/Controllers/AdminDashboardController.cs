
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace KalkulatorPaliwka.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("adminid")))
                return RedirectToAction("Login", "Admin");

            return View();
        }
    }
}
