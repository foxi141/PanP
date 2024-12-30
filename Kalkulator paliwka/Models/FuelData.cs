namespace KalkulatorPaliwka.Models
{
    public class FuelData
    {
        public int Id { get; set; }
        public int userid { get; set; }
        public DateTime Date { get; set; }
        public decimal AverageConsumption { get; set; } // Średnie spalanie (l/100km)
        public int Distance { get; set; } // Przebyte kilometry
        public decimal FuelConsumed { get; set; } // Spalone paliwo (l)
    }
}
