using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Domain.Entities;
using ExampleCompany.Exam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExampleCompany.Exam.Infrastructure.Persistence.Repositories;

public class ExamAttemptRepository : Repository<ExamAttempt>, IExamAttemptRepository
{
    public ExamAttemptRepository(ExamDbContext context) : base(context)
    {
    }

    public async Task<ExamAttempt?> GetWithAnswersAsync(
        int attemptId,
        CancellationToken cancellationToken = default)
        => await Context.ExamAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);
}
