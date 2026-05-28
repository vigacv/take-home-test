using Fundo.Applications.WebApi.Models;
using Fundo.Domain.Entities;
using Fundo.Domain.Repositories;

namespace Fundo.Applications.WebApi.Services;

public class LoanService(ILoanRepository repository) : ILoanService
{
    public async Task<IEnumerable<LoanResponse>> GetAllAsync()
    {
        var loans = await repository.GetAllAsync();
        return loans.Select(LoanResponse.FromDomain);
    }

    public async Task<LoanResponse?> GetByIdAsync(Guid id)
    {
        var loan = await repository.GetByIdAsync(id);
        return loan is null ? null : LoanResponse.FromDomain(loan);
    }

    public async Task<LoanResponse> CreateAsync(CreateLoanRequest request)
    {
        var loan = request.ToDomain();

        var created = await repository.CreateAsync(loan);
        return LoanResponse.FromDomain(created);
    }

    public async Task<LoanResponse?> MakePaymentAsync(Guid id, MakePaymentRequest request)
    {
        var loan = await repository.GetByIdAsync(id);
        if (loan is null) return null;

        if (request.PaymentAmount > loan.CurrentBalance)
            throw new InvalidOperationException(
                $"Payment of {request.PaymentAmount:F2} exceeds current balance of {loan.CurrentBalance:F2}.");

        loan.CurrentBalance -= request.PaymentAmount;

        if (loan.CurrentBalance == 0)
            loan.Status = LoanStatus.Paid;

        var updated = await repository.UpdateAsync(loan);
        return LoanResponse.FromDomain(updated);
    }
}