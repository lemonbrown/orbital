using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Rings;
using Orbital.Domain.Users;


namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class RingConfiguration : IEntityTypeConfiguration<Ring>
{
    public void Configure(EntityTypeBuilder<Ring> builder)
    {
        builder.ToTable("Rings");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasConversion(id => id.Value, value => RingId.From(value));

        builder.Property(r => r.OwnerUserId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Slug).HasMaxLength(120).IsRequired();
        builder.HasIndex(r => r.Slug).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(500);

        builder.Property(r => r.Visibility)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.VerificationMode)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(r => r.ApprovalMode)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(r => r.IsPublicJoinEnabled)
            .IsRequired();

        builder.Property(r => r.IsApiOnboardingEnabled)
            .IsRequired();

        builder.Property(r => r.CreatedAt).IsRequired();

        builder.OwnsOne(r => r.ActivityConfig, ac =>
        {
            ac.Property(a => a.IsEnabled)
                .HasColumnName("ActivityConfig_IsEnabled")
                .HasDefaultValue(false)
                .IsRequired();

            ac.Property(a => a.CrawlingEnabled)
                .HasColumnName("ActivityConfig_CrawlingEnabled")
                .HasDefaultValue(false)
                .IsRequired();

            ac.Property(a => a.RecentPostWeight)
                .HasColumnName("ActivityConfig_RecentPostWeight")
                .HasDefaultValue(2.0m)
                .IsRequired();

            ac.Property(a => a.RecentUpdateWeight)
                .HasColumnName("ActivityConfig_RecentUpdateWeight")
                .HasDefaultValue(1.5m)
                .IsRequired();

            ac.Property(a => a.ActivityWindowDays)
                .HasColumnName("ActivityConfig_ActivityWindowDays")
                .HasDefaultValue(30)
                .IsRequired();

            ac.Property(a => a.CrawlFrequency)
                .HasColumnName("ActivityConfig_CrawlFrequency")
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(CrawlFrequency.Daily)
                .IsRequired();

            ac.Property(a => a.SkipStaleSites)
                .HasColumnName("ActivityConfig_SkipStaleSites")
                .HasDefaultValue(false)
                .IsRequired();

            ac.Property(a => a.StaleSiteThresholdDays)
                .HasColumnName("ActivityConfig_StaleSiteThresholdDays")
                .HasDefaultValue(90)
                .IsRequired();
        });

        builder.HasMany(r => r.ApiKeys)
            .WithOne()
            .HasForeignKey(nameof(RingApiKey.RingId))
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Memberships as a child collection — accessed via EF navigation only
        builder.HasMany(r => r.Memberships)
            .WithOne()
            .HasForeignKey(nameof(RingMembership.RingId))
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // Edges as a child collection
        builder.HasMany(r => r.Edges)
            .WithOne()
            .HasForeignKey(nameof(Edge.RingId))
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
