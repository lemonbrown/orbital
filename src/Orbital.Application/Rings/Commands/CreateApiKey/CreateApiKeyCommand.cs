using MediatR;

namespace Orbital.Application.Rings.Commands.CreateApiKey;

public sealed record CreateApiKeyCommand(Guid RingId, string Label) : IRequest<CreateApiKeyResponse>;
