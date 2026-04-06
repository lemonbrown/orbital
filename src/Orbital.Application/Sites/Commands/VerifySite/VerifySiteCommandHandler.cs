using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Sites;

namespace Orbital.Application.Sites.Commands.VerifySite;

public sealed class VerifySiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    ISiteVerificationService verificationService)
    : IRequestHandler<VerifySiteCommand, VerifySiteResponse>
{
    public async Task<VerifySiteResponse> Handle(VerifySiteCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var siteId = SiteId.From(request.SiteId);
        var site = await siteRepository.FindByIdAsync(siteId, cancellationToken)
            ?? throw NotFoundException.For("Site", request.SiteId);

        if (site.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("You do not own this site.");

        var verified = await verificationService.VerifyAsync(site.Url, site.VerificationToken, cancellationToken);

        if (verified)
        {
            site.MarkVerified();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new VerifySiteResponse(true, SiteVerificationStatus.Verified, "Site verified successfully.");
        }
        else
        {
            site.MarkFailed();
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new VerifySiteResponse(false, SiteVerificationStatus.Failed,
                $"Verification meta tag not found. Add <meta name=\"orbital-verify\" content=\"{site.VerificationToken.Value}\"> to your site's <head>.");
        }
    }
}
