using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.CheckSnippet;

public sealed class CheckSnippetCommandHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository,
    ISnippetCheckService snippetCheckService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CheckSnippetCommand, CheckSnippetResponse>
{
    public async Task<CheckSnippetResponse> Handle(CheckSnippetCommand request, CancellationToken cancellationToken)
    {
        var ring = await ringRepository.FindByIdWithDetailsAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        var membershipId = MembershipId.From(request.MembershipId);
        var membership = ring.Memberships.FirstOrDefault(m => m.Id == membershipId)
            ?? throw NotFoundException.For("Membership", request.MembershipId);

        if (membership.Status != MembershipStatus.PendingVerification)
            return new CheckSnippetResponse(false, membership.Status, "Membership is not awaiting snippet verification.");

        var site = await siteRepository.FindByIdAsync(membership.SiteId, cancellationToken)
            ?? throw NotFoundException.For("Site", membership.SiteId.Value);

        var found = await snippetCheckService.CheckAsync(site.Url.Value, site.Id.Value, cancellationToken);
        if (!found)
            return new CheckSnippetResponse(false, MembershipStatus.PendingVerification,
                "Snippet not found. Add the script tag to your site and try again.");

        var approveResult = ring.MarkSystemVerified(membershipId);
        if (approveResult.IsFailure)
            throw new DomainException(approveResult.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var newStatus = membership.Status;
        var message = newStatus == MembershipStatus.Approved 
            ? "Snippet verified. Your site has been approved."
            : "Snippet verified. Your application is now pending owner approval.";

        return new CheckSnippetResponse(true, newStatus, message);
    }}
