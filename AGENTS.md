# Project conventions for AI agents

Invoice approval demo app: ASP.NET Core 8 Web API with a small static web UI.

## Layout

- `backend/src/InvoiceApp.Domain`: business rules, no web or storage code. `ApprovalService` owns the invoice lifecycle (Draft, PendingApproval, Approved, Rejected).
- `backend/src/InvoiceApp.Api`: controllers, sign-in (`Auth/`), in-memory repository, and the web UI under `wwwroot/`.
- `backend/tests/InvoiceApp.Tests`: xUnit tests, one test class per class under test, named `<ClassUnderTest>Tests`.
- `e2e`: Playwright end-to-end tests (`tests/`) and page objects (`pages/`).
- `docs/use-cases`: short briefs for features we may build.

## Conventions

- Controllers live in `Controllers/`, one per resource, route `api/[controller]`. Request bodies are `record` types declared inside the controller.
- Controllers inherit `ApiControllerBase` and use `Handle(...)` so service errors map to 404, 409 and 400.
- Every endpoint is behind sign-in. Restrict actions with `[RequireRole("...")]`: destructive actions are Admin only, approvals are Approver or Admin.
- Business rules go in the Domain project, never in controllers.
- Register services in `Program.cs`. Repositories are interfaces in Domain with an in-memory implementation in Api.
- Every new rule or endpoint needs tests. Use `FakeInvoiceRepository` in tests.
- Use clear names; no one-letter variables or `flag` parameters.
- End-to-end tests use page objects and role-based locators (`getByRole`, `getByLabel`), never CSS or XPath selectors.

## Commands

- Run the app: `dotnet run --project backend/src/InvoiceApp.Api` then open http://localhost:5000
- Run the tests: `dotnet test backend/InvoiceApp.sln`
- Run end-to-end tests: `cd e2e && npm install && npx playwright install chromium && npm test`

## Demo accounts (not real credentials)

- `demo.approver` / `ChangeMe123!` (role Approver)
- `demo.admin` / `ChangeMe123!` (role Admin)

## Never

- Commit real credentials, tokens or customer data.
- Add business logic to `Program.cs` or controllers.

## Approval requirements

Implement the assignment's requirements exactly:

- A Manager may approve invoices up to 50,000.
- A Director may approve invoices up to 500,000.
- Anything above 500,000 requires two distinct approvers, and the second approver must be a Director.
- The same user may never count as both approvers on one invoice.
- A rejected attempt returns a clear reason, and nothing about the invoice changes.

Do not introduce additional business rules that are not specified in the assignment.

## Testing requirements

- Write unit tests for all approval rules.
- Review every generated test.
- Include a negative test proving that the same user cannot count as both approvers.
- Include a negative test proving that a rejected attempt does not change the invoice.
- Add one Playwright end-to-end test covering a rejected over-limit approval through the UI.
- Record actual mutation testing scores before and after the changes. Never invent test results or mutation scores.

## AI-assisted workflow

- Use Spec Kit, OpenSpec, or BMAD to create the specification and task breakdown. Commit the generated files.
- Implement the feature in at least two stages:
  1. Single-approver limits, with tests.
  2. The two-approver rule, with tests.
- Create a separate commit for each implementation stage, with a reasoned commit message.
- Use appropriate agent skills and hooks for structured output.
- Create and use multiple agents and sub-agents for the assignment.
- Log all AI interactions in a file.
- Review the pull request using the workshop review prompt.
- List every review finding in the pull request description, with a verdict of fixed, dismissed with a reason, or deferred with a reason.

## Submission requirements

- Submit a pull request from the fork.
- Include the updated AGENTS.md, specification, task breakdown, tests, and AI interaction log.
- Add ASSIGNMENT.md at the repository root with the write-up and demo video link.
- Provide an 8–10 minute MP4 demo showing something running live, such as tests, mutation testing, or the end-to-end test.
- Install the specified Stryker version for .NET 8 mutation testing:

  dotnet tool install -g dotnet-stryker --version 4.13.0

- Report mutation testing results only after running the tests and recording the actual results.ohk next
oh