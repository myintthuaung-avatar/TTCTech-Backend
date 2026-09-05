using ExampleCompany.Exam.Application.DTOs;
using ExampleCompany.Exam.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExampleCompany.Exam.Api.Controllers;

[ApiController]
[Route("api/exams")]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

    /// <summary>Loads the exam questions/choices for screen IT 10-1.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ExamPaperDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExamPaperDto>> GetExam(int id, CancellationToken cancellationToken)
    {
        try
        {
            var exam = await _examService.GetExamForTakingAsync(id, cancellationToken);
            return Ok(exam);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Grades and saves the submission, returning the result for screen IT 10-2.</summary>
    [HttpPost("{id:int}/submit")]
    [ProducesResponseType(typeof(ExamResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExamResultDto>> SubmitExam(
        int id,
        [FromBody] SubmitExamRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _examService.SubmitExamAsync(id, request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
