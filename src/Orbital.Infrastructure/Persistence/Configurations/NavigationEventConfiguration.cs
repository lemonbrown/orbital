using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Navigation;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class NavigationEventConfiguration : IEntityTypeConfiguration<NavigationEvent>
{
    public void Configure(EntityTypeBuilder<NavigationEvent> builder)
    {
        builder.ToTable("NavigationEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.RingId)
            .HasConversion(id => id.Value, value => RingId.From(value))
            .IsRequired();

        builder.Property(e => e.DestinationSiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .IsRequired();

        builder.Property(e => e.OccurredAt).IsRequired();

        builder.HasIndex(e => new { e.RingId, e.OccurredAt });
    }
}
