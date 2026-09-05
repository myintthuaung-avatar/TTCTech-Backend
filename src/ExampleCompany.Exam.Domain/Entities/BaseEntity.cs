namespace ExampleCompany.Exam.Domain.Entities;

/// <summary>
/// Common base for all persisted entities so the generic repository
/// (IRepository&lt;T&gt;) has a single constraint to work against.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
