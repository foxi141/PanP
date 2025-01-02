using KalkulatorPaliwka.Data;
using KalkulatorPaliwka.Models;
using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    // Konstruktor wstrzykujący AppDbContext
    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    // Akcja Dashboard - widok główny
    public IActionResult Index()
    {
        // Odczytujemy userid z sesji
        var userid = GetLoggedInuserid();  // Pobieramy userid z sesji

        // Debugowanie
        Console.WriteLine($"User id from session: {userid}");

        // Jeśli użytkownik nie jest zalogowany (brak userid w sesji), przekierowujemy do logowania
        if (string.IsNullOrEmpty(userid))
        {
            return RedirectToAction("Login", "Account");  // Jeśli użytkownik nie jest zalogowany, przekierowujemy do logowania
        }

        return View();  // Renderowanie dashboardu, jeśli użytkownik jest zalogowany
    }

    // Akcja dla dodania danych paliwowych
    public IActionResult AddFuelData()
    {
        return RedirectToAction("Add", "FuelData");  // Przekierowanie do akcji Add w kontrolerze FuelData
    }

    // Akcja dla historii danych paliwowych
    public IActionResult FuelHistory()
    {
        return RedirectToAction("History", "FuelData");  // Przekierowanie do akcji History w kontrolerze FuelData
    }

    // Przykładowa metoda pomocnicza do pobierania użytkownika
    private string GetLoggedInuserid()
    {
        // Odczytanie wartości userid z sesji
        return HttpContext.Session.GetString("userid");
    }
}
