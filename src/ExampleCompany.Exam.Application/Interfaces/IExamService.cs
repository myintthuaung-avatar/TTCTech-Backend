using ExampleCompany.Exam.Application.DTOs;

namespace ExampleCompany.Exam.Application.Interfaces;

public interface IExamService
{
    /// <summary>Builds the exam for screen IT 10-1 (never leaks correct answers).</summary>
    Task<ExamPaperDto> GetExamForTakingAsync(int examPaperId, CancellationToken cancellationToken = default);

    /// <summary>Grades the submission server-side, persists it, and returns the result for IT 10-2.</summary>
    Task<ExamResultDto> SubmitExamAsync(int examPaperId, SubmitExamRequest request, CancellationToken cancellationToken = default);

    /// <summary>Re-fetches a previously saved result, e.g. on page refresh of IT 10-2.</summary>
    Task<ExamResultDto> GetResultAsync(int attemptId, CancellationToken cancellationToken = default);
}
