using MediatR;
using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.UpdateRingSettings;

public sealed record UpdateRingSettingsCommand(
    Guid RingId,
    bool IsPublicJoinEnabled,
    bool IsApiOnboardingEnabled,
    bool IsPublicDirectoryEnabled,
    VerificationMode VerificationMode,
    ApprovalMode ApprovalMode) : IRequest;
