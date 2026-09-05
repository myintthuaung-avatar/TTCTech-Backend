using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Domain.Entities;
using ExampleCompany.Exam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExampleCompany.Exam.Infrastructure.Persistence.Repositories;

public class ExamPaperRepository : Repository<ExamPaper>, IExamPaperRepository
{
    public ExamPaperRepository(ExamDbContext context) : base(context)
    {
    }

    public async Task<ExamPaper?> GetWithQuestionsAndChoicesAsync(
        int examPaperId,
        CancellationToken cancellationToken = default)
        => await Context.ExamPapers
            .Include(e => e.Questions)
                .ThenInclude(q => q.Choices)
            .FirstOrDefaultAsync(e => e.Id == examPaperId, cancellationToken);
}
