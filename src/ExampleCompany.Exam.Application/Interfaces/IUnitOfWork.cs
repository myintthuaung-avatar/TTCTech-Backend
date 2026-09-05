namespace ExampleCompany.Exam.Application.Interfaces;

/// <summary>
/// Coordinates the repositories that participate in a single request and
/// commits them as one database transaction via SaveChangesAsync.
/// </summary>
public interface IUnitOfWork
{
    IExamPaperRepository ExamPapers { get; }

    IExamAttemptRepository ExamAttempts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
