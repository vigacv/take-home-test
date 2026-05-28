using System.ComponentModel.DataAnnotations;

namespace Fundo.Applications.WebApi.Models;

public sealed record MakePaymentRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than zero.")]
    public decimal PaymentAmount { get; init; }
}
