namespace ExampleCompany.Exam.Domain.Entities;

/// <summary>
/// The choice an examinee selected for one question within an attempt.
/// </summary>
public class ExamAnswer : BaseEntity
{
    public int ExamAttemptId { get; set; }

    public ExamAttempt ExamAttempt { get; set; } = null!;

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public int SelectedChoiceId { get; set; }

    public Choice SelectedChoice { get; set; } = null!;

    public bool IsCorrect { get; set; }
}
