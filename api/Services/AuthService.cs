using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Api.Dto;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace api.Services;

public interface IAuthService
{
    Task<AuthResult> Authenticate(HttpRequest request);
    AuthResult Authorize(HttpRequest request);
}

public class AuthService : IAuthService
{
    List<string> AuthorizedUsers => new List<string>() { "***REMOVED***", "***REMOVED***", "***REMOVED***" };

    public async Task<AuthResult> Authenticate(HttpRequest request)
    {
        var authResult = await AuthenticateUser(request);

        return authResult;
    }

    public AuthResult Authorize(HttpRequest request)
    {
        var authResult = AuthorizeUser(request);

        return authResult;
    }

    private async Task<AuthResult> AuthenticateUser(HttpRequest req)
    {

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var authRequest = JsonConvert.DeserializeObject<AuthRequest>(requestBody);

        if (authRequest != null && AuthorizedUsers.Contains(authRequest.UserName))
        {
            return new AuthResult(authRequest.UserName, true, Base64Encode(authRequest.UserName));
        }

        return new AuthResult(string.Empty, false, string.Empty);
    }


    private AuthResult AuthorizeUser(HttpRequest request)
    {
        if (request.Headers.TryGetValue("x-tis-auth-token", out var header))
        {
            var token = header[0];
            var userName = Base64Decode(token);

            if (AuthorizedUsers.Contains(userName))
            {
                return new AuthResult(userName, true, token);
            }
        }

        return new AuthResult(string.Empty, false, string.Empty);
    }

    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    private static string Base64Decode(string base64Text)
    {
        var decoded = Convert.FromBase64String(base64Text);
        return Encoding.UTF8.GetString(decoded);
    }
}
