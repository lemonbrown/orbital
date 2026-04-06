using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(OrbitalDbContext db, ILogger logger)
    {
        if (await db.Users.AnyAsync()) return; // Idempotent — skip if already seeded

        logger.LogInformation("Seeding development data…");

        // Create demo user
        var userResult = User.Create("demouser", "demo@orbital.dev", "Demo1234!");
        if (userResult.IsFailure) throw new InvalidOperationException(userResult.Error);
        var user = userResult.Value;
        await db.Users.AddAsync(user);

        // Create two demo sites
        var site1Result = Site.Create(user.Id, "Demo Blog", "https://demoblog.example.com", "A demo blog site");
        var site2Result = Site.Create(user.Id, "Demo Portfolio", "https://portfolio.example.com", "A demo portfolio");
        if (site1Result.IsFailure || site2Result.IsFailure) throw new InvalidOperationException("Seed site creation failed.");

        var site1 = site1Result.Value;
        var site2 = site2Result.Value;
        site1.MarkVerified();
        site2.MarkVerified();

        await db.Sites.AddRangeAsync(site1, site2);

        // Create a demo public ring with both sites
        var ringResult = Ring.Create(user.Id, site1.Id, "Demo Ring", "A demonstration web ring", RingVisibility.Public);
        if (ringResult.IsFailure) throw new InvalidOperationException(ringResult.Error);
        var ring = ringResult.Value;

        ring.RequestJoin(site2.Id);

        // Approve the second site
        var pendingMembership = ring.Memberships.First(m => m.SiteId == site2.Id);
        ring.ApproveMember(pendingMembership.Id, user.Id);

        await db.Rings.AddAsync(ring);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeding complete. Demo credentials: demo@orbital.dev / Demo1234!");
    }
}
