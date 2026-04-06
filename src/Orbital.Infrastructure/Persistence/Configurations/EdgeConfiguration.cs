using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class EdgeConfiguration : IEntityTypeConfiguration<Edge>
{
    public void Configure(EntityTypeBuilder<Edge> builder)
    {
        builder.ToTable("Edges");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasConversion(id => id.Value, value => EdgeId.From(value));

        builder.Property(e => e.RingId)
            .HasConversion(id => id.Value, value => RingId.From(value))
            .IsRequired();

        builder.Property(e => e.FromSiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .IsRequired();

        builder.Property(e => e.ToSiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .IsRequired();

        builder.Property(e => e.Dimension)
            .HasConversion(d => d.Value, v => Dimension.FromString(v))
            .HasColumnName("Dimension")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Label)
            .HasConversion(l => l.Value, v => EdgeLabel.FromString(v))
            .HasColumnName("Label")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Weight).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => new { e.RingId, e.FromSiteId });
    }
}
