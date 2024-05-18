namespace API.Persistence;

using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

public static class DbInitializer
{
    public static void Initialize(CongestionTaxDbContext context)
    {
        context.Database.Migrate();

        if (context.Vehicles.Any() || context.CongestionTaxRules.Any() || context.TollFees.Any())
        {
            return; // DB has been seeded
        }

        #region Vehicles

        var tollFreeVehicles = new Vehicle[]
            {
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Motorcycle },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Tractor },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Emergency },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Diplomat },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Foreign },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Military },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random1 },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random2 },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random3 },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random4 },
            new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random5 },
            };

        foreach (var vehicle in tollFreeVehicles)
        {
            context.Vehicles.Add(vehicle);
        }
        #endregion

        #region Hours and amounts for congestion tax in Gothenburg

        var gothenburgRule = new CongestionTaxRule
        {
            City = "Gothenburg",
            MaxDailyFee = 60
        };

        //The maximum amount per day and vehicle is 60 SEK.
        //A congestion tax is charged during fixed hours for vehicles driving into and out of Gothenburg.
        var tollFees = new[]
        {
            new TollFee { StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(6, 29, 59), Fee = 8, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(6, 30, 0), EndTime = new TimeSpan(6, 59, 59), Fee = 13, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(7, 59, 59), Fee = 18, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 29, 59), Fee = 13, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(14, 59, 59), Fee = 8, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(15, 29, 59), Fee = 13, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 59, 59), Fee = 18, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(17, 0, 0), EndTime = new TimeSpan(17, 59, 59), Fee = 13, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(18, 29, 59), Fee = 8, CongestionTaxRule = gothenburgRule },
            new TollFee { StartTime = new TimeSpan(18, 30, 0), EndTime = new TimeSpan(5, 59, 59), Fee = 0, CongestionTaxRule = gothenburgRule }
        };

        context.CongestionTaxRules.Add(gothenburgRule);
        context.TollFees.AddRange(tollFees);
        #endregion

        context.SaveChanges();
    }
}
public enum AllTypesOfVehicles
{
    Motorcycle = 0,
    Tractor = 1,
    Emergency = 2,
    Diplomat = 3,
    Foreign = 4,
    Military = 5,
    Random1 = 6,
    Random2 = 7,
    Random3 = 8,
    Random4 = 9,
    Random5 = 10,
}
