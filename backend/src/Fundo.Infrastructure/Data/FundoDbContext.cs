using Fundo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fundo.Infrastructure.Data;

public class FundoDbContext(DbContextOptions<FundoDbContext> options) : DbContext(options)
{
    public DbSet<Loan> Loans => Set<Loan>();

    public static readonly IReadOnlyList<Loan> SeedLoans =
    [
        new Loan { Id = new Guid("a1b2c3d4-0001-0000-0000-000000000001"), Amount = 1000.00m, CurrentBalance = 1000.00m, ApplicantName = "Maria Silva", Status = LoanStatus.Active },
        new Loan { Id = new Guid("a1b2c3d4-0002-0000-0000-000000000002"), Amount = 2500.00m, CurrentBalance = 0.00m, ApplicantName = "João Santos", Status = LoanStatus.Paid },
        new Loan { Id = new Guid("a1b2c3d4-0003-0000-0000-000000000003"), Amount = 5000.00m, CurrentBalance = 5000.00m, ApplicantName = "Ana Oliveira", Status = LoanStatus.Active },
        new Loan { Id = new Guid("a1b2c3d4-0004-0000-0000-000000000004"), Amount = 750.00m, CurrentBalance = 0.00m, ApplicantName = "Carlos Pereira", Status = LoanStatus.Paid },
        new Loan { Id = new Guid("a1b2c3d4-0005-0000-0000-000000000005"), Amount = 3200.00m, CurrentBalance = 1600.00m, ApplicantName = "Fernanda Lima", Status = LoanStatus.Active },
    ];

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Id)
                .HasDefaultValueSql("newsequentialid()")
                .ValueGeneratedOnAdd();
            entity.Property(l => l.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(l => l.CurrentBalance).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(l => l.ApplicantName).IsRequired().HasMaxLength(200);
            entity.Property(l => l.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.HasData(SeedLoans);
        });
    }
}
