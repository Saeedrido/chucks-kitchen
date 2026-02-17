using ChuksKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChuksKitchen.Persistence.Configurations;

public class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(f => f.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(f => f.ImageUrl)
            .HasMaxLength(500);

        builder.Property(f => f.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.PreparationTimeMinutes)
            .IsRequired();

        builder.Property(f => f.StockQuantity)
            .IsRequired();

        builder.Property(f => f.SpiceLevel)
            .HasMaxLength(50);

        // Note: Relationships configured via navigation properties
    }
}
