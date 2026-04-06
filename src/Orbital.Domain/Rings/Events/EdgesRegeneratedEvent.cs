using Orbital.Domain.Common;

namespace Orbital.Domain.Rings.Events;

public sealed record EdgesRegeneratedEvent(RingId RingId, string Dimension, int EdgeCount) : IDomainEvent;
