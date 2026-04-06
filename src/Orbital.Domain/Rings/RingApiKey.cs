using System.Security.Cryptography;
using System.Text;
using Orbital.Domain.Common;

namespace Orbital.Domain.Rings;

public sealed class RingApiKey : Entity<RingApiKeyId>
{
    public RingId RingId { get; private set; }
    public string Label { get; private set; } = default!;
    public string KeyPrefix { get; private set; } = default!;
    public string KeyHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsRevoked => RevokedAt.HasValue;

    private RingApiKey() { }

    public static (RingApiKey Entity, string PlainKey) Generate(RingId ringId, string label)
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var keyBody = Convert.ToHexString(bytes).ToLowerInvariant();
        var plainKey = $"sk_{keyBody}";
        var prefix = plainKey[..16];
        var hash = ComputeHash(plainKey);

        var entity = new RingApiKey
        {
            Id = RingApiKeyId.New(),
            RingId = ringId,
            Label = label.Trim(),
            KeyPrefix = prefix,
            KeyHash = hash,
            CreatedAt = DateTime.UtcNow
        };

        return (entity, plainKey);
    }

    public bool Verify(string key) => !IsRevoked && ComputeHash(key) == KeyHash;

    public void Revoke() => RevokedAt = DateTime.UtcNow;

    private static string ComputeHash(string key)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
