using Orbital.Domain.Common;
using Orbital.Domain.Sites;

namespace Orbital.Domain.Rings;

public sealed class RingMembership : Entity<MembershipId>
{
    public RingId RingId { get; private set; }
    public SiteId SiteId { get; private set; }
    public MembershipRole Role { get; private set; }
    public MembershipStatus Status { get; private set; }
    public int OrderIndex { get; internal set; }
    public DateTime JoinedAt { get; private set; }
    public string? ApplicantName { get; private set; }
    public string? ContactEmail { get; private set; }

    private RingMembership() { }

    internal static RingMembership CreateOwner(RingId ringId, SiteId siteId) =>
        new()
        {
            Id = MembershipId.New(),
            RingId = ringId,
            SiteId = siteId,
            Role = MembershipRole.Owner,
            Status = MembershipStatus.Approved,
            OrderIndex = 0,
            JoinedAt = DateTime.UtcNow
        };

    internal static RingMembership CreatePendingApproval(RingId ringId, SiteId siteId) =>
        CreateApplication(ringId, siteId, MembershipStatus.PendingApproval, null, null);
    internal static RingMembership CreateApplication(
        RingId ringId,
        SiteId siteId,
        MembershipStatus status,
        string? applicantName,
        string? contactEmail) =>
        new()
        {
            Id = MembershipId.New(),
            RingId = ringId,
            SiteId = siteId,
            Role = MembershipRole.Member,
            Status = status,
            OrderIndex = -1,
            JoinedAt = DateTime.UtcNow,
            ApplicantName = applicantName?.Trim(),
            ContactEmail = contactEmail?.Trim()
        };

    internal void Approve(int orderIndex)
    {
        Status = MembershipStatus.Approved;
        OrderIndex = orderIndex;
    }

    internal void MarkPendingApproval()
    {
        Status = MembershipStatus.PendingApproval;
    }

    internal void Reject() => Status = MembershipStatus.Rejected;}
