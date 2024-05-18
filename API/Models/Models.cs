namespace API.Models;

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


public enum TollFreeVehicles
{
    Motorcycle = 0,
    Tractor = 1,
    Emergency = 2,
    Diplomat = 3,
    Foreign = 4,
    Military = 5,

}