using Fundo.Applications.WebApi.Models;
using Fundo.Applications.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fundo.Applications.WebApi.Controllers;

[ApiController]
[Route("loans")]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class LoanManagementController(ILoanService loanService, ILogger<LoanManagementController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<LoanResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetAll()
    {
        var loans = await loanService.GetAllAsync();
        return Ok(loans);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LoanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> GetById(Guid id)
    {
        var loan = await loanService.GetByIdAsync(id);
        if (loan is null)
        {
            logger.LogWarning("Loan {LoanId} not found", id);
            return NotFound();
        }
        return Ok(loan);
    }

    [HttpPost]
    [ProducesResponseType<LoanResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoanResponse>> Create([FromBody] CreateLoanRequest request)
    {
        var loan = await loanService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
    }

    [HttpPost("{id:guid}/payment")]
    [ProducesResponseType<LoanResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanResponse>> MakePayment(Guid id, [FromBody] MakePaymentRequest request)
    {
        try
        {
            var loan = await loanService.MakePaymentAsync(id, request);
            if (loan is null)
            {
                logger.LogWarning("Loan {LoanId} not found for payment", id);
                return NotFound();
            }
            return Ok(loan);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Payment failed for loan {LoanId}: {Message}", id, ex.Message);
            return BadRequest(new ErrorResponse(ex.Message));
        }
    }
}
