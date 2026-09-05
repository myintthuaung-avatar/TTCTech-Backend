using ExampleCompany.Exam.Application.Interfaces;
using ExampleCompany.Exam.Application.Services;
using ExampleCompany.Exam.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string VueDevCorsPolicy = "VueDevCorsPolicy";

// --- Services -----------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Example Company Exam API",
        Version = "v1",
        Description = "Backend for the single-choice IT exam (IT 10-1 / IT 10-2)."
    });
});

builder.Services.AddDbContext<ExamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ExamDb")));

// Application services / unit of work - one scoped instance per HTTP request.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IExamService, ExamService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(VueDevCorsPolicy, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", // Vite dev server default
                "http://localhost:3000") // Vue CLI dev server default
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Pipeline -------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Applies pending migrations and seeds mock data automatically on startup
    // for local development, so there's no manual DB setup step.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ExamDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseCors(VueDevCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so integration tests can spin up the API via WebApplicationFactory<Program>.
public partial class Program
{
}
