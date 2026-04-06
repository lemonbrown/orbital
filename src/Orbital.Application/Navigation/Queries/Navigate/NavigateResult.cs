namespace Orbital.Application.Navigation.Queries.Navigate;

public sealed record NavigateResult(
    Guid TargetSiteId,
    string TargetSiteUrl,
    string TargetSiteName,
    string Dimension,
    string Direction);
