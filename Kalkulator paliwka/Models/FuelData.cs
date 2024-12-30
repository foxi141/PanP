namespace KalkulatorPaliwka.Models
{
    public class FuelData
    {
        public int Id { get; set; }
        public string userid    { get; set; } // Nazwa użytkownika
        public double Distance { get; set; } // Przejechane kilometry
        public double FuelConsumption { get; set; } // Spalanie w litrach na 100 km
        public double FuelPrice { get; set; } // Cena paliwa za litr
        public double TotalCost { get; set; } // Całkowity koszt paliwa (obliczany)

        // Metoda do obliczenia kosztu paliwa
        public void CalculateTotalCost()
        {
            TotalCost = (Distance / 100) * FuelConsumption * FuelPrice;
        }
    }
}
