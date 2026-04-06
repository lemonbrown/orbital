using MediatR;

namespace Orbital.Application.Rings.Commands.RevokeApiKey;

public sealed record RevokeApiKeyCommand(Guid RingId, Guid KeyId) : IRequest;
