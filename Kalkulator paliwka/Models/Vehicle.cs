using Kalkulator_paliwka.Views.Admin;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KalkulatorPaliwka.Models; 
public class Vehicles
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Brand { get; set; } // Marka pojazdu

    [Required]
    public string Model { get; set; } // Model pojazdu

    [Required]
    public string RegistrationNumber { get; set; } // Numer rejestracyjny

    public string ?username { get; set; } // Zmienione na Username

    public string photopath { get; set; } = "/vehiclephotos/default.png";
}
