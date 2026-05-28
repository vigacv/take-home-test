using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Fundo.Applications.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Fundo.Services.Tests.Integration;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbName = Guid.NewGuid().ToString();
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, cfg) =>
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Testing:DbName"] = dbName
            }));
    }
}

public class LoanManagementControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoanManagementControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithSeededLoans()
    {
        var response = await _client.GetAsync("/loans");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loans = await response.Content.ReadFromJsonAsync<List<LoanResponse>>();
        loans.Should().HaveCountGreaterThanOrEqualTo(5);
        loans.Should().Contain(l => l.ApplicantName == "Maria Silva");
        loans.Should().Contain(l => l.ApplicantName == "João Santos");
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnLoan()
    {
        var all = await (await _client.GetAsync("/loans")).Content.ReadFromJsonAsync<List<LoanResponse>>();
        var firstId = all!.First().Id;

        var response = await _client.GetAsync($"/loans/{firstId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loan = await response.Content.ReadFromJsonAsync<LoanResponse>();
        loan!.Id.Should().Be(firstId);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync($"/loans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnCreatedLoan()
    {
        var request = new CreateLoanRequest { Amount = 1500m, ApplicantName = "Test User" };

        var response = await _client.PostAsJsonAsync("/loans", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var loan = await response.Content.ReadFromJsonAsync<LoanResponse>();
        loan!.Amount.Should().Be(1500m);
        loan.CurrentBalance.Should().Be(1500m);
        loan.Status.Should().Be("active");
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_WithMissingApplicantName_ShouldReturnBadRequest()
    {
        var request = new { amount = 1000 };

        var response = await _client.PostAsJsonAsync("/loans", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakePayment_WithValidPayment_ShouldReduceBalance()
    {
        var createRequest = new CreateLoanRequest { Amount = 1000m, ApplicantName = "Payment Test" };
        var createResponse = await _client.PostAsJsonAsync("/loans", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

        var paymentRequest = new MakePaymentRequest { PaymentAmount = 400m };
        var paymentResponse = await _client.PostAsJsonAsync($"/loans/{created!.Id}/payment", paymentRequest);

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await paymentResponse.Content.ReadFromJsonAsync<LoanResponse>();
        updated!.CurrentBalance.Should().Be(600m);
        updated.Status.Should().Be("active");
    }

    [Fact]
    public async Task MakePayment_WithFullPayment_ShouldSetStatusToPaid()
    {
        var createRequest = new CreateLoanRequest { Amount = 500m, ApplicantName = "Full Payment Test" };
        var createResponse = await _client.PostAsJsonAsync("/loans", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

        var paymentRequest = new MakePaymentRequest { PaymentAmount = 500m };
        var paymentResponse = await _client.PostAsJsonAsync($"/loans/{created!.Id}/payment", paymentRequest);

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await paymentResponse.Content.ReadFromJsonAsync<LoanResponse>();
        updated!.CurrentBalance.Should().Be(0m);
        updated.Status.Should().Be("paid");
    }

    [Fact]
    public async Task MakePayment_WhenOverpaying_ShouldReturnBadRequest()
    {
        var createRequest = new CreateLoanRequest { Amount = 300m, ApplicantName = "Overpay Test" };
        var createResponse = await _client.PostAsJsonAsync("/loans", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

        var paymentRequest = new MakePaymentRequest { PaymentAmount = 999m };
        var paymentResponse = await _client.PostAsJsonAsync($"/loans/{created!.Id}/payment", paymentRequest);

        paymentResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MakePayment_WithInvalidId_ShouldReturnNotFound()
    {
        var paymentRequest = new MakePaymentRequest { PaymentAmount = 100m };
        var response = await _client.PostAsJsonAsync($"/loans/{Guid.NewGuid()}/payment", paymentRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
