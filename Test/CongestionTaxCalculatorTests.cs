using API.Models;
using API.Persistence;
using API.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace Test;

public class CongestionTaxCalculatorTests
{
    private readonly Mock<CongestionTaxDbContext> _mockContext;
    private readonly CongestionTaxCalculator _calculator;

    public CongestionTaxCalculatorTests()
    {
        var options = new DbContextOptionsBuilder<CongestionTaxDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _mockContext = new Mock<CongestionTaxDbContext>(options);
        _calculator = new CongestionTaxCalculator(_mockContext.Object);
    }

    [Fact]
    public void GetTax_ReturnsZeroForNullDates()
    {
        // Arrange
        var mockContext = new Mock<CongestionTaxDbContext>(new DbContextOptions<CongestionTaxDbContext>());
        var calculator = new CongestionTaxCalculator(mockContext.Object);

        // Act
        var tax = calculator.GetTax("Gothenburg", new Vehicle(), null);

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void GetTax_ReturnsZeroForEmptyDates()
    {
        // Arrange
        var mockContext = new Mock<CongestionTaxDbContext>(new DbContextOptions<CongestionTaxDbContext>());
        var calculator = new CongestionTaxCalculator(mockContext.Object);

        // Act
        var tax = calculator.GetTax("Gothenburg", new Vehicle(), new DateTime[0]);

        // Assert
        Assert.Equal(0, tax);
    }

    [Fact]
    public void GetTax_ReturnsZeroForTollFreeVehicle()
    {
        // Arrange
        var mockContext = new Mock<CongestionTaxDbContext>(new DbContextOptions<CongestionTaxDbContext>());
        var calculator = new CongestionTaxCalculator(mockContext.Object);
        var vehicle = new Vehicle { VehicleType = (int)TollFreeVehicles.Motorcycle };

        // Act
        var tax = calculator.GetTax("Gothenburg", vehicle, new[] { new DateTime(2013, 1, 1, 6, 0, 0) });

        // Assert
        Assert.Equal(0, tax);
    }


    [Fact]
    public void GetTax_SingleChargeRule_ReturnsHighestFee()
    {
        var rule = new CongestionTaxRule
        {
            City = "Gothenburg",
            MaxDailyFee = 60,
            TollFees = new List<TollFee>
            {
                new TollFee { StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(6, 29, 59), Fee = 8 },
                new TollFee { StartTime = new TimeSpan(6, 30, 0), EndTime = new TimeSpan(6, 59, 59), Fee = 13 },
                new TollFee { StartTime = new TimeSpan(7, 0, 0), EndTime = new TimeSpan(7, 29, 59), Fee = 18 },
                new TollFee { StartTime = new TimeSpan(7, 30, 0), EndTime = new TimeSpan(7, 59, 59), Fee = 13 },
                new TollFee { StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(8, 29, 59), Fee = 8 }
            }
        };

        var congestionTaxRules = new List<CongestionTaxRule> { rule };

        _mockContext.Setup(c => c.CongestionTaxRules).Returns((DbSet<CongestionTaxRule>)congestionTaxRules.AsQueryable());

        _mockContext.Setup(c => c.TollFees.Where(It.IsAny<Expression<Func<TollFee, bool>>>()))
            .Returns((Expression<Func<TollFee, bool>> predicate) => rule.TollFees.AsQueryable().Where(predicate));

        var dates = new[]
        {
            new DateTime(2013, 5, 1, 6, 20, 0), // fee = 8
            new DateTime(2013, 5, 1, 6, 50, 0), // fee = 13
            new DateTime(2013, 5, 1, 7, 10, 0) // fee = 18 
        };

        //dates are in the same hour so the max would is the tax which is 18.
        //AllTypesOfVehicles.Random2 is not freeTaxVehicle so jump txt 0.
        var result = _calculator.GetTax("Gothenburg", new Vehicle((int)AllTypesOfVehicles.Random2), dates);
        Assert.Equal(18, result);
    }

    [Fact]
    public void GetExpectedTax()
    {
        var dates = new[]
        {
            new DateTime(2013, 5, 1, 6, 20, 0), // fee = 8
            new DateTime(2013, 5, 1, 6, 50, 0), // fee = 13
            new DateTime(2013, 5, 1, 7, 10, 0) // fee = 18 
        };

        //dates are in the same hour so the max would is the tax which is 18.
        //AllTypesOfVehicles.Random2 is not freeTaxVehicle so jump txt 0.
        var result = _calculator.GetTax("Gothenburg", new Vehicle { VehicleType = (int)AllTypesOfVehicles.Random2 }, dates);

        Assert.Equal(18, result);
    }


}
