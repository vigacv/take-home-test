using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Data;

public class FundoDbContext(DbContextOptions<FundoDbContext> options) : DbContext(options)
{
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(l => l.ApplicantName).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.HasData(
                new Loan { Id = 1, Amount = 1000.00m, ApplicantName = "Maria Silva", Status = LoanStatus.Active },
                new Loan { Id = 2, Amount = 2500.00m, ApplicantName = "João Santos", Status = LoanStatus.Paid },
                new Loan { Id = 3, Amount = 5000.00m, ApplicantName = "Ana Oliveira", Status = LoanStatus.Active },
                new Loan { Id = 4, Amount = 750.00m, ApplicantName = "Carlos Pereira", Status = LoanStatus.Paid },
                new Loan { Id = 5, Amount = 3200.00m, ApplicantName = "Fernanda Lima", Status = LoanStatus.Active }
            );
        });
    }
}
