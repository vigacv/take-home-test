using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models;

public sealed record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
