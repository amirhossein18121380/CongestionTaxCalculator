using API.Models;

namespace API.Persistence;

using Microsoft.EntityFrameworkCore;

public class CongestionTaxDbContext : DbContext
{
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<TollFee> TollFees { get; set; }
    public DbSet<CongestionTaxRule> CongestionTaxRules { get; set; }

    public CongestionTaxDbContext(DbContextOptions<CongestionTaxDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //modelBuilder.Entity<Vehicle>()
        //    .HasData(
        //        new Vehicle { Id = 1, VehicleType = "Car" },
        //        new Vehicle { Id = 2, VehicleType = "Motorbike" }
        //        // Add other vehicles as needed
        //    );

        modelBuilder.Entity<CongestionTaxRule>()
            .HasMany(r => r.TollFees)
            .WithOne(f => f.CongestionTaxRule)
            .HasForeignKey(f => f.CongestionTaxRuleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
