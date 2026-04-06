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

        // Demo user
        var userResult = User.Create("demouser", "demo@orbital.dev", "Demo1234!");
        if (userResult.IsFailure) throw new InvalidOperationException(userResult.Error);
        var user = userResult.Value;
        await db.Users.AddAsync(user);

        // Sites
        var siteData = new[]
        {
            ("Robb Knight",         "https://rknight.me",             "Personal site and blog of Robb Knight."),
            ("Keenan",              "https://gkeenan.co",             "Blog and personal musings from Keenan."),
            ("Flamed",              "https://flamed.dk",              "Danish indie blogger writing about the web."),
            ("Manu",                "https://manuelmoreale.com",      "Personal blog by Manu Moreale."),
            ("Sophie Koonin",       "https://localghost.dev",         "Web developer and blogger."),
            ("Wouter Groeneveld",   "https://brainbaking.com",        "Brain Baking — thoughts on software and learning."),
            ("Terence Eden",        "https://shkspr.mobi",            "Terence Eden's personal blog."),
            ("Clive Thompson",      "https://clivethompson.medium.com","Tech journalist and author."),
            ("Jan-Lukas Else",      "https://jlelse.blog",            "Personal blog by Jan-Lukas Else."),
            ("Winnie Lim",          "https://winnielim.org",          "Designer and writer."),
            ("Alexey Guzey",        "https://guzey.com",              "Research and ideas blog."),
            ("Tom MacWright",       "https://macwright.com",          "Software developer and writer."),
            ("Brandur Leach",       "https://brandur.org",            "Engineering blog by Brandur Leach."),
            ("Julia Evans",         "https://jvns.ca",                "Programming zines and posts by Julia Evans."),
            ("Simon Willison",      "https://simonwillison.net",      "Web developer and co-creator of Django."),
            ("Maggie Appleton",     "https://maggieappleton.com",     "Illustrated essays on programming and culture."),
            ("Jim Nielsen",         "https://blog.jim-nielsen.com",   "Thoughts on design and the web."),
            ("Baldur Bjarnason",    "https://www.baldurbjarnason.com","Writing on ebooks, the web, and publishing."),
        };

        var sites = new List<Site>();
        foreach (var (name, url, desc) in siteData)
        {
            var result = Site.Create(user.Id, name, url, desc);
            if (result.IsFailure) throw new InvalidOperationException(result.Error);
            var site = result.Value;
            site.MarkVerified();
            sites.Add(site);
        }
        await db.Sites.AddRangeAsync(sites);

        // Ring
        var ringResult = Ring.Create(user.Id, "Indie Web Bloggers", "A ring of personal blogs and indie web sites worth reading.");
        if (ringResult.IsFailure) throw new InvalidOperationException(ringResult.Error);
        var ring = ringResult.Value;

        ring.UpdateSettings(
            isPublicJoinEnabled: true,
            isApiOnboardingEnabled: false,
            isPublicDirectoryEnabled: true,
            verificationMode: VerificationMode.None,
            approvalMode: ApprovalMode.Manual);

        // Add all sites as approved members
        foreach (var site in sites)
        {
            var joinResult = ring.RequestJoin(site.Id);
            if (joinResult.IsFailure) throw new InvalidOperationException(joinResult.Error);

            var membership = joinResult.Value;
            ring.ApproveMember(membership.Id, user.Id);
        }

        await db.Rings.AddAsync(ring);
        await db.SaveChangesAsync();

        logger.LogInformation("Seeding complete. Demo credentials: demo@orbital.dev / Demo1234!");
    }
}
