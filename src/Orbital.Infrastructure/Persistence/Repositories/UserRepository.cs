using Microsoft.EntityFrameworkCore;
using Orbital.Domain.Interfaces;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(OrbitalDbContext db) : IUserRepository
{
    public Task<User?> FindByIdAsync(UserId id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByEmailAsync(Email email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value, ct);

    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Username == username, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);
}
