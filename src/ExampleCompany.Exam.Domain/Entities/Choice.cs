namespace ExampleCompany.Exam.Domain.Entities;

public class Choice : BaseEntity
{
    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Never sent to the client. Only used server-side to grade a submission.
    /// </summary>
    public bool IsCorrect { get; set; }
}
