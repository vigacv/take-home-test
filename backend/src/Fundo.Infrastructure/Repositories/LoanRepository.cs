using Fundo.Domain.Entities;
using Fundo.Domain.Repositories;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories;

public class LoanRepository(FundoDbContext context) : ILoanRepository
{
    public async Task<IEnumerable<Loan>> GetAllAsync()
        => await context.Loans.AsNoTracking().ToListAsync();

    public async Task<Loan?> GetByIdAsync(int id)
        => await context.Loans.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
}
