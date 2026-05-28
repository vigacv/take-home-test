using Fundo.Domain.Entities;
using Fundo.Domain.Repositories;
using Fundo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Repositories;

public class LoanRepository(FundoDbContext context) : ILoanRepository
{
    public async Task<IEnumerable<Loan>> GetAllAsync()
        => await context.Loans.AsNoTracking().ToListAsync();

    public async Task<Loan?> GetByIdAsync(Guid id)
        => await context.Loans.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loan> CreateAsync(Loan loan)
    {
        context.Loans.Add(loan);
        await context.SaveChangesAsync();
        return loan;
    }

    public async Task<Loan> UpdateAsync(Loan loan)
    {
        context.Loans.Update(loan);
        await context.SaveChangesAsync();
        return loan;
    }
}
