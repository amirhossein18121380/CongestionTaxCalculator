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


        var tollFreeVehicles = new Vehicle[]
            {
            new Vehicle { VehicleType = (int)TollFreeVehicles.Motorcycle },
            new Vehicle { VehicleType = (int)TollFreeVehicles.Tractor },
            new Vehicle { VehicleType = (int)TollFreeVehicles.Emergency },
            new Vehicle { VehicleType = (int)TollFreeVehicles.Diplomat },
            new Vehicle { VehicleType = (int)TollFreeVehicles.Foreign },
            new Vehicle { VehicleType = (int)TollFreeVehicles.Military },
            new Vehicle { VehicleType = 8 }
            };

        foreach (var vehicle in tollFreeVehicles)
        {
            context.Vehicles.Add(vehicle);
        }

        var gothenburgRule = new CongestionTaxRule
        {
            City = "Gothenburg",
            MaxDailyFee = 60
        };

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

        context.SaveChanges();
    }
}

