using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.UpdateActivityConfig;

public sealed class UpdateActivityConfigCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateActivityConfigCommand>
{
    public async Task Handle(UpdateActivityConfigCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ring = await ringRepository.FindByIdAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        if (ring.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("Only the ring owner can update activity settings.");

        var frequency = Enum.Parse<CrawlFrequency>(request.CrawlFrequency);

        ring.ConfigureActivityTracking(RingActivityConfig.Create(
            request.IsEnabled,
            request.CrawlingEnabled,
            request.RecentPostWeight,
            request.RecentUpdateWeight,
            request.ActivityWindowDays,
            frequency,
            request.SkipStaleSites,
            request.StaleSiteThresholdDays));

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
