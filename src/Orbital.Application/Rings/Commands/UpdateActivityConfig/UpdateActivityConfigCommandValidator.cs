using FluentValidation;

namespace Orbital.Application.Rings.Commands.UpdateActivityConfig;

public sealed class UpdateActivityConfigCommandValidator : AbstractValidator<UpdateActivityConfigCommand>
{
    public UpdateActivityConfigCommandValidator()
    {
        RuleFor(x => x.RecentPostWeight)
            .InclusiveBetween(0m, 10m)
            .WithMessage("Post weight must be between 0 and 10.");

        RuleFor(x => x.RecentUpdateWeight)
            .InclusiveBetween(0m, 10m)
            .WithMessage("Update weight must be between 0 and 10.");

        RuleFor(x => x.ActivityWindowDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Activity window must be between 1 and 365 days.");

        RuleFor(x => x.CrawlFrequency)
            .Must(v => Enum.TryParse<Orbital.Domain.Rings.CrawlFrequency>(v, out _))
            .WithMessage("Invalid crawl frequency.");

        RuleFor(x => x.StaleSiteThresholdDays)
            .InclusiveBetween(1, 365)
            .WithMessage("Stale site threshold must be between 1 and 365 days.");
    }
}
