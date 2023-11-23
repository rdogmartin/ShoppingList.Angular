using System.Security.Claims;

namespace Api.Dto;

/// <summary>
/// Contains authentication information
/// </summary>
public record class AuthResult(ClaimsPrincipal User)
{
    public bool IsAuthenticated { get; } = User.Identity?.IsAuthenticated ?? false;
}
