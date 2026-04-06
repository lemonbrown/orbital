using Orbital.Domain.Users;

namespace Orbital.Application.Common.Interfaces;

public interface ICurrentUserService
{
    UserId? UserId { get; }
    bool IsAuthenticated { get; }
}
