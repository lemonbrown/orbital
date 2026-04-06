using MediatR;
using Orbital.Application.Common.Exceptions;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.UpdateRingSettings;

public sealed class UpdateRingSettingsCommandHandler(
    IRingRepository ringRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateRingSettingsCommand>
{
    public async Task Handle(UpdateRingSettingsCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException();

        var ring = await ringRepository.FindByIdAsync(RingId.From(request.RingId), cancellationToken)
            ?? throw NotFoundException.For("Ring", request.RingId);

        if (ring.OwnerUserId != currentUserService.UserId!.Value)
            throw new UnauthorizedException("Only the ring owner can update settings.");

        ring.UpdateSettings(request.IsPublicJoinEnabled, request.IsApiOnboardingEnabled, request.IsPublicDirectoryEnabled, request.VerificationMode, request.ApprovalMode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
