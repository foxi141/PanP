public class UserModel
{
    public int UserModelId { get; set; }  // Klucz główny
    public string Username { get; set; }

    // Kolekcja przypisanych pojazdów (jeśli chcesz mieć możliwość przypisania wielu pojazdów do jednego użytkownika)
    public ICollection<VehicleModel> Vehicles { get; set; }
}
