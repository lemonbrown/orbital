using MediatR;

namespace Orbital.Application.Rings.Queries.GetApiKeys;

public sealed record GetApiKeysQuery(Guid RingId) : IRequest<IReadOnlyList<ApiKeyDto>>;
