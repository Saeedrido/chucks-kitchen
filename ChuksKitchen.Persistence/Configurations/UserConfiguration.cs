using ChuksKitchen.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChuksKitchen.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Phone).IsUnique();

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.ReferralCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(u => u.ReferralCode).IsUnique();

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.OtpCode)
            .HasMaxLength(10);

        builder.Property(u => u.OtpExpiry)
            .IsRequired(false);

        builder.Property(u => u.OtpGeneratedAt)
            .IsRequired(false);

        builder.Property(u => u.FailedOtpAttempts)
            .IsRequired();

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        // Note: Relationships configured via navigation properties, not fluent API for InMemory
    }
}
