using Fundo.Domain.Entities;

namespace Fundo.Applications.WebApi.Models;

public sealed record LoanResponse(Guid Id, decimal Amount,
    decimal CurrentBalance, string ApplicantName,
    string Status)
{
    public static LoanResponse FromDomain(Loan loan) =>
        new(loan.Id, loan.Amount, loan.CurrentBalance, loan.ApplicantName, loan.Status.ToString().ToLower());
}
