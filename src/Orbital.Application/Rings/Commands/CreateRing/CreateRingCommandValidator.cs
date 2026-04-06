using FluentValidation;

namespace Orbital.Application.Rings.Commands.CreateRing;

public sealed class CreateRingCommandValidator : AbstractValidator<CreateRingCommand>
{
    public CreateRingCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
        RuleFor(x => x.OwnerSiteId).NotEmpty();
    }
}
