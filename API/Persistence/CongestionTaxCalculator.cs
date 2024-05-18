namespace API.Persistence;

using API.Models;
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

        var rule = _context.CongestionTaxRules
            .FirstOrDefault(r => r.City == city);
        if (rule == null)
        {
            return 0;
        }
        var fees = _context.TollFees
            .Where(x => x.CongestionTaxRuleId == rule.Id).ToList();

        rule.TollFees = fees;

        if (rule == null)
            throw new Exception($"No congestion tax rules found for city: {city}");

        Array.Sort(dates);
        int totalFee = 0;
        DateTime intervalStart = dates[0];

        foreach (DateTime date in dates)
        {
            int nextFee = GetTollFee(rule, date);
            int tempFee = GetTollFee(rule, intervalStart);

            long diffInMillies = date.Millisecond - intervalStart.Millisecond;
            long minutes = diffInMillies / 1000 / 60;

            if (minutes <= 60)
            {
                if (totalFee > 0) totalFee -= tempFee;
                if (nextFee >= tempFee) tempFee = nextFee;
                totalFee += tempFee;
            }
            else
            {
                totalFee += nextFee;
            }
        }

        return totalFee > rule.MaxDailyFee ? rule.MaxDailyFee : totalFee;
    }

    private int GetTollFee(CongestionTaxRule rule, DateTime date)
    {
        if (IsTollFreeDate(date))
            return 0;

        TimeSpan timeOfDay = date.TimeOfDay;

        var fee = rule.TollFees.FirstOrDefault(t => timeOfDay >= t.StartTime && timeOfDay <= t.EndTime);
        return fee?.Fee ?? 0;
    }

    //The tax is not charged on weekends (Saturdays and Sundays),
    //public holidays, days before a public holiday and during the month of July.
    private bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            return true;

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

        return publicHolidays.Contains(date.Date) || date.Month == 7;
    }

    //Tax Exempt vehicles
    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        if (vehicle?.VehicleType.ToString() == null) return false;
        var vehicleType = vehicle.VehicleType;
        return vehicleType.Equals((int)TollFreeVehicles.Motorcycle) ||
               vehicleType.Equals((int)TollFreeVehicles.Tractor) ||
               vehicleType.Equals((int)TollFreeVehicles.Emergency) ||
               vehicleType.Equals((int)TollFreeVehicles.Diplomat) ||
               vehicleType.Equals((int)TollFreeVehicles.Foreign) ||
               vehicleType.Equals((int)TollFreeVehicles.Military);
    }
}
