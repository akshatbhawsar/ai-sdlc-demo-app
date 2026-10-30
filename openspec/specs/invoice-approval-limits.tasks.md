# Tasks: Role-based invoice approval limits

## Stage 1: Single-approver limits
1. Review the current invoice approval service and determine the validation point for approval rules.
2. Add Manager approval limit logic so managers can approve invoices up to 50,000.
3. Add Director approval limit logic so directors can approve invoices up to 500,000.
4. Return a clear rejection reason when a user exceeds their allowed approval amount.
5. Ensure a rejected approval attempt does not mutate invoice state.
6. Write unit tests for:
   - Manager approval at 50,000
   - Manager rejection above 50,000
   - Director approval at 500,000
   - Director rejection above 500,000 when no second approver is present
   - Rejected approval leaves invoice state unchanged

## Stage 2: Two-approver rule
1. Add logic for invoice values above 500,000 requiring two approvers.
2. Validate that the second approver is a Director.
3. Validate that the approvers are different users.
4. Reject same-user approval on the same invoice.
5. Add unit tests for:
   - same user used twice is rejected
   - second approver is not a Director
   - rejected attempt does not mutate invoice
6. Add one Playwright end-to-end test for a rejected over-limit approval through the UI.

## Validation
- Run the unit tests for the approval logic.
- Run the Playwright test for the rejected over-limit approval flow.
- Run mutation testing and record the actual scores before and after the fix.
- Review the pull request with the workshop review prompt.
- In the PR description, list every review finding with one of these verdicts:
  - fixed
  - dismissed with a reason
  - deferred with a reason