namespace API.Models;

using System.ComponentModel.DataAnnotations;

public interface IVehicle
{
    string GetVehicleType();
}

public class Motorbike : IVehicle
{
    public string GetVehicleType()
    {
        return "Motorbike";
    }
}

public class Car : IVehicle
{
    public string GetVehicleType()
    {
        return "Car";
    }
}

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

public enum TollFreeVehicles
{
    Motorcycle = 0,
    Tractor = 1,
    Emergency = 2,
    Diplomat = 3,
    Foreign = 4,
    Military = 5,

}

public class TollFee : Entity<int>
{
    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public int Fee { get; set; }

    [Required]
    public int CongestionTaxRuleId { get; set; }
    public CongestionTaxRule CongestionTaxRule { get; set; }
}

public class CongestionTaxRule : Entity<int>
{
    [Required]
    [MaxLength(100)]
    public required string City { get; set; }

    [Required]
    public int MaxDailyFee { get; set; }

    public ICollection<TollFee> TollFees { get; set; } = new List<TollFee>();
}


public class Entity<T>
{
    public T Id { get; set; }
}
