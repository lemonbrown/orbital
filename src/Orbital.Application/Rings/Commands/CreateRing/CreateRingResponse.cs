namespace Orbital.Application.Rings.Commands.CreateRing;

public sealed record CreateRingResponse(Guid Id, string Name, string Slug, string Description);
