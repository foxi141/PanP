using Kalkulator_paliwka.Views.Admin;
using System.ComponentModel.DataAnnotations;
namespace KalkulatorPaliwka.Models; 
public class Vehicles
{
    [Key]
    public int Id { get; set; } // Klucz główny
    [Required]
    public string Brand { get; set; } // Marka pojazdu

    [Required]
    public string Model { get; set; } // Model pojazdu

    [Required]
    public string RegistrationNumber { get; set; } // Numer rejestracyjny

    public string ?username { get; set; } // Zmienione na Username

    public string photopath { get; set; }
}
