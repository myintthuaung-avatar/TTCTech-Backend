namespace ExampleCompany.Exam.Application.DTOs;

/// <summary>What the Vue app renders on IT 10-1. Deliberately has no IsCorrect anywhere.</summary>
public class ExamPaperDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public List<QuestionDto> Questions { get; set; } = new();
}

public class QuestionDto
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public int Order { get; set; }

    public List<ChoiceDto> Choices { get; set; } = new();
}

public class ChoiceDto
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
}
