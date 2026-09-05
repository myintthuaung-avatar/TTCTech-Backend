namespace ExampleCompany.Exam.Application.DTOs;

/// <summary>What the Vue app renders on IT 10-2.</summary>
public class ExamResultDto
{
    public int AttemptId { get; set; }

    public string ExamineeName { get; set; } = string.Empty;

    public int Score { get; set; }

    public int TotalQuestions { get; set; }

    public DateTime SubmittedAtUtc { get; set; }

    public List<AnswerReviewDto> AnswerReview { get; set; } = new();
}

public class AnswerReviewDto
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int SelectedChoiceId { get; set; }

    public string SelectedChoiceText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public string CorrectChoiceText { get; set; } = string.Empty;
}
