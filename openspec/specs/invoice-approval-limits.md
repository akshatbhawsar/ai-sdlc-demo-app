# Feature: Role-based invoice approval limits

## Summary
Add role-based approval limits to the invoice approval service so managers and directors can approve only within the allowed thresholds.

## Business Rules
1. A Manager may approve invoices up to 50,000.
2. A Director may approve invoices up to 500,000.
3. Anything above 500,000 requires two distinct approvers.
4. The second approver must be a Director.
5. The same user may never count as both approvers on one invoice.
6. A rejected approval attempt returns a clear reason.
7. A rejected approval attempt leaves the invoice unchanged.
8. No extra business rules should be introduced beyond those specified in the assignment.

## Functional Requirements

### R1: Single-approver limits
- A Manager can approve an invoice of 50,000 or less.
- A Manager cannot approve an invoice above 50,000.
- A Director can approve an invoice of 500,000 or less.
- A Director cannot approve an invoice above 500,000 without a second approver.

### R2: Two-approver flow
- If the invoice amount is above 500,000, two approvers are required.
- The second approver must be a Director.
- The approvers must be different users.
- The same user cannot be used as both approvers for one invoice.

### R3: Rejected attempts are safe
- When a validation fails, the service must return a clear reason.
- A rejected approval attempt must not mutate the invoice.
- No partial approval state may be written on failure.

## Acceptance Criteria
- Manager approves 50,000 successfully.
- Manager rejects 50,001 with a clear reason.
- Director approves 500,000 successfully.
- Director cannot approve over 500,000 without a second approver.
- Two distinct approvers can approve above 500,000 only when the second approver is a Director.
- Same-user dual approval is rejected.
- Rejected approval does not change invoice state.
- Unit tests cover all above scenarios.
- One Playwright UI test covers a rejected over-limit approval.