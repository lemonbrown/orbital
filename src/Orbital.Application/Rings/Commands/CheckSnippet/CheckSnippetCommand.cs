using MediatR;

namespace Orbital.Application.Rings.Commands.CheckSnippet;

public sealed record CheckSnippetCommand(Guid RingId, Guid MembershipId) : IRequest<CheckSnippetResponse>;
