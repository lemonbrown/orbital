using FluentValidation;

namespace Orbital.Application.Sites.Commands.AddSite;

public sealed class AddSiteCommandValidator : AbstractValidator<AddSiteCommand>
{
    public AddSiteCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Url).NotEmpty().Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("URL must be a valid absolute URL.");
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}
