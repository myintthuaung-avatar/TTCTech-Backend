using System.Linq.Expressions;
using ExampleCompany.Exam.Domain.Entities;

namespace ExampleCompany.Exam.Application.Interfaces;

/// <summary>
/// Generic repository contract shared by every entity. Entity-specific
/// repositories extend this rather than reimplementing basic CRUD.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
