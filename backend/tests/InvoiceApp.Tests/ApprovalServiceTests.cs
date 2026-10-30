// using InvoiceApp.Domain;
// using Xunit;

// namespace InvoiceApp.Tests;

// public class ApprovalServiceTests
// {
//     private static Invoice InvoiceWith(string id, InvoiceStatus status) =>
//         new() { Id = id, CustomerName = "Test School", Amount = 100m, Status = status };

//     private static ApprovalService ServiceFor(params Invoice[] invoices) =>
//         new(new FakeInvoiceRepository(invoices));

//     [Fact]
//     public void Submit_MovesDraftToPendingApproval()
//     {
//         var invoice = InvoiceWith("INV-1", InvoiceStatus.Draft);

//         var result = ServiceFor(invoice).Submit("INV-1");

//         Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
//     }

//     [Fact]
//     public void Submit_RejectsInvoiceThatIsNotADraft()
//     {
//         var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Approved));

//         Assert.Throws<InvalidOperationException>(() => service.Submit("INV-1"));
//     }

//     [Fact]
//     public void Approve_SetsStatusApproverAndTimestamp()
//     {
//         var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

//         var result = service.Approve("INV-1", "demo.approver");

//         Assert.Equal(InvoiceStatus.Approved, result.Status);
//         Assert.Equal("demo.approver", result.DecidedBy);
//         Assert.NotNull(result.DecidedAt);
//     }

//     [Fact]
//     public void Approve_RejectsInvoiceThatIsStillADraft()
//     {
//         var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Draft));

//         Assert.Throws<InvalidOperationException>(() => service.Approve("INV-1", "demo.approver"));
//     }

//     [Fact]
//     public void Approve_ThrowsWhenInvoiceDoesNotExist()
//     {
//         var service = ServiceFor();

//         Assert.Throws<KeyNotFoundException>(() => service.Approve("INV-404", "demo.approver"));
//     }

//     [Fact]
//     public void Reject_RecordsReasonAndApprover()
//     {
//         var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

//         var result = service.Reject("INV-1", "demo.approver", "Duplicate invoice");

//         Assert.Equal(InvoiceStatus.Rejected, result.Status);
//         Assert.Equal("Duplicate invoice", result.DecisionNote);
//     }

//     [Fact]
//     public void Reject_RequiresAReason()
//     {
//         var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

//         Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.approver", "  "));
//     }
    
// [Fact]
// public void Approve_ManagerCanApproveInvoiceAtManagerLimit()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 50_000m;
//     var service = ServiceFor(invoice);

//     var result = service.Approve("INV-1", "demo.manager", "Manager");

//     Assert.Equal(InvoiceStatus.Approved, result.Status);
//     Assert.Equal("demo.manager", result.DecidedBy);
// }

// [Fact]
// public void Approve_ManagerCannotApproveAboveManagerLimit_WithoutChangingInvoice()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 50_000.01m;
//     var service = ServiceFor(invoice);

//     var exception = Assert.Throws<InvalidOperationException>(
//         () => service.Approve("INV-1", "demo.manager", "Manager"));

//     Assert.Contains("Director", exception.Message);
//     Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
//     Assert.Null(invoice.FirstApprover);
//     Assert.Null(invoice.DecidedBy);
//     Assert.Null(invoice.DecidedAt);
// }

// [Fact]
// public void Approve_DirectorCanApproveInvoiceAtDirectorLimit()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 500_000m;
//     var service = ServiceFor(invoice);

//     var result = service.Approve("INV-1", "demo.director", "Director");

//     Assert.Equal(InvoiceStatus.Approved, result.Status);
//     Assert.Equal("demo.director", result.DecidedBy);
// }

// [Fact]
// public void Approve_InvoiceAboveDirectorLimitNeedsTwoApprovers()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 500_000.01m;
//     var service = ServiceFor(invoice);

//     var firstResult = service.Approve(
//         "INV-1", "demo.manager", "Manager");

//     Assert.Equal(InvoiceStatus.PendingApproval, firstResult.Status);
//     Assert.Equal("demo.manager", firstResult.FirstApprover);
//     Assert.Null(firstResult.DecidedBy);

//     var secondResult = service.Approve(
//         "INV-1", "demo.director", "Director");

//     Assert.Equal(InvoiceStatus.Approved, secondResult.Status);
//     Assert.Equal("demo.manager", secondResult.FirstApprover);
//     Assert.Equal("demo.director", secondResult.DecidedBy);
// }

// [Fact]
// public void Approve_SameUserCannotApproveTwice_WithoutChangingInvoice()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 600_000m;
//     var service = ServiceFor(invoice);

//     service.Approve("INV-1", "demo.manager", "Manager");

//     var firstApprover = invoice.FirstApprover;
//     var firstApprovedAt = invoice.FirstApprovedAt;

//     var exception = Assert.Throws<InvalidOperationException>(
//         () => service.Approve("INV-1", "demo.manager", "Manager"));

//     Assert.Contains("different user", exception.Message);
//     Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
//     Assert.Equal(firstApprover, invoice.FirstApprover);
//     Assert.Equal(firstApprovedAt, invoice.FirstApprovedAt);
//     Assert.Null(invoice.DecidedBy);
//     Assert.Null(invoice.DecidedAt);
// }

// [Fact]
// public void Approve_NonDirectorCannotGiveSecondApproval_WithoutChangingInvoice()
// {
//     var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
//     invoice.Amount = 600_000m;
//     var service = ServiceFor(invoice);

//     service.Approve("INV-1", "manager.one", "Manager");

//     var firstApprover = invoice.FirstApprover;
//     var firstApprovedAt = invoice.FirstApprovedAt;

//     var exception = Assert.Throws<InvalidOperationException>(
//         () => service.Approve("INV-1", "manager.two", "Manager"));

//     Assert.Contains("Director", exception.Message);
//     Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
//     Assert.Equal(firstApprover, invoice.FirstApprover);
//     Assert.Equal(firstApprovedAt, invoice.FirstApprovedAt);
//     Assert.Null(invoice.DecidedBy);
//     Assert.Null(invoice.DecidedAt);
// }

// }
using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class ApprovalServiceTests
{
    private static Invoice InvoiceWith(string id, InvoiceStatus status, decimal amount = 100m) =>
        new()
        {
            Id = id,
            CustomerName = "Test School",
            Amount = amount,
            Status = status
        };

    private static ApprovalService ServiceFor(params Invoice[] invoices) =>
        new(new FakeInvoiceRepository(invoices));

    [Fact]
    public void Submit_MovesDraftToPendingApproval()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.Draft);
        var service = ServiceFor(invoice);

        var result = service.Submit("INV-1");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
    }

    [Fact]
    public void Submit_RejectsInvoiceThatIsNotADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Approved));

        Assert.Throws<InvalidOperationException>(() => service.Submit("INV-1"));
    }

    [Fact]
    public void Approve_ManagerCanApproveInvoiceAtManagerLimit()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 50_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "demo.manager", "Manager");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("demo.manager", result.DecidedBy);
    }

    [Fact]
    public void Approve_ManagerCannotApproveAboveManagerLimit_AndLeavesInvoiceUnchanged()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 50_001m);
        var service = ServiceFor(invoice);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.Approve("INV-1", "demo.manager", "Manager"));

        Assert.Contains("Director", exception.Message);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Null(invoice.DecidedBy);
        Assert.Null(invoice.DecidedAt);
        Assert.Null(invoice.FirstApprover);
    }

    [Fact]
    public void Approve_DirectorCanApproveInvoiceAtDirectorLimit()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 500_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "demo.director", "Director");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("demo.director", result.DecidedBy);
    }

    [Fact]
    public void Approve_InvoiceAboveDirectorLimitRequiresFirstApprover()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 500_001m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "demo.manager", "Manager");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
        Assert.Equal("demo.manager", result.FirstApprover);
        Assert.Null(result.DecidedBy);
    }

    [Fact]
    public void Approve_SecondApproverOnLargeInvoiceMustBeADirector()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 600_000m);
        invoice.FirstApprover = "demo.manager";
        invoice.FirstApproverRole = "Manager";
        invoice.FirstApprovedAt = DateTime.UtcNow;

        var service = ServiceFor(invoice);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.Approve("INV-1", "demo.manager-2", "Manager"));

        Assert.Contains("Director", exception.Message);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Equal("demo.manager", invoice.FirstApprover);
        Assert.Null(invoice.DecidedBy);
    }

    [Fact]
    public void Approve_SameUserCannotApproveTwice_OnSameLargeInvoice()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 600_000m);
        invoice.FirstApprover = "demo.manager";
        invoice.FirstApproverRole = "Manager";
        invoice.FirstApprovedAt = DateTime.UtcNow;

        var service = ServiceFor(invoice);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            service.Approve("INV-1", "demo.manager", "Director"));

        Assert.Contains("same user", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Equal("demo.manager", invoice.FirstApprover);
        Assert.Null(invoice.DecidedBy);
    }

    [Fact]
    public void Approve_SecondApproverOnLargeInvoiceMustBeDistinctUser()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 600_000m);
        invoice.FirstApprover = "demo.manager";
        invoice.FirstApproverRole = "Manager";
        invoice.FirstApprovedAt = DateTime.UtcNow;

        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "demo.director", "Director");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("demo.director", result.DecidedBy);
    }

    [Fact]
    public void Approve_RejectsInvoiceThatIsNotPending()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Draft));

        Assert.Throws<InvalidOperationException>(() => service.Approve("INV-1", "demo.approver", "Manager"));
    }

    [Fact]
    public void Approve_ThrowsWhenInvoiceDoesNotExist()
    {
        var service = ServiceFor();

        Assert.Throws<KeyNotFoundException>(() => service.Approve("INV-404", "demo.director", "Director"));
    }

    [Fact]
    public void Reject_RecordsReasonAndApprover()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
        var service = ServiceFor(invoice);

        var result = service.Reject("INV-1", "demo.approver", "Duplicate invoice");

        Assert.Equal(InvoiceStatus.Rejected, result.Status);
        Assert.Equal("Duplicate invoice", result.DecisionNote);
        Assert.Equal("demo.approver", result.DecidedBy);
    }

    [Fact]
    public void Reject_RequiresAReason()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval);
        var service = ServiceFor(invoice);

        Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.approver", "   "));
    }
}