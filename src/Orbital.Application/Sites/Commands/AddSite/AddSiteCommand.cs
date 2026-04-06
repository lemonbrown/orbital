using MediatR;

namespace Orbital.Application.Sites.Commands.AddSite;

public sealed record AddSiteCommand(string Name, string Url, string? Description) : IRequest<AddSiteResponse>;
