using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(Guid id);
    Task<Loan> CreateAsync(Loan loan);
    Task<Loan> UpdateAsync(Loan loan);
}
