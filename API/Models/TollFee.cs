using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class TollFee : Entity<int>
{
    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public int Fee { get; set; }

    [Required]
    public int CongestionTaxRuleId { get; set; }
    public CongestionTaxRule CongestionTaxRule { get; set; }
}