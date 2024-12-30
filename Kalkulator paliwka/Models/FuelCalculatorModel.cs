using System.ComponentModel.DataAnnotations;
using Kalkulator_paliwka.Data;  // Dodaj brakujące using
using Kalkulator_paliwka.Models; // Dodaj brakujące using

namespace Kalkulator_paliwka.Models
{
    public class FuelCalculatorModel
    {
        public int CalculationId { get; set; }  // Klucz główny
        public int UserId { get; set; }  // Klucz obcy do użytkownika
        public int VehicleId { get; set; }  // Klucz obcy do pojazdu
        public double Kilometers { get; set; }
        public double AverageConsumption { get; set; }
        public double FuelBefore { get; set; }
        public double? TotalFuelNeeded { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserModel User { get; set; }  // Nawiazanie do użytkownika
        public VehicleModel Vehicle { get; set; }  // Nawiazanie do pojazdu
    }
}
