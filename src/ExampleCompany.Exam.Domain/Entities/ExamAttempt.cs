namespace ExampleCompany.Exam.Domain.Entities;

/// <summary>
/// One completed exam submission: who took it and what score they got
/// (screen IT 10-2 is built from this).
/// </summary>
public class ExamAttempt : BaseEntity
{
    public int ExamPaperId { get; set; }

    public ExamPaper ExamPaper { get; set; } = null!;

    public string ExamineeName { get; set; } = string.Empty;

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime SubmittedAtUtc { get; set; }

    public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
}
