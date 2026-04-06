using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;

namespace Orbital.Application.Sites.Queries.GetMySites;

public sealed class GetMySitesQueryHandler(
    ISiteRepository siteRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMySitesQuery, IReadOnlyList<SiteDto>>
{
    public async Task<IReadOnlyList<SiteDto>> Handle(GetMySitesQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var sites = await siteRepository.GetByOwnerAsync(currentUserService.UserId!.Value, cancellationToken);

        return sites.Select(s => new SiteDto(
            s.Id.Value,
            s.Name,
            s.Url.Value,
            s.Description,
            s.VerificationStatus,
            s.VerificationToken.Value,
            s.CreatedAt)).ToList();
    }
}
