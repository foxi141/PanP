namespace KalkulatorPaliwka.Models
{
    public class User
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string passwordhash { get; set; } // Zmieniono nazwę pola
        public string email { get; set; }
        public DateTime createdat { get; set; }
    }
}