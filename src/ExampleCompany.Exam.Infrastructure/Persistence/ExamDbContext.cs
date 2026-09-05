using ExampleCompany.Exam.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExampleCompany.Exam.Infrastructure.Persistence;

public class ExamDbContext : DbContext
{
    public ExamDbContext(DbContextOptions<ExamDbContext> options) : base(options)
    {
    }

    public DbSet<ExamPaper> ExamPapers => Set<ExamPaper>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<Choice> Choices => Set<Choice>();

    public DbSet<ExamAttempt> ExamAttempts => Set<ExamAttempt>();

    public DbSet<ExamAnswer> ExamAnswers => Set<ExamAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamPaper>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);

            entity.HasMany(e => e.Questions)
                  .WithOne(q => q.ExamPaper)
                  .HasForeignKey(q => q.ExamPaperId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);

            entity.HasMany(e => e.Choices)
                  .WithOne(c => c.Question)
                  .HasForeignKey(c => c.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Choice>(entity =>
        {
            entity.Property(e => e.Text).IsRequired().HasMaxLength(500);
        });

        modelBuilder.Entity<ExamAttempt>(entity =>
        {
            entity.Property(e => e.ExamineeName).IsRequired().HasMaxLength(200);

            entity.HasMany(e => e.Answers)
                  .WithOne(a => a.ExamAttempt)
                  .HasForeignKey(a => a.ExamAttemptId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ExamAnswer>(entity =>
        {
            // Restrict here: an answer must never cascade-delete the question
            // or choice it points to - only deleting the attempt should.
            entity.HasOne(a => a.Question)
                  .WithMany()
                  .HasForeignKey(a => a.QuestionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.SelectedChoice)
                  .WithMany()
                  .HasForeignKey(a => a.SelectedChoiceId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData.Seed(modelBuilder);
    }
}
