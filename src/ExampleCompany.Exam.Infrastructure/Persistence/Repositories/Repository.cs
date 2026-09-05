using System.Linq.Expressions;
using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Domain.Entities;
using ExampleCompany.Exam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ExampleCompany.Exam.Infrastructure.Persistence.Repositories;

/// <summary>
/// Single generic implementation reused by every entity-specific repository,
/// so basic CRUD is written once instead of per entity.
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ExamDbContext Context;
    protected readonly DbSet<T> DbSet;

    public Repository(ExamDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);
}
