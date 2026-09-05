namespace ExampleCompany.Exam.Domain.Entities;

/// <summary>
/// A set of questions the examinee takes (screen IT 10-1).
/// </summary>
public class ExamPaper : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
