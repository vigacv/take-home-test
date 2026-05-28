namespace Fundo.Domain.Entities;

public class Loan
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal CurrentBalance { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public LoanStatus Status { get; set; }

    public Loan(decimal amount, string applicantName)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(applicantName)) throw new ArgumentException("Applicant name is required.", nameof(applicantName));
        Amount = amount;
        ApplicantName = applicantName;
        CurrentBalance = amount;
        Status = LoanStatus.Active;
    }

    public Loan() { }
}
