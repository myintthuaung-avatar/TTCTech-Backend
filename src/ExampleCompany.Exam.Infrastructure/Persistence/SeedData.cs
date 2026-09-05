using ExampleCompany.Exam.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExampleCompany.Exam.Infrastructure.Persistence;

/// <summary>
/// Mock exam data, applied via EF Core's model-seeding (HasData) so it is
/// created automatically as part of the first migration - no manual scripts
/// required, per the assessment note "exam data can be mocked in the database".
/// </summary>
public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamPaper>().HasData(
            new ExamPaper { Id = 1, Title = "IT Knowledge Assessment (Set 10-1)" }
        );

        modelBuilder.Entity<Question>().HasData(
            new Question { Id = 1, ExamPaperId = 1, Order = 1, Text = "Which HTTP method is idempotent?" },
            new Question { Id = 2, ExamPaperId = 1, Order = 2, Text = "Which SQL keyword removes duplicate rows from a result set?" },
            new Question { Id = 3, ExamPaperId = 1, Order = 3, Text = "In a C# interface, which keyword allows a member to have a default implementation?" },
            new Question { Id = 4, ExamPaperId = 1, Order = 4, Text = "What does REST stand for?" },
            new Question { Id = 5, ExamPaperId = 1, Order = 5, Text = "Which ASP.NET Core service lifetime creates one instance per HTTP request?" }
        );

        modelBuilder.Entity<Choice>().HasData(
            // Question 1
            new Choice { Id = 1, QuestionId = 1, Text = "POST", IsCorrect = false },
            new Choice { Id = 2, QuestionId = 1, Text = "PUT", IsCorrect = true },
            new Choice { Id = 3, QuestionId = 1, Text = "PATCH", IsCorrect = false },
            new Choice { Id = 4, QuestionId = 1, Text = "CONNECT", IsCorrect = false },

            // Question 2
            new Choice { Id = 5, QuestionId = 2, Text = "DISTINCT", IsCorrect = true },
            new Choice { Id = 6, QuestionId = 2, Text = "UNIQUE", IsCorrect = false },
            new Choice { Id = 7, QuestionId = 2, Text = "GROUP BY", IsCorrect = false },
            new Choice { Id = 8, QuestionId = 2, Text = "FILTER", IsCorrect = false },

            // Question 3
            new Choice { Id = 9, QuestionId = 3, Text = "abstract", IsCorrect = false },
            new Choice { Id = 10, QuestionId = 3, Text = "default", IsCorrect = true },
            new Choice { Id = 11, QuestionId = 3, Text = "virtual", IsCorrect = false },
            new Choice { Id = 12, QuestionId = 3, Text = "sealed", IsCorrect = false },

            // Question 4
            new Choice { Id = 13, QuestionId = 4, Text = "Representational State Transfer", IsCorrect = true },
            new Choice { Id = 14, QuestionId = 4, Text = "Remote Execution Service Transfer", IsCorrect = false },
            new Choice { Id = 15, QuestionId = 4, Text = "Relational Entity State Transaction", IsCorrect = false },
            new Choice { Id = 16, QuestionId = 4, Text = "Reactive State Transfer", IsCorrect = false },

            // Question 5
            new Choice { Id = 17, QuestionId = 5, Text = "Singleton", IsCorrect = false },
            new Choice { Id = 18, QuestionId = 5, Text = "Scoped", IsCorrect = true },
            new Choice { Id = 19, QuestionId = 5, Text = "Transient", IsCorrect = false },
            new Choice { Id = 20, QuestionId = 5, Text = "Static", IsCorrect = false }
        );
    }
}
