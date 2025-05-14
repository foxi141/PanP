using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KalkulatorPaliwka.Models
{
    public class Vehicles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        public string photopath { get; set; } = "/vehiclephotos/default.png";

        public DateTime? technicalinspectiondate { get; set; }

        public DateTime? insurancedate { get; set; }
        public string? username { get; set; } // Zmienione na Username
    }
}
