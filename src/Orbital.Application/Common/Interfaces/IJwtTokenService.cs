using Orbital.Domain.Users;

namespace Orbital.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
