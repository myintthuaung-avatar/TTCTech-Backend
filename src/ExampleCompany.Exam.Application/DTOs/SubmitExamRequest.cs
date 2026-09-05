namespace ExampleCompany.Exam.Application.DTOs;

/// <summary>Posted by the Vue app when the examinee clicks "Submit" on IT 10-1.</summary>
public class SubmitExamRequest
{
    public string ExamineeName { get; set; } = string.Empty;

    public List<SubmittedAnswerDto> Answers { get; set; } = new();
}

public class SubmittedAnswerDto
{
    public int QuestionId { get; set; }

    public int ChoiceId { get; set; }
}
