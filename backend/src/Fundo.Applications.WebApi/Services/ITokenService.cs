using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Services;

public interface ITokenService
{
    TokenResponse GenerateToken(string username);
}
