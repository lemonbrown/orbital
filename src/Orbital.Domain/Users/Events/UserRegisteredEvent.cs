using Orbital.Domain.Common;

namespace Orbital.Domain.Users.Events;

public sealed record UserRegisteredEvent(UserId UserId, string Username, string Email) : IDomainEvent;
