namespace InvoiceApp.Domain;

public class ApprovalService
{
    private const decimal ManagerLimit = 50_000m;
    private const decimal DirectorLimit = 500_000m;

    private readonly IInvoiceRepository _repository;

    public ApprovalService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public Invoice Submit(string invoiceId)
    {
        var invoice = Find(invoiceId);

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Invoice {invoiceId} is {invoice.Status}; only drafts can be submitted.");
        }

        invoice.Status = InvoiceStatus.PendingApproval;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Approve(string invoiceId, string approver, string role)
    {
        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        if (string.IsNullOrWhiteSpace(approver))
        {
            throw new InvalidOperationException("An approver username is required.");
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new InvalidOperationException("An approver role is required.");
        }

        if (role is not ("Manager" or "Director"))
        {
            throw new InvalidOperationException(
                "Only a Manager or Director may approve invoices.");
        }

        if (invoice.Amount <= 0)
        {
            throw new InvalidOperationException(
                "The invoice amount must be greater than zero.");
        }

        if (invoice.Amount <= ManagerLimit)
        {
            if (role != "Manager" && role != "Director")
            {
                throw new InvalidOperationException(
                    "Only a Manager or Director may approve invoices up to 50,000.");
            }

            RecordFinalApproval(invoice, approver, role);
            _repository.Update(invoice);
            return invoice;
        }

        if (invoice.Amount <= DirectorLimit)
        {
            if (role != "Director")
            {
                throw new InvalidOperationException(
                    "Invoices above 50,000 and up to 500,000 require a Director approval.");
            }

            RecordFinalApproval(invoice, approver, role);
            _repository.Update(invoice);
            return invoice;
        }

        if (string.IsNullOrWhiteSpace(invoice.FirstApprover))
        {
            if (role != "Manager" && role != "Director")
            {
                throw new InvalidOperationException(
                    "The first approver for invoices above 500,000 must be a Manager or Director.");
            }

            invoice.FirstApprover = approver;
            invoice.FirstApproverRole = role;
            invoice.FirstApprovedAt = DateTime.UtcNow;
            _repository.Update(invoice);
            return invoice;
        }

        if (string.Equals(invoice.FirstApprover, approver, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The same user may not count as both approvers on one invoice.");
        }

        if (role != "Director")
        {
            throw new InvalidOperationException(
                "The second approver for invoices above 500,000 must be a Director.");
        }

        RecordFinalApproval(invoice, approver, role);
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Reject(string invoiceId, string approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "A reason is required to reject an invoice.", nameof(reason));
        }

        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        invoice.Status = InvoiceStatus.Rejected;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.DecisionNote = reason.Trim();
        _repository.Update(invoice);
        return invoice;
    }

    private static void RecordFinalApproval(
        Invoice invoice,
        string approver,
        string role)
    {
        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = approver;
        invoice.DecidedByRole = role;
        invoice.DecidedAt = DateTime.UtcNow;
    }

    private Invoice Find(string invoiceId) =>
        _repository.GetById(invoiceId)
        ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");

    private static void EnsurePending(Invoice invoice)
    {
        if (invoice.Status != InvoiceStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                $"Invoice {invoice.Id} is {invoice.Status}; only pending invoices can be decided.");
        }
    }
}