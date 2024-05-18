using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class CongestionTaxRule : Entity<int>
{
    [Required]
    [MaxLength(100)]
    public string City { get; set; }

    [Required]
    public int MaxDailyFee { get; set; }

    public ICollection<TollFee> TollFees { get; set; } = new List<TollFee>();
}