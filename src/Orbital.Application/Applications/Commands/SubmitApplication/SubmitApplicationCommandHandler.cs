using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;
using Orbital.Domain.Sites;
using Orbital.Domain.Users;

namespace Orbital.Application.Applications.Commands.SubmitApplication;

public sealed class SubmitApplicationCommandHandler(
    IRingRepository ringRepository,
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitApplicationCommand, SubmitApplicationResponse>
{
    public async Task<SubmitApplicationResponse> Handle(
        SubmitApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var ring = await ringRepository.FindByIdWithDetailsAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        // Access control
        if (!request.IsApiKeyRequest && request.CallerUserId is null && !ring.IsPublicJoinEnabled)
            throw new DomainException("This ring does not accept public applications.");

        if (request.IsApiKeyRequest && !ring.IsApiOnboardingEnabled)
            throw new DomainException("API onboarding is not enabled for this ring.");

        // Normalize URL and resolve/create site
        var urlResult = SiteUrl.Create(request.SiteUrl);
        if (urlResult.IsFailure)
            throw new DomainException(urlResult.Error!);

        var normalizedUrl = urlResult.Value.Value;

        var site = await siteRepository.FindByUrlAsync(normalizedUrl, cancellationToken);
        if (site is null)
        {
            var ownerId = request.CallerUserId.HasValue ? UserId.From(request.CallerUserId.Value) : (UserId?)null;
            var siteResult = Site.Create(ownerId, request.SiteName, request.SiteUrl, request.Description);
            if (siteResult.IsFailure)
                throw new DomainException(siteResult.Error!);

            site = siteResult.Value;
            await siteRepository.AddAsync(site, cancellationToken);
        }

        var joinResult = ring.RequestJoin(site.Id, request.ApplicantName, request.ContactEmail);
        if (joinResult.IsFailure)
            throw new DomainException(joinResult.Error!);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var membership = joinResult.Value;
        string snippetHtml = BuildSnippetHtml(site.Id.Value);
        string? checkSnippetUrl = null;

        if (membership.Status == MembershipStatus.PendingVerification)
        {
            checkSnippetUrl = $"/api/rings/{ring.Id.Value}/memberships/{membership.Id.Value}/check-snippet";
        }
        return new SubmitApplicationResponse(
            membership.Id.Value,
            site.Id.Value,
            membership.Status,
            snippetHtml,
            checkSnippetUrl);
    }

    private static string BuildSnippetHtml(Guid siteId) =>
        $"""<script src="https://orbital.app/widget.js" data-id="{siteId}" async></script>""";
}
