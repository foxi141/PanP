
using System.Collections.Generic;

namespace Kalkulator_paliwka.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Car> Cars { get; set; }
    }

    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public double FuelConsumption { get; set; }
        public User User { get; set; }
    }
}
