using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Infrastructure.Persistence.Repositories;

namespace ExampleCompany.Exam.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ExamDbContext _context;

    private IExamPaperRepository? _examPapers;
    private IExamAttemptRepository? _examAttempts;

    public UnitOfWork(ExamDbContext context)
    {
        _context = context;
    }

    public IExamPaperRepository ExamPapers => _examPapers ??= new ExamPaperRepository(_context);

    public IExamAttemptRepository ExamAttempts => _examAttempts ??= new ExamAttemptRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
