using MediatR;
using Orbital.Application.Rings.Queries.GetRing;

namespace Orbital.Application.Rings.Queries.GetRingBySlug;

public sealed record GetRingBySlugQuery(string Slug) : IRequest<RingDetailDto>;
