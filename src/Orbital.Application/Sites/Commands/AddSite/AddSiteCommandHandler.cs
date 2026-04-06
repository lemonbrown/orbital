using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Sites;

namespace Orbital.Application.Sites.Commands.AddSite;

public sealed class AddSiteCommandHandler(
    ISiteRepository siteRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<AddSiteCommand, AddSiteResponse>
{
    public async Task<AddSiteResponse> Handle(AddSiteCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ownerId = currentUserService.UserId!.Value;

        var siteResult = Site.Create(ownerId, request.Name, request.Url, request.Description);
        if (siteResult.IsFailure)
            throw new DomainException(siteResult.Error!);

        var site = siteResult.Value;
        await siteRepository.AddAsync(site, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddSiteResponse(
            site.Id.Value,
            site.Name,
            site.Url.Value,
            site.Description,
            site.VerificationToken.Value,
            site.VerificationStatus);
    }
}
