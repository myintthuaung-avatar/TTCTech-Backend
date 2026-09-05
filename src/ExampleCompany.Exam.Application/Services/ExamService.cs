using ExampleCompany.Exam.Application.DTOs;
using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Domain.Entities;

namespace ExampleCompany.Exam.Application.Services;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExamService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ExamPaperDto> GetExamForTakingAsync(int examPaperId, CancellationToken cancellationToken = default)
    {
        var examPaper = await _unitOfWork.ExamPapers.GetWithQuestionsAndChoicesAsync(examPaperId, cancellationToken)
            ?? throw new KeyNotFoundException($"Exam paper {examPaperId} was not found.");

        return new ExamPaperDto
        {
            Id = examPaper.Id,
            Title = examPaper.Title,
            Questions = examPaper.Questions
                .OrderBy(q => q.Order)
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Order = q.Order,
                    Choices = q.Choices
                        .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })
                        .ToList()
                })
                .ToList()
        };
    }

    public async Task<ExamResultDto> SubmitExamAsync(int examPaperId, SubmitExamRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.ExamineeName))
        {
            throw new ArgumentException("Examinee name is required.", nameof(request));
        }

        var examPaper = await _unitOfWork.ExamPapers.GetWithQuestionsAndChoicesAsync(examPaperId, cancellationToken)
            ?? throw new KeyNotFoundException($"Exam paper {examPaperId} was not found.");

        var attempt = new ExamAttempt
        {
            ExamPaperId = examPaper.Id,
            ExamineeName = request.ExamineeName.Trim(),
            SubmittedAtUtc = DateTime.UtcNow,
            TotalQuestions = examPaper.Questions.Count
        };

        var score = 0;

        // Grade against the choices loaded from the database - the client
        // never gets to say what's correct or what the score should be.
        foreach (var question in examPaper.Questions)
        {
            var submitted = request.Answers.FirstOrDefault(a => a.QuestionId == question.Id);
            if (submitted is null)
            {
                continue; // left unanswered
            }

            var selectedChoice = question.Choices.FirstOrDefault(c => c.Id == submitted.ChoiceId);
            if (selectedChoice is null)
            {
                continue; // choice id didn't belong to this question - ignore rather than trust it
            }

            var isCorrect = selectedChoice.IsCorrect;
            if (isCorrect)
            {
                score++;
            }

            attempt.Answers.Add(new ExamAnswer
            {
                QuestionId = question.Id,
                SelectedChoiceId = selectedChoice.Id,
                IsCorrect = isCorrect
            });
        }

        attempt.Score = score;

        await _unitOfWork.ExamAttempts.AddAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return BuildResultDto(attempt, examPaper);
    }

    public async Task<ExamResultDto> GetResultAsync(int attemptId, CancellationToken cancellationToken = default)
    {
        var attempt = await _unitOfWork.ExamAttempts.GetWithAnswersAsync(attemptId, cancellationToken)
            ?? throw new KeyNotFoundException($"Exam attempt {attemptId} was not found.");

        var examPaper = await _unitOfWork.ExamPapers.GetWithQuestionsAndChoicesAsync(attempt.ExamPaperId, cancellationToken)
            ?? throw new KeyNotFoundException($"Exam paper {attempt.ExamPaperId} was not found.");

        return BuildResultDto(attempt, examPaper);
    }

    private static ExamResultDto BuildResultDto(ExamAttempt attempt, ExamPaper examPaper)
    {
        var review = attempt.Answers.Select(a =>
        {
            var question = examPaper.Questions.First(q => q.Id == a.QuestionId);
            var correctChoice = question.Choices.First(c => c.IsCorrect);
            var selectedChoice = question.Choices.First(c => c.Id == a.SelectedChoiceId);

            return new AnswerReviewDto
            {
                QuestionId = question.Id,
                QuestionText = question.Text,
                SelectedChoiceId = selectedChoice.Id,
                SelectedChoiceText = selectedChoice.Text,
                IsCorrect = a.IsCorrect,
                CorrectChoiceText = correctChoice.Text
            };
        }).ToList();

        return new ExamResultDto
        {
            AttemptId = attempt.Id,
            ExamineeName = attempt.ExamineeName,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            SubmittedAtUtc = attempt.SubmittedAtUtc,
            AnswerReview = review
        };
    }
}
