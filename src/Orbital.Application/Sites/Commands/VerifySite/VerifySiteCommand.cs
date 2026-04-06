using MediatR;

namespace Orbital.Application.Sites.Commands.VerifySite;

public sealed record VerifySiteCommand(Guid SiteId) : IRequest<VerifySiteResponse>;
