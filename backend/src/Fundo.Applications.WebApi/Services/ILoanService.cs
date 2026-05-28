using Fundo.Applications.WebApi.Models;

namespace Fundo.Applications.WebApi.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanResponse>> GetAllAsync();
    Task<LoanResponse?> GetByIdAsync(Guid id);
    Task<LoanResponse> CreateAsync(CreateLoanRequest request);
    Task<LoanResponse?> MakePaymentAsync(Guid id, MakePaymentRequest request);
}
