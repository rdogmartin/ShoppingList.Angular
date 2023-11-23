using System.Collections.Generic;

namespace api.Dto;

public record class ClientPrincipal(string IdentityProvider, string UserId, string UserDetails, IEnumerable<string> UserRoles)
{
}
