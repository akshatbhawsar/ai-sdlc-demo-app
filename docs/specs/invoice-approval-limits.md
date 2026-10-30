# Invoice Approval Limits — Feature Specification

## 1. Objective

Add approval limits to the existing invoice approval application. Approval decisions must follow the approver's authority, and invoices must not change when an approval attempt is rejected.

## 2. Business Rules

### Rule 1: Manager approval limit
- A Manager can approve an invoice with an amount up to and including 50,000.
- A Manager cannot approve an invoice above 50,000.
- An unsuccessful attempt must return a clear explanation and leave the invoice unchanged.

### Rule 2: Director approval limit
- A Director can approve an invoice with an amount up to and including 500,000.
- A Director cannot approve an invoice above 500,000 using the single-approval process.
- An unsuccessful attempt must return a clear explanation and leave the invoice unchanged.

### Rule 3: Invoices above 500,000
- An invoice above 500,000 requires two distinct approvers.
- The second approver must be a Director.
- The same user cannot provide both approvals.
- The invoice must not become Approved until both valid approvals have been recorded.

### Rule 4: Preserve invoice state
- A rejected approval attempt must not change the invoice's approval status or recorded approvals.
- Existing invoice lifecycle rules must remain valid.
- Approval rules must be enforced in the Domain layer, not only in the UI or controller.

## 3. Acceptance Criteria

1. A Manager can approve an eligible invoice of 50,000.
2. A Manager's attempt to approve an invoice above 50,000 is rejected.
3. A Director can approve an eligible invoice of 500,000.
4. A Director's single-approval attempt for an invoice above 500,000 is rejected.
5. An invoice above 500,000 requires two different users.
6. The second approval for an invoice above 500,000 must come from a Director.
7. A user cannot approve the same invoice twice to satisfy the two-approver requirement.
8. Rejected attempts return a clear reason and do not mutate the invoice.
9. Existing functionality continues to work.
10. Unit tests cover the approval limits, boundary amounts, two-approver rules, and negative cases.
11. A Playwright E2E test demonstrates an over-limit approval being rejected through the UI.

## 4. Testing Strategy

- Use xUnit for Domain/business-rule tests.
- Use the existing FakeInvoiceRepository for tests where appropriate.
- Test the exact boundary amounts: 50,000 and 500,000, as well as amounts just above each limit.
- Test the same-user rejection and the requirement for a Director as the second approver.
- Verify that failed attempts leave the invoice unchanged.
- Use Playwright for at least one UI-level rejection test.
- Measure mutation-test scores before and after the implementation and document the results.

## 5. Design Constraints

- Keep approval business logic in InvoiceApp.Domain.
- Preserve the existing API and authentication conventions where possible.
- Inspect the existing role model before deciding how Manager and Director map to the current Approver and Admin roles.
- Do not add a database; the application currently uses in-memory storage.
- Do not claim tests or mutation scores passed unless the commands were actually run and their results recorded.

## 6. Review Expectations

- Review the specification before implementation.
- Use separate, meaningful commits for documentation/specification, single-approver limits, and two-approver enforcement.
- Review AI-generated changes for correctness and consistency with existing code.
- Record review findings and whether each was fixed, dismissed with a reason, or deferred with a reason.
- Keep an AI interaction log of important prompts, generated changes, and review decisions.