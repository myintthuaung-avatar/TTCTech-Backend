# Example Company — IT Exam API

Backend for a single-choice IT exam application:

1. **IT 10-1** — exam screen, one question at a time, single-select answers only.
2. On submit, the server grades the answers, saves `{ examineeName, score }` to
   the database, and returns the result for **IT 10-2**.
3. "Take again" simply asks the frontend to fetch a fresh exam and reset local
   state — no destructive delete is needed to start over.

Built with **.NET 9**, **ASP.NET Core Web API**, and **EF Core / SQL Server**,
using a layered architecture with a generic repository + unit of work pattern.

## Architecture

```
src/
  ExampleCompany.Exam.Domain/          Entities only. No dependencies on anything else.
  ExampleCompany.Exam.Application/     DTOs, service interfaces, IRepository<T>/IUnitOfWork
                                        contracts, and ExamService (business/grading logic).
  ExampleCompany.Exam.Infrastructure/  EF Core DbContext, generic Repository<T>, entity-specific
                                        repositories, UnitOfWork, and mock seed data.
  ExampleCompany.Exam.Api/             Controllers, Program.cs (composition root), Swagger, CORS.
tests/
  ExampleCompany.Exam.Tests/           Unit tests for ExamService (scoring, validation).
```

Dependency direction: `Api → Infrastructure → Application → Domain`
(`Api` also references `Application` directly for its interfaces/DTOs).

### Why a generic repository + unit of work

`IRepository<T>` implements the CRUD operations every entity needs once. Entity
-specific repositories (`IExamPaperRepository`, `IExamAttemptRepository`) extend
it only to add the one or two queries that are genuinely specific to that
entity (eager-loading the question/choice graph, for example) — rather than
either duplicating CRUD per entity, or forcing every specialised query through
a single bloated generic interface.

`IUnitOfWork` groups the repositories that participate in one request and
commits them together with a single `SaveChangesAsync`, so a submission's
`ExamAttempt` and its `ExamAnswer` rows are always written atomically.

### Grading is always server-side

The client (Vue) only ever receives question text and choice text — never
`IsCorrect`. When an exam is submitted, `ExamService` re-derives the score
from the choices stored in the database; it never trusts a score (or a
correctness flag) sent by the client.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- A local or reachable SQL Server instance (SQL Server, Azure SQL, or
  SQL Server in Docker all work)

## Setup

1. Update the connection string in
   `src/ExampleCompany.Exam.Api/appsettings.json` (`ConnectionStrings:ExamDb`)
   to point at your SQL Server instance.

2. Install the EF Core CLI tool (once per machine), if you don't already have it:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. Restore and create the initial migration:

   ```bash
   dotnet restore

   dotnet ef migrations add InitialCreate \
     --project src/ExampleCompany.Exam.Infrastructure \
     --startup-project src/ExampleCompany.Exam.Api
   ```

4. Run the API. In `Development`, `Program.cs` calls `db.Database.Migrate()`
   on startup, so the database (and mock exam data) is created automatically
   — no separate `database update` step is required for local dev:

   ```bash
   dotnet run --project src/ExampleCompany.Exam.Api
   ```

5. Swagger UI opens automatically at `https://localhost:5081/swagger`
   (or `http://localhost:5080/swagger`).

## Running tests

```bash
dotnet test
```

## API endpoints

| Method | Route                    | Purpose                                             |
|--------|---------------------------|------------------------------------------------------|
| GET    | `/api/exams/{id}`         | Get exam questions/choices for IT 10-1 (id `1` is seeded) |
| POST   | `/api/exams/{id}/submit`  | Submit answers; grades server-side, saves the attempt, returns the IT 10-2 result |
| GET    | `/api/attempts/{id}`      | Re-fetch a previously saved result                   |

### Submit request body

```json
{
  "examineeName": "Jane Doe",
  "answers": [
    { "questionId": 1, "choiceId": 2 },
    { "questionId": 2, "choiceId": 5 }
  ]
}
```

### Submit / result response body

```json
{
  "attemptId": 1,
  "examineeName": "Jane Doe",
  "score": 2,
  "totalQuestions": 5,
  "submittedAtUtc": "2026-09-04T10:00:00Z",
  "answerReview": [
    {
      "questionId": 1,
      "questionText": "Which HTTP method is idempotent?",
      "selectedChoiceId": 2,
      "selectedChoiceText": "PUT",
      "isCorrect": true,
      "correctChoiceText": "PUT"
    }
  ]
}
```

## CORS

The API allows `http://localhost:5173` (Vite default) and `http://localhost:3000`
(Vue CLI default) in development — update the `VueDevCorsPolicy` in
`Program.cs` if your Vue dev server runs on a different port, and restrict it
to your real frontend origin before any non-local deployment.

## Mock data

Five sample questions are seeded via EF Core `HasData` in
`SeedData.cs`/the `InitialCreate` migration — no manual scripts needed.
