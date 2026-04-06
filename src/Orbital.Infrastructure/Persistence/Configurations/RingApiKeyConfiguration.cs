using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Rings;

namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class RingApiKeyConfiguration : IEntityTypeConfiguration<RingApiKey>
{
    public void Configure(EntityTypeBuilder<RingApiKey> builder)
    {
        builder.ToTable("RingApiKeys");

        builder.HasKey(k => k.Id);
        builder.Property(k => k.Id)
            .HasConversion(id => id.Value, value => RingApiKeyId.From(value));

        builder.Property(k => k.RingId)
            .HasConversion(id => id.Value, value => RingId.From(value))
            .IsRequired();

        builder.Property(k => k.Label).HasMaxLength(100).IsRequired();
        builder.Property(k => k.KeyPrefix).HasMaxLength(20).IsRequired();
        builder.Property(k => k.KeyHash).HasMaxLength(64).IsRequired();
        builder.Property(k => k.CreatedAt).IsRequired();
        builder.Property(k => k.RevokedAt);

        builder.HasIndex(k => k.KeyPrefix);
    }
}
