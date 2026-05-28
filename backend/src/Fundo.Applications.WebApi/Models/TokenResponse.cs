namespace Fundo.Applications.WebApi.Models;

public sealed record TokenResponse(string AccessToken, int ExpiresInSeconds);
