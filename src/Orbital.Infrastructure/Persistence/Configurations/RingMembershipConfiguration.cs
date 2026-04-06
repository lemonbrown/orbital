using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;

namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class RingMembershipConfiguration : IEntityTypeConfiguration<RingMembership>
{
    public void Configure(EntityTypeBuilder<RingMembership> builder)
    {
        builder.ToTable("RingMemberships");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id)
            .HasConversion(id => id.Value, value => MembershipId.From(value));

        builder.Property(m => m.RingId)
            .HasConversion(id => id.Value, value => RingId.From(value))
            .IsRequired();

        builder.Property(m => m.SiteId)
            .HasConversion(id => id.Value, value => SiteId.From(value))
            .IsRequired();

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(m => m.OrderIndex).IsRequired();
        builder.Property(m => m.JoinedAt).IsRequired();
    }
}
