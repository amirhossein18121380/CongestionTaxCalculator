namespace API.Models;


//Not Used
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