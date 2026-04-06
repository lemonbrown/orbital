using MediatR;

namespace Orbital.Application.Rings.Commands.UpdateActivityConfig;

public sealed record UpdateActivityConfigCommand(
    Guid RingId,
    bool IsEnabled,
    bool CrawlingEnabled,
    decimal RecentPostWeight,
    decimal RecentUpdateWeight,
    int ActivityWindowDays,
    string CrawlFrequency,
    bool SkipStaleSites,
    int StaleSiteThresholdDays) : IRequest;
