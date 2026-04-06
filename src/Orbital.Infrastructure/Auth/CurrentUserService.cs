using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Orbital.Application.Common.Interfaces;
using Orbital.Domain.Users;

namespace Orbital.Infrastructure.Auth;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public UserId? UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return claim is not null && Guid.TryParse(claim, out var id)
                ? Domain.Users.UserId.From(id)
                : null;
        }
    }

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated is true;
}
