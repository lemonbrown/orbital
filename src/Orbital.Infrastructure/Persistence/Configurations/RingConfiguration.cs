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

        builder.Property(r => r.CreatedAt).IsRequired();

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
