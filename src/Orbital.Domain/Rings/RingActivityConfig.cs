namespace Orbital.Domain.Rings;

public sealed class RingActivityConfig
{
    public bool IsEnabled { get; private set; }
    public bool CrawlingEnabled { get; private set; }
    public decimal RecentPostWeight { get; private set; }
    public decimal RecentUpdateWeight { get; private set; }
    public int ActivityWindowDays { get; private set; }
    public CrawlFrequency CrawlFrequency { get; private set; }
    public bool SkipStaleSites { get; private set; }
    public int StaleSiteThresholdDays { get; private set; }

    private RingActivityConfig() { }

    public static RingActivityConfig CreateDefault() => new()
    {
        IsEnabled = false,
        CrawlingEnabled = false,
        RecentPostWeight = 2.0m,
        RecentUpdateWeight = 1.5m,
        ActivityWindowDays = 30,
        CrawlFrequency = CrawlFrequency.Daily,
        SkipStaleSites = false,
        StaleSiteThresholdDays = 90
    };

    public static RingActivityConfig Create(
        bool isEnabled,
        bool crawlingEnabled,
        decimal recentPostWeight,
        decimal recentUpdateWeight,
        int activityWindowDays,
        CrawlFrequency crawlFrequency,
        bool skipStaleSites,
        int staleSiteThresholdDays) => new()
    {
        IsEnabled = isEnabled,
        CrawlingEnabled = crawlingEnabled,
        RecentPostWeight = recentPostWeight,
        RecentUpdateWeight = recentUpdateWeight,
        ActivityWindowDays = activityWindowDays,
        CrawlFrequency = crawlFrequency,
        SkipStaleSites = skipStaleSites,
        StaleSiteThresholdDays = staleSiteThresholdDays
    };
}
