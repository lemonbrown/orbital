using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence.Configurations;

public sealed class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => SiteId.From(value));

        builder.Property(s => s.OwnerUserId)
            .HasConversion(
                id => (Guid?)id!.Value.Value,
                value => (UserId?)UserId.From(value!.Value))
            .IsRequired(false);

        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(500);

        builder.OwnsOne(s => s.Url, url =>
        {
            url.Property(u => u.Value)
                .HasColumnName("Url")
                .HasMaxLength(2048)
                .IsRequired();
        });

        builder.OwnsOne(s => s.VerificationToken, token =>
        {
            token.Property(t => t.Value)
                .HasColumnName("VerificationToken")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(s => s.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();
    }
}
