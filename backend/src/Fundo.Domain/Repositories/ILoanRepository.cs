using Fundo.Domain.Entities;

namespace Fundo.Domain.Repositories;

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(int id);
}
