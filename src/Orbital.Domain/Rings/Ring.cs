using Orbital.Domain.Common;
using Orbital.Domain.Rings.Events;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Domain.Rings;

public sealed class Ring : AggregateRoot<RingId>
{
    private readonly List<RingMembership> _memberships = [];
    private readonly List<Edge> _edges = [];
    private readonly List<RingApiKey> _apiKeys = [];

    public UserId OwnerUserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public RingVisibility Visibility { get; private set; }
    public VerificationMode VerificationMode { get; private set; }
    public ApprovalMode ApprovalMode { get; private set; }
    public bool IsPublicJoinEnabled { get; private set; }
    public bool IsApiOnboardingEnabled { get; private set; }
    public bool IsPublicDirectoryEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RingActivityConfig ActivityConfig { get; private set; } = default!;

    public IReadOnlyList<RingMembership> Memberships => _memberships.AsReadOnly();
    public IReadOnlyList<Edge> Edges => _edges.AsReadOnly();
    public IReadOnlyList<RingApiKey> ApiKeys => _apiKeys.AsReadOnly();

    private Ring() { }

    public static Result<Ring> Create(
        UserId ownerUserId,
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
            VerificationMode = VerificationMode.None,
            ApprovalMode = ApprovalMode.Manual,
            IsPublicJoinEnabled = false,
            IsApiOnboardingEnabled = false,
            IsPublicDirectoryEnabled = false,
            CreatedAt = DateTime.UtcNow,
            ActivityConfig = RingActivityConfig.CreateDefault()
        };

        ring.RaiseDomainEvent(new RingCreatedEvent(ring.Id, ownerUserId, ring.Name));

        return Result.Success(ring);
    }

    public void UpdateSettings(
        bool isPublicJoinEnabled,
        bool isApiOnboardingEnabled,
        bool isPublicDirectoryEnabled,
        VerificationMode verificationMode,
        ApprovalMode approvalMode)
    {
        IsPublicJoinEnabled = isPublicJoinEnabled;
        IsApiOnboardingEnabled = isApiOnboardingEnabled;
        IsPublicDirectoryEnabled = isPublicDirectoryEnabled;
        VerificationMode = verificationMode;
        ApprovalMode = approvalMode;
    }

    public void ConfigureActivityTracking(RingActivityConfig config)
    {
        ActivityConfig = config;
    }

    public Result<RingMembership> RequestJoin(
        SiteId siteId,
        string? applicantName = null,
        string? contactEmail = null)
    {
        if (_memberships.Any(m => m.SiteId == siteId && m.Status != MembershipStatus.Rejected))
            return Result.Failure<RingMembership>("Site already has a pending or approved membership.");

        MembershipStatus status;

        if (VerificationMode == VerificationMode.None)
        {
            status = ApprovalMode == ApprovalMode.Auto 
                ? MembershipStatus.Approved 
                : MembershipStatus.PendingApproval;
        }
        else
        {
            status = MembershipStatus.PendingVerification;
        }

        var membership = RingMembership.CreateApplication(Id, siteId, status, applicantName, contactEmail);
        _memberships.Add(membership);

        RaiseDomainEvent(new MemberJoinedRingEvent(Id, siteId));

        if (status == MembershipStatus.Approved)
        {
            membership.OrderIndex = GetNextOrderIndex();
            RaiseDomainEvent(new MemberApprovedEvent(Id, siteId, membership.Id));
            RegenerateEdges(Dimension.Default);
        }

        return Result.Success(membership);
    }

    public Result ApproveMember(MembershipId membershipId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can approve members.");

        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Status != MembershipStatus.PendingVerification && membership.Status != MembershipStatus.PendingApproval)
            return Result.Failure("Only pending memberships can be approved.");

        var nextIndex = GetNextOrderIndex();

        membership.Approve(nextIndex);
        RaiseDomainEvent(new MemberApprovedEvent(Id, membership.SiteId, membershipId));
        RegenerateEdges(Dimension.Default);

        return Result.Success();
    }

    public Result MarkSystemVerified(MembershipId membershipId)
    {
        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Status != MembershipStatus.PendingVerification)
            return Result.Failure("Only pending-verification memberships can be system-verified.");

        if (ApprovalMode == ApprovalMode.Auto)
        {
            var nextIndex = GetNextOrderIndex();
            membership.Approve(nextIndex);
            RaiseDomainEvent(new MemberApprovedEvent(Id, membership.SiteId, membershipId));
            RegenerateEdges(Dimension.Default);
        }
        else
        {
            membership.MarkPendingApproval();
        }

        return Result.Success();
    }

    private int GetNextOrderIndex()
    {
        return _memberships
            .Where(m => m.Status == MembershipStatus.Approved)
            .Select(m => m.OrderIndex)
            .DefaultIfEmpty(-1)
            .Max() + 1;
    }

    public Result RejectMember(MembershipId membershipId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can reject members.");

        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Status != MembershipStatus.PendingVerification && membership.Status != MembershipStatus.PendingApproval)
            return Result.Failure("Only pending memberships can be rejected.");

        membership.Reject();
        return Result.Success();
    }

    public Result RemoveMember(MembershipId membershipId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can remove members.");

        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership is null)
            return Result.Failure("Membership not found.");

        if (membership.Role == MembershipRole.Owner)
            return Result.Failure("The ring owner cannot be removed.");

        _memberships.Remove(membership);

        if (membership.Status == MembershipStatus.Approved)
            RegenerateEdges(Dimension.Default);

        return Result.Success();
    }

    public Result<(RingApiKey Entity, string PlainKey)> CreateApiKey(UserId requestingUserId, string label)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure<(RingApiKey, string)>("Only the ring owner can create API keys.");

        if (string.IsNullOrWhiteSpace(label) || label.Length > 100)
            return Result.Failure<(RingApiKey, string)>("Label must be between 1 and 100 characters.");

        var (entity, plainKey) = RingApiKey.Generate(Id, label);
        _apiKeys.Add(entity);

        return Result.Success((entity, plainKey));
    }

    public Result RevokeApiKey(RingApiKeyId keyId, UserId requestingUserId)
    {
        if (requestingUserId != OwnerUserId)
            return Result.Failure("Only the ring owner can revoke API keys.");

        var key = _apiKeys.FirstOrDefault(k => k.Id == keyId);
        if (key is null)
            return Result.Failure("API key not found.");

        key.Revoke();
        return Result.Success();
    }

    public void RegenerateEdges(Dimension dimension)
    {
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
