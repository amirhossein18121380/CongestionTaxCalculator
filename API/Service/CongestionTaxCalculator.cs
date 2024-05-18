namespace API.Service;

using API.Models;
using API.Persistence;
using System;
using System.Linq;

public interface ICongestionTaxCalculator
{
    int GetTax(string city, Vehicle vehicle, DateTime[] dates);
}

public class CongestionTaxCalculator : ICongestionTaxCalculator
{
    private readonly CongestionTaxDbContext _context;

    public CongestionTaxCalculator(CongestionTaxDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public int GetTax(string city, Vehicle vehicle, DateTime[] dates)
    {
        if (dates == null || dates.Length == 0 || IsTollFreeVehicle(vehicle))
            return 0;

        var rule = GetCongestionTaxRuleForCity(city);

        if (rule == null)
            throw new Exception($"Not Found congestion tax rules for city: {city}");

        var totalFee = CalculateTotalFee(rule, dates);

        return totalFee > rule.MaxDailyFee ? rule.MaxDailyFee : totalFee;
    }

    private CongestionTaxRule GetCongestionTaxRuleForCity(string city)
    {
        var rule = _context.CongestionTaxRules.FirstOrDefault(r => r.City == city);

        if (rule == null)
            throw new Exception($"No congestion tax rules found for city: {city}");

        rule.TollFees = _context.TollFees.Where(x => x.CongestionTaxRuleId == rule.Id).ToList();

        return rule;
    }

    private int CalculateTotalFee(CongestionTaxRule rule, DateTime[] dates)
    {
        Array.Sort(dates);
        int totalFee = 0;
        DateTime intervalStart = dates[0];
        int tempFee = 0;

        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(rule, date);

            ////The single charge rule
            if (IsWithinSameHour(intervalStart, date))
            {
                tempFee = Math.Max(tempFee, nextFee);
            }
            else
            {
                totalFee += tempFee;
                tempFee = nextFee;
                intervalStart = date;
            }
        }

        totalFee += tempFee;

        return totalFee;
    }

    private bool IsWithinSameHour(DateTime start, DateTime end)
    {
        return (end - start).TotalMinutes <= 60;
    }

    private int GetTollFee(CongestionTaxRule rule, DateTime date)
    {
        if (IsTollFreeDate(date.AddDays(2)))
            return 0;

        TimeSpan timeOfDay = date.TimeOfDay;

        var fee = rule.TollFees.FirstOrDefault(t => timeOfDay >= t.StartTime && timeOfDay <= t.EndTime);
        return fee?.Fee ?? 0;
    }

    //The tax is not charged on weekends (Saturdays and Sundays),
    //public holidays, days before a public holiday and during the month of July.
    private bool IsTollFreeDate(DateTime date)
    {
        //limit the scope to the year 2013.
        var publicHolidays = new[]
        {
            new DateTime(2013, 1, 1),
            new DateTime(2013, 3, 28),
            new DateTime(2013, 3, 29),
            new DateTime(2013, 4, 1),
            new DateTime(2013, 4, 30),
            new DateTime(2013, 5, 1),
            new DateTime(2013, 5, 8),
            new DateTime(2013, 5, 9),
            new DateTime(2013, 6, 5),
            new DateTime(2013, 6, 6),
            new DateTime(2013, 6, 21),
            new DateTime(2013, 11, 1),
            new DateTime(2013, 12, 24),
            new DateTime(2013, 12, 25),
            new DateTime(2013, 12, 26),
            new DateTime(2013, 12, 31)
        };

        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ||
               publicHolidays.Contains(date.Date) || date.Month == 7;
    }

    //Tax Exempt vehicles
    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle == null || vehicle.VehicleType == null)
            return false;

        var vehicleType = (TollFreeVehicles)vehicle.VehicleType;
        return new[]
        {
            TollFreeVehicles.Motorcycle,
            TollFreeVehicles.Tractor,
            TollFreeVehicles.Emergency,
            TollFreeVehicles.Diplomat,
            TollFreeVehicles.Foreign,
            TollFreeVehicles.Military
        }.Contains(vehicleType);
    }
}