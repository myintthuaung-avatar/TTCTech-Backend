using ExampleCompany.Exam.Application.DTOs;
using ExampleCompany.Exam.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExampleCompany.Exam.Api.Controllers;

[ApiController]
[Route("api/attempts")]
public class AttemptsController : ControllerBase
{
    private readonly IExamService _examService;

    public AttemptsController(IExamService examService)
    {
        _examService = examService;
    }

    /// <summary>Re-fetches a previously saved result, e.g. if IT 10-2 is reloaded.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExamResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExamResultDto>> GetAttempt(int id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _examService.GetResultAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
