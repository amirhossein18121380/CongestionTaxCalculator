using API.Models;
using System.ComponentModel.DataAnnotations;

public class CongestionTaxRule : Entity<int>
{
    [Required]
    [MaxLength(100)]
    public required string City { get; set; }

    [Required]
    public int MaxDailyFee { get; set; }

    public ICollection<TollFee> TollFees { get; set; } = new List<TollFee>();
}