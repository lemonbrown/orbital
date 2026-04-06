using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence;

public sealed class OrbitalDbContext(DbContextOptions<OrbitalDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Ring> Rings => Set<Ring>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrbitalDbContext).Assembly);
    }
}
