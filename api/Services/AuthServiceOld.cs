using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using api.Dto;
using Api.Dto;
using Microsoft.AspNetCore.Http;

namespace api.Services;

public interface IAuthServiceOld
{
    AuthResultOld Authorize(HttpRequest request);
}

public class AuthServiceOld : IAuthServiceOld
{
    public AuthResultOld Authorize(HttpRequest request)
    {
        var user = ParseUser(request);

        return new AuthResultOld(user);
    }

    private static ClaimsPrincipal ParseUser(HttpRequest req)
    {
        ClientPrincipal principal = new(String.Empty, String.Empty, String.Empty, Array.Empty<string>());

        if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
        {
            var data = header[0];
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidOperationException("Unable to parse the HTTP request header x-ms-client-principal into a ClientPrincipal instance.");
        }

        principal = principal with { UserRoles = principal.UserRoles.Except(new[] { "anonymous" }, StringComparer.OrdinalIgnoreCase) ?? throw new InvalidOperationException() };

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
