using Orbital.Domain.Users;

namespace Orbital.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(UserId id, CancellationToken ct = default);
    Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
}
