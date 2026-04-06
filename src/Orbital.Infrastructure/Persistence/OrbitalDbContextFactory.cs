using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orbital.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so EF CLI doesn't need to reference Orbital.Api.
/// </summary>
public sealed class OrbitalDbContextFactory : IDesignTimeDbContextFactory<OrbitalDbContext>
{
    public OrbitalDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrbitalDbContext>()
            .UseSqlite("Data Source=orbital.db")
            .Options;

        return new OrbitalDbContext(options);
    }
}
