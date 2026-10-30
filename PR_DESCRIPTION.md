# Pull Request Description

## Summary
This PR implements the invoice approval business rules in the domain layer, adds validation coverage for the approval limits, exercises the over-limit rejection flow through the UI, and records the AI-assisted workflow evidence required by the assignment.

## Review Findings
- Finding: Approval limits and role rules are implemented correctly.
  - Verdict: fixed
  - Reason: `backend/src/InvoiceApp.Domain/ApprovalService.cs` enforces the Manager limit at 50,000, the Director limit at 500,000, and the two-approver rule above 500,000 with the second approver required to be a Director.

- Finding: Duplicate approver reuse is blocked.
  - Verdict: fixed
  - Reason: The domain logic rejects the same user acting as both approvers on one invoice, and `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs` includes a negative test covering that rule.

- Finding: Invalid approval attempts do not mutate invoice state.
  - Verdict: fixed
  - Reason: The service rejects invalid attempts without changing status or approver fields, and the tests include a negative no-mutation check required by the assignment.

- Finding: Unit tests cover the required approval rules and negatives.
  - Verdict: fixed
  - Reason: `ApprovalServiceTests.cs` includes the Manager, Director, over-limit, duplicate-user, and no-mutation scenarios; the repo evidence shows the tests are present and passing.

- Finding: Playwright UI coverage for the rejected over-limit approval flow is present.
  - Verdict: fixed
  - Reason: `e2e/tests/invoice-approval-over-reject-limit.spec.ts` validates the over-limit rejection path through the UI and checks that the invoice remains pending after the failed approval.

- Finding: Mutation testing evidence is based on actual tool output.
  - Verdict: fixed
  - Reason: The repo contains the Stryker-generated report and `ASSIGNMENT.md` records a real mutation score of 55.66%, rather than assuming or inventing a value.

- Finding: AI workflow logging is present.
  - Verdict: fixed
  - Reason: `AI-INTRACTION.md` exists with structured entries, and the repo includes the supporting `.github/hooks` and prompt files for the required review/logging workflow.

- Finding: A live GitHub PR could not be opened directly from this environment.
  - Verdict: dismissed with a reason
  - Reason: This is an environment limitation because the GitHub CLI is not installed here; the review was still performed against the repo diff and actual project evidence in the workspace.

- Finding: The final demo video and final submission link are still missing.
  - Verdict: deferred with a reason
  - Reason: `ASSIGNMENT.md` explicitly states the video link is not recorded in this workspace environment, so the final assignment submission remains incomplete until the required MP4 is recorded and linked.

## Review state
The implementation and evidence are aligned with the assignment: approval thresholds, duplicate-user rejection, no-mutation behavior, unit tests, Playwright coverage, mutation evidence, and AI log evidence are all present. The remaining blocker is the final submission packaging: the required video and final link are still pending.

## Final submission status
Not ready for final submission yet. The code and review evidence are in good shape, but the required 8–10 minute MP4 demo must be recorded and linked before the repository is fully complete.
