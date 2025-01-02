namespace KalkulatorPaliwka.Models
{
    public class FuelData
    {
        public int? Id { get; set; }
        public double Distance { get; set; }
        public double FuelConsumption { get; set; }
        public double FuelPrice { get; set; }
        public double TotalCost { get; set; }
        public string userid { get; set; }
        public int? Vehicleid { get; set; }

    }
}
