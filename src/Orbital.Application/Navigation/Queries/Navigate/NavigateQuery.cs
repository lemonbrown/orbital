using MediatR;

namespace Orbital.Application.Navigation.Queries.Navigate;

public sealed record NavigateQuery(
    Guid RingId,
    Guid SiteId,
    string Dimension,
    string Direction) : IRequest<NavigateResult>;
