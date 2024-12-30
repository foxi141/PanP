using System.ComponentModel.DataAnnotations;

namespace KalkulatorPaliwka.Models
{
    public class FuelData
    {
        [Key] // Określenie klucza głównego
        public int Id { get; set; } // Klucz główny
        [Required(ErrorMessage = "Kilometry są wymagane.")]
        [Range(1, int.MaxValue, ErrorMessage = "Kilometry muszą być większe od 0.")]
        public int Distance { get; set; }

        [Required(ErrorMessage = "Spalanie jest wymagane.")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Spalanie musi być większe od 0.")]
        public double FuelConsumption { get; set; }

        [Required(ErrorMessage = "Cena paliwa jest wymagana.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena paliwa musi być większa od 0.")]
        public double FuelPrice { get; set; }

        public double TotalCost { get; set; }
        public string? userid { get; set; }  // Jeśli chcesz, aby mogło być null
        // Usuń [Required], jeśli przypisujesz userid w kontrolerze
      
    }
}
