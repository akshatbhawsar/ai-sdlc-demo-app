# Assignment Summary

## Overview
This project implements the invoice approval business rules in the domain layer, adds validation coverage for the approval limits, exercises the over-limit rejection flow through the UI, and records the AI-assisted workflow evidence required by the assignment.

## Business Rules Implemented
- A Manager may approve invoices up to 50,000.
- A Director may approve invoices up to 500,000.
- Amounts above 500,000 require two distinct approvers.
- The second approver must be a Director.
- The same user may never count as both approvers on one invoice.
- A rejected approval attempt returns a clear reason and leaves the invoice unchanged.

## Files Updated
- `backend/src/InvoiceApp.Domain/ApprovalService.cs`
- `backend/tests/InvoiceApp.Tests/ApprovalServiceTests.cs`
- `e2e/tests/invoice-approval-over-reject-limit.spec.ts`
- `e2e/playwright.config.ts`
- `openspec/specs/invoice-approval-limits.md`
- `openspec/specs/invoice-approval-limits.tasks.md`
- `AGENTS.md`
- `AI-INTRACTION.md`
- `.github/hooks/run-with-ai-log.ps1`
- `.github/hooks/log-ai-pr-mutation.ps1`

## Validation Evidence
Actual validation commands run and results recorded:

- `dotnet test backend/InvoiceApp.sln --nologo`
  - Result: 13 passed, 0 failed

- Mutation testing was executed with the required Stryker version for .NET:
  - `dotnet tool install -g dotnet-stryker --version 4.13.0`
  - Run from `backend/` using `dotnet stryker --reporter json --reporter html --output StrykerOutput`
  - Result: actual report generated successfully
  - Actual mutation score recorded: 55.66%

- Playwright scenario for the rejected over-limit approval path was implemented and corrected to target the correct draft invoice row before approval submission.
  - The repo is configured to use the installed Microsoft Edge browser because Chromium download is blocked in the company environment.
  - The scenario covers the required UI rejection flow using the project’s browser-based validation path.

## Mutation Testing Results
Actual Stryker output used for reporting:

- Before change: no recorded mutation score found in the current workspace evidence.
- After change: 55.66%
- Mutants created: 130
- Mutants tested: 73
- Mutants skipped because of NoCoverage or Ignored: 57

Generated reports:
- `backend/StrykerOutput/reports/mutation-report.json`
- `backend/StrykerOutput/reports/mutation-report.html`

## Final Submission Status
The following items remain pending for completion:
- Open and finalize the PR on the fork.
- Ensure the commit history includes at least three meaningful standalone commits.
- Add the final PR description with every review finding and verdict.
- Record the 8–10 minute MP4 demo video, upload it, and insert the final link.
- Include the final video URL in this file and in the PR description.

## AI Interaction Log
The assignment requires a log of AI-assisted activity. The repository includes a structured workflow log at `AI-INTRACTION.md`.

This log captures structured evidence for validation and review workflow events in JSON form, including:
- timestamp
- stage
- agent and model
- request/context
- tools used
- status
- artifact references

## PR Review Findings
The findings below list the main issues observed during review and record the verdict for each one.

1. The Playwright selector originally targeted the first row matching the customer name rather than the newly created draft invoice.
   - Verdict: fixed
   - Reason: the selector now filters the correct draft row before submitting for approval.

2. The repository needed a structured AI interaction log.
   - Verdict: fixed
   - Reason: `AI-INTRACTION.md` and the hook scripts in `.github/hooks` provide the workflow log evidence required by the assignment.

3. Mutation testing required actual evidence from the tool output.
   - Verdict: fixed
   - Reason: the real Stryker mutation report was generated and the score was taken from the report instead of being assumed.

4. The assignment required a negative test proving that the same user cannot count as both approvers.
   - Verdict: fixed
   - Reason: the duplicate-user rejection scenario is covered in `ApprovalServiceTests`.

5. The assignment required a negative test proving that a rejected approval does not mutate the invoice.
   - Verdict: fixed
   - Reason: the approval logic and tests verify that invalid attempts leave the invoice state unchanged.

## Demo
Demo video link: not recorded in this workspace environment.

The project includes the required validation evidence, AI workflow log, and mutation report, but the final MP4 demo asset remains to be recorded and linked before final submission.

## Notes
- The implementation was staged as required: single-approver rules first, then the two-approver rule.
- The domain layer remains the single source of business rules.
- The approval logic remains out of controllers and startup code, as required by the project conventions.
- No real credentials or production data were introduced.