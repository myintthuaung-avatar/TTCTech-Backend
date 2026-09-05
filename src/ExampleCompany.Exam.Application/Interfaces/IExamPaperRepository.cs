using ExampleCompany.Exam.Domain.Entities;

namespace ExampleCompany.Exam.Application.Interfaces;

/// <summary>
/// Adds the one query that is specific to exam papers (eager-loading the
/// question/choice graph) on top of the generic CRUD operations.
/// </summary>
public interface IExamPaperRepository : IRepository<ExamPaper>
{
    Task<ExamPaper?> GetWithQuestionsAndChoicesAsync(
        int examPaperId,
        CancellationToken cancellationToken = default);
}
