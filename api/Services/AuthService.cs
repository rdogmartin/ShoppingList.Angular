using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using api.Dto;
using Api.Dto;
using Microsoft.AspNetCore.Http;

namespace api.Services;

public interface IAuthService
{
    AuthResult Authorize(HttpRequest request);
}

public class AuthService : IAuthService
{
    public AuthResult Authorize(HttpRequest request)
    {
        var user = ParseUser(request);

        return new AuthResult(user);
    }

    private static ClaimsPrincipal ParseUser(HttpRequest req)
    {
        ClientPrincipal principal = new(String.Empty, String.Empty, String.Empty, Array.Empty<string>());

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        principal = principal with { UserRoles = principal.UserRoles?.Except(new string[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase) };

        if (!principal.UserRoles.Any())
        {
            return new ClaimsPrincipal();
        }

        var identity = new ClaimsIdentity(principal.IdentityProvider);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
        identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
        identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        return new ClaimsPrincipal(identity);
    }
}
