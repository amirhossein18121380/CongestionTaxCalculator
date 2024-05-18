using API.Models;
using API.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CongestionTaxController : ControllerBase
{
    private readonly ICongestionTaxCalculator _taxCalculator;
    private readonly CongestionTaxDbContext _context;

    public CongestionTaxController(ICongestionTaxCalculator taxCalculator, CongestionTaxDbContext context)
    {
        _taxCalculator = taxCalculator;
        _context = context;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateTax([FromBody] CalculateTaxRequest request)
    {
        if (request == null || request.Dates == null || !request.Dates.Any())
        {
            return BadRequest("Invalid request data");
        }

        var vehicle = _context.Vehicles.FirstOrDefault(v => v.VehicleType == request.VehicleType);
        if (vehicle == null)
        {
            vehicle = new Vehicle(request.VehicleType);
        }

        var dates = request.Dates.Select(d => DateTime.Parse(d)).ToArray();
        var tax = _taxCalculator.GetTax(request.City, vehicle, dates);

        return Ok(new { TotalTax = tax });
    }
}

public class CalculateTaxRequest
{
    public string City { get; } = "Gothenburg";
    public int VehicleType { get; set; }
    public List<string> Dates { get; set; }
}