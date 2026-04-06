namespace Orbital.Application.Rings.Queries.GetApiKeys;

public sealed record ApiKeyDto(
    Guid Id,
    string Label,
    string KeyPrefix,
    DateTime CreatedAt,
    bool IsRevoked,
    DateTime? RevokedAt);
