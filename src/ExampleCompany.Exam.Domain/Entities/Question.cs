namespace ExampleCompany.Exam.Domain.Entities;

public class Question : BaseEntity
{
    public int ExamPaperId { get; set; }

    public ExamPaper ExamPaper { get; set; } = null!;

    public string Text { get; set; } = string.Empty;

    /// <summary>Display order on screen IT 10-1.</summary>
    public int Order { get; set; }

    public ICollection<Choice> Choices { get; set; } = new List<Choice>();
}
