namespace Kalkulator_paliwka.Models
{
   public class VehicleModel
    {
        public int VehicleId { get; set; }  // Klucz główny
        public string Make { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public int UserId { get; set; }  // Klucz obcy do użytkownika
        public UserModel User { get; set; } // Nawiazanie do użytkownika
    }
}
