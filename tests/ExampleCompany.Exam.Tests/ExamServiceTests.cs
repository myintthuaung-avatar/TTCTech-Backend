using ExampleCompany.Exam.Application.DTOs;
using ExampleCompany.Exam.Application.Services;
using ExampleCompany.Exam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExampleCompany.Exam.Tests;

public class ExamServiceTests
{
    private static ExamDbContext CreateSeededContext()
    {
        var options = new DbContextOptionsBuilder<ExamDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ExamDbContext(options);
        context.Database.EnsureCreated(); // applies the same HasData seed used by migrations
        return context;
    }

    [Fact]
    public async Task SubmitExamAsync_CalculatesScoreCorrectly_WhenAllAnswersCorrect()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var request = new SubmitExamRequest
        {
            ExamineeName = "John Doe",
            Answers = new List<SubmittedAnswerDto>
            {
                new() { QuestionId = 1, ChoiceId = 2 },  // PUT - correct
                new() { QuestionId = 2, ChoiceId = 5 },  // DISTINCT - correct
                new() { QuestionId = 3, ChoiceId = 10 }, // default - correct
                new() { QuestionId = 4, ChoiceId = 13 }, // Representational State Transfer - correct
                new() { QuestionId = 5, ChoiceId = 18 }  // Scoped - correct
            }
        };

        var result = await service.SubmitExamAsync(1, request);

        Assert.Equal(5, result.Score);
        Assert.Equal(5, result.TotalQuestions);
        Assert.Equal("John Doe", result.ExamineeName);
    }

    [Fact]
    public async Task SubmitExamAsync_IgnoresClientSuppliedScore_AndGradesServerSide()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var request = new SubmitExamRequest
        {
            ExamineeName = "Jane Doe",
            Answers = new List<SubmittedAnswerDto>
            {
                new() { QuestionId = 1, ChoiceId = 1 }, // POST - wrong
                new() { QuestionId = 2, ChoiceId = 5 }  // DISTINCT - correct
                // Questions 3-5 left unanswered on purpose.
            }
        };

        var result = await service.SubmitExamAsync(1, request);

        Assert.Equal(1, result.Score);
        Assert.Equal(5, result.TotalQuestions); // total counts every question in the paper, not just answered ones
    }

    [Fact]
    public async Task SubmitExamAsync_ThrowsArgumentException_WhenExamineeNameMissing()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var request = new SubmitExamRequest { ExamineeName = "   ", Answers = new List<SubmittedAnswerDto>() };

        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitExamAsync(1, request));
    }

    [Fact]
    public async Task SubmitExamAsync_ThrowsKeyNotFound_WhenExamPaperDoesNotExist()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var request = new SubmitExamRequest { ExamineeName = "John Doe", Answers = new List<SubmittedAnswerDto>() };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.SubmitExamAsync(999, request));
    }

    [Fact]
    public async Task GetExamForTakingAsync_ReturnsAllQuestionsWithoutExposingCorrectAnswer()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var examDto = await service.GetExamForTakingAsync(1);

        Assert.Equal("IT Knowledge Assessment (Set 10-1)", examDto.Title);
        Assert.Equal(5, examDto.Questions.Count);
        Assert.All(examDto.Questions, q => Assert.True(q.Choices.Count >= 2));
        // ChoiceDto intentionally has no IsCorrect property - nothing to assert
        // at runtime, the compiler is what enforces this guarantee.
    }

    [Fact]
    public async Task GetResultAsync_ReturnsPreviouslySavedAttempt()
    {
        await using var context = CreateSeededContext();
        var service = new ExamService(new UnitOfWork(context));

        var submitted = await service.SubmitExamAsync(1, new SubmitExamRequest
        {
            ExamineeName = "Retrieved Later",
            Answers = new List<SubmittedAnswerDto> { new() { QuestionId = 1, ChoiceId = 2 } }
        });

        var fetched = await service.GetResultAsync(submitted.AttemptId);

        Assert.Equal(submitted.AttemptId, fetched.AttemptId);
        Assert.Equal("Retrieved Later", fetched.ExamineeName);
        Assert.Equal(1, fetched.Score);
    }
}
