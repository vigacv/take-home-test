using Fundo.Applications.WebApi.Models;
using Fundo.Applications.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers;

[ApiController]
[Route("loans")]
public class LoanManagementController(ILoanService loanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetAll()
    {
        var loans = await loanService.GetAllAsync();
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanResponse>> GetById(Guid id)
    {
        var loan = await loanService.GetByIdAsync(id);
        return loan is null ? NotFound() : Ok(loan);
    }

    [HttpPost]
    public async Task<ActionResult<LoanResponse>> Create([FromBody] CreateLoanRequest request)
    {
        var loan = await loanService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPost("{id:guid}/payment")]
    public async Task<ActionResult<LoanResponse>> MakePayment(Guid id, [FromBody] MakePaymentRequest request)
    {
        try
        {
            var loan = await loanService.MakePaymentAsync(id, request);
            return loan is null ? NotFound() : Ok(loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
