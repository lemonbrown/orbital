using Orbital.Domain.Common;
using Orbital.Domain.Sites.Events;
using Orbital.Domain.Users;

namespace Orbital.Domain.Sites;

public sealed class Site : AggregateRoot<SiteId>
{
    public UserId? OwnerUserId { get; private set; }
    public string Name { get; private set; } = default!;
    public SiteUrl Url { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public SiteVerificationStatus VerificationStatus { get; private set; }
    public VerificationToken VerificationToken { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private Site() { }

    public static Result<Site> Create(UserId? ownerId, string name, string url, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result.Failure<Site>("Name must be between 1 and 100 characters.");

        var urlResult = SiteUrl.Create(url);
        if (urlResult.IsFailure)
            return Result.Failure<Site>(urlResult.Error!);

        var site = new Site
        {
            Id = SiteId.New(),
            OwnerUserId = ownerId,
            Name = name.Trim(),
            Url = urlResult.Value,
            Description = description?.Trim() ?? string.Empty,
            VerificationStatus = SiteVerificationStatus.Pending,
            VerificationToken = VerificationToken.Generate(),
            CreatedAt = DateTime.UtcNow
        };

        site.RaiseDomainEvent(new SiteAddedEvent(site.Id, ownerId, site.Url.Value));

        return Result.Success(site);
    }

    public Result MarkVerified()
    {
        if (VerificationStatus == SiteVerificationStatus.Verified)
            return Result.Failure("Site is already verified.");

        VerificationStatus = SiteVerificationStatus.Verified;
        RaiseDomainEvent(new SiteVerifiedEvent(Id));
        return Result.Success();
    }

    public Result MarkFailed()
    {
        VerificationStatus = SiteVerificationStatus.Failed;
        return Result.Success();
    }
}
