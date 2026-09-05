using ExampleCompany.Exam.Domain.Entities;

namespace ExampleCompany.Exam.Application.Interfaces;

public interface IExamAttemptRepository : IRepository<ExamAttempt>
{
    Task<ExamAttempt?> GetWithAnswersAsync(
        int attemptId,
        CancellationToken cancellationToken = default);
}
