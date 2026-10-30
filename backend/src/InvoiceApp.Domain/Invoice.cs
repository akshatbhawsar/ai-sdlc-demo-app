// namespace InvoiceApp.Domain;

// public class Invoice
// {
//     public string Id { get; set; } = string.Empty;
//     public string CustomerName { get; set; } = string.Empty;
//     public decimal Amount { get; set; }
//     public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     public string? DecidedBy { get; set; }
//     public DateTime? DecidedAt { get; set; }
//     public string? DecisionNote { get; set; }
// }

namespace InvoiceApp.Domain;

public class Invoice
{
    public string Id { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // First approval for invoices requiring two approvers.
    public string? FirstApprover { get; set; }
    public string? FirstApproverRole { get; set; }
    public DateTime? FirstApprovedAt { get; set; }

    // Final decision details.
    public string? DecidedBy { get; set; }
    public string? DecidedByRole { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? DecisionNote { get; set; }
}
