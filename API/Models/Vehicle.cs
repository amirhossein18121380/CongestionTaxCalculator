using System.ComponentModel.DataAnnotations;

namespace API.Models;
public class Vehicle : Entity<int>
{
    public Vehicle() { }
    public Vehicle(int vehicleType)
    {
        VehicleType = vehicleType;
    }
    [Required]
    public int VehicleType { get; set; }
}
