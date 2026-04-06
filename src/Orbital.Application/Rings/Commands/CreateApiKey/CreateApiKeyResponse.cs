namespace Orbital.Application.Rings.Commands.CreateApiKey;

public sealed record CreateApiKeyResponse(Guid KeyId, string Label, string PlainKey, DateTime CreatedAt);
