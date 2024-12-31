using System.ComponentModel.DataAnnotations;
namespace KalkulatorPaliwka.Models;

public class UserHistory
{
    [Key]
    public string Id { get; set; } // Klucz główny
    public string username { get; set; } // Zmienione na Username
    public DateTime Date { get; set; } // Data zapisania
    public double FuelConsumption { get; set; } // Zużycie paliwa
    public double TotalCost { get; set; } // Całkowity koszt paliwa
}
