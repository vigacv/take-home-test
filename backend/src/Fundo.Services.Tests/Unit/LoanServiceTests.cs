using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Fundo.Applications.WebApi.Models;
using Fundo.Applications.WebApi.Services;
using Fundo.Domain.Entities;
using Fundo.Domain.Repositories;
using Moq;
using Xunit;

namespace Fundo.Services.Tests.Unit;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _repositoryMock = new();
    private readonly LoanService _sut;

    public LoanServiceTests()
    {
        _sut = new LoanService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedLoans()
    {
        var loans = new[]
        {
            new Loan { Id = Guid.NewGuid(), Amount = 1000m, CurrentBalance = 1000m, ApplicantName = "Alice", Status = LoanStatus.Active },
            new Loan { Id = Guid.NewGuid(), Amount = 500m, CurrentBalance = 0m, ApplicantName = "Bob", Status = LoanStatus.Paid }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(loans);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Status.Should().Be("active");
        result[1].Status.Should().Be("paid");
    }

    [Fact]
    public async Task GetByIdAsync_WhenLoanExists_ShouldReturnResponse()
    {
        var id = Guid.NewGuid();
        var loan = new Loan { Id = id, Amount = 1000m, CurrentBalance = 600m, ApplicantName = "Alice", Status = LoanStatus.Active };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(loan);

        var result = await _sut.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.CurrentBalance.Should().Be(600m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLoanNotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Loan?)null);

        var result = await _sut.GetByIdAsync(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateLoanWithActiveStatusAndFullBalance()
    {
        var request = new CreateLoanRequest { Amount = 2000m, ApplicantName = "Carol" };
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Loan>()))
            .ReturnsAsync((Loan l) =>
            {
                var loan = new Loan(l.Amount, l.ApplicantName) { Id = l.Id };
                return loan;
            });

        var result = await _sut.CreateAsync(request);

        result.Amount.Should().Be(2000m);
        result.CurrentBalance.Should().Be(2000m);
        result.Status.Should().Be("active");
        result.ApplicantName.Should().Be("Carol");
    }

    [Fact]
    public async Task MakePaymentAsync_WhenLoanNotFound_ShouldReturnNull()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Loan?)null);

        var result = await _sut.MakePaymentAsync(id, new MakePaymentRequest { PaymentAmount = 100m });

        result.Should().BeNull();
    }

    [Fact]
    public async Task MakePaymentAsync_WithPartialPayment_ShouldReduceBalance()
    {
        var id = Guid.NewGuid();
        var loan = new Loan { Id = id, Amount = 1000m, CurrentBalance = 1000m, ApplicantName = "Alice", Status = LoanStatus.Active };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(loan);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Loan>())).ReturnsAsync((Loan l) => l);

        var result = await _sut.MakePaymentAsync(id, new MakePaymentRequest { PaymentAmount = 400m });

        result!.CurrentBalance.Should().Be(600m);
        result.Status.Should().Be("active");
    }

    [Fact]
    public async Task MakePaymentAsync_WithFullPayment_ShouldSetStatusToPaid()
    {
        var id = Guid.NewGuid();
        var loan = new Loan { Id = id, Amount = 1000m, CurrentBalance = 500m, ApplicantName = "Alice", Status = LoanStatus.Active };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(loan);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Loan>())).ReturnsAsync((Loan l) => l);

        var result = await _sut.MakePaymentAsync(id, new MakePaymentRequest { PaymentAmount = 500m });

        result!.CurrentBalance.Should().Be(0m);
        result.Status.Should().Be("paid");
    }

    [Fact]
    public async Task MakePaymentAsync_WhenPaymentExceedsBalance_ShouldThrowInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var loan = new Loan { Id = id, Amount = 1000m, CurrentBalance = 200m, ApplicantName = "Alice", Status = LoanStatus.Active };
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(loan);

        var act = async () => await _sut.MakePaymentAsync(id, new MakePaymentRequest { PaymentAmount = 500m });

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*exceeds current balance*");
    }
}
