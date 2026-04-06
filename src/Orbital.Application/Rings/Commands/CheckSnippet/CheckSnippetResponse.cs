using Orbital.Domain.Rings;

namespace Orbital.Application.Rings.Commands.CheckSnippet;

public sealed record CheckSnippetResponse(bool Found, MembershipStatus Status, string Message);
