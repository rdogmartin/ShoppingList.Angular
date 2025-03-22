namespace Api.Dto;

/// <summary>
/// Contains authentication information
/// </summary>
public record class AuthResult(string UserName, bool IsAuthenticated, string Token) { }
