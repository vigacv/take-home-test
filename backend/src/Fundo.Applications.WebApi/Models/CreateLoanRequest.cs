using System.ComponentModel.DataAnnotations;
using Fundo.Domain.Entities;

namespace Fundo.Applications.WebApi.Models;

public sealed record CreateLoanRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; init; }

    [Required]
    [MaxLength(200)]
    public string ApplicantName { get; init; } = string.Empty;

    public Loan ToDomain() => new(Amount, ApplicantName);
}
