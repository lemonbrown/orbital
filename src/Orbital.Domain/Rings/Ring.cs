using Orbital.Domain.Common;
using Orbital.Domain.Rings.Events;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Domain.Rings;

public sealed class Ring : AggregateRoot<RingId>
{
    private readonly List<RingMembership> _memberships = [];
    private readonly List<Edge> _edges = [];

    public UserId OwnerUserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public RingVisibility Visibility { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyList<RingMembership> Memberships => _memberships.AsReadOnly();
    public IReadOnlyList<Edge> Edges => _edges.AsReadOnly();

    private Ring() { }

    public static Result<Ring> Create(
        UserId ownerUserId,
        SiteId ownerSiteId,
        string name,
        string? description,
        RingVisibility visibility = RingVisibility.Public)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result.Failure<Ring>("Ring name must be between 1 and 100 characters.");

        var slug = GenerateSlug(name);

        var ring = new Ring
        {
            Id = RingId.New(),
            OwnerUserId = ownerUserId,
            Name = name.Trim(),
            Slug = slug,
            Description = description?.Trim() ?? string.Empty,
            Visibility = visibility,
            CreatedAt = DateTime.UtcNow
        };

        // Owner's site is automatically the first approved member
        var ownerMembership = RingMembership.CreateOwner(ring.Id, ownerSiteId);
        ring._memberships.Add(ownerMembership);

        ring.RaiseDomainEvent(new RingCreatedEvent(ring.Id, ownerUserId, ring.Name));

        return Result.Success(ring);
    }

    public Result RequestJoin(SiteId siteId)
    {
        if (_memberships.Any(m => m.SiteId == siteId && m.Status != MembershipStatus.Rejected))
            return Result.Failure("Site already has a pending or approved membership.");

        var membership = RingMembership.CreatePending(Id, siteId);
        _memberships.Add(membership);

        RaiseDomainEvent(new MemberJoinedRingEvent(Id, siteId));

        return Result.Success();
    }

    public Result ApproveMember(MembershipId membershipId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can approve members.");

        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Status != MembershipStatus.Pending)
            return Result.Failure("Only pending memberships can be approved.");

        var nextIndex = _memberships
            .Where(m => m.Status == MembershipStatus.Approved)
            .Max(m => m.OrderIndex) + 1;

        membership.Approve(nextIndex);

        RaiseDomainEvent(new MemberApprovedEvent(Id, membership.SiteId, membershipId));

        RegenerateEdges(Dimension.Default);

        return Result.Success();
    }

    public Result RejectMember(MembershipId membershipId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can reject members.");

        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Status != MembershipStatus.Pending)
            return Result.Failure("Only pending memberships can be rejected.");

        membership.Reject();
        return Result.Success();
    }

    public void RegenerateEdges(Dimension dimension)
    {
        // Remove existing edges for this dimension
        _edges.RemoveAll(e => e.Dimension == dimension);

        var approvedSiteIds = _memberships
            .Where(m => m.Status == MembershipStatus.Approved)
            .OrderBy(m => m.OrderIndex)
            .Select(m => m.SiteId)
            .ToList();

        var newEdges = RingEdgeGenerator.GenerateCircularEdges(Id, approvedSiteIds, dimension);
        _edges.AddRange(newEdges);

        RaiseDomainEvent(new EdgesRegeneratedEvent(Id, dimension.Value, newEdges.Count));
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.Trim().ToLowerInvariant();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');
        var suffix = Guid.NewGuid().ToString("N")[..8];
        return $"{slug}-{suffix}";
    }
}
