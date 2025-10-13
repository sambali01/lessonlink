using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly SubjectService _subjectService;

    public SubjectsController(SubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // GET /api/Subjects
    [HttpGet]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjectsResult = await _subjectService.GetAllAsync();
        return HandleServiceResult(subjectsResult);
    }

    // GET /api/Subjects/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubjectById(int id)
    {
        var subjectResult = await _subjectService.GetByIdAsync(id);
        return HandleServiceResult(subjectResult);
    }

    [HttpPost]
    public async Task<IActionResult> PostSubject([FromBody] SubjectCreateDto subjectCreateDto)
    {
        var result = await _subjectService.CreateAsync(subjectCreateDto);
        return HandleServiceResult(result);
    }

    private IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                201 => CreatedAtAction(nameof(GetSubjectById), new { id = (result.Data as Subject)?.Id }, result.Data),
                204 => NoContent(),
                _ => StatusCode(result.StatusCode, result.Data)
            };
        }
        else
        {
            return result.StatusCode switch
            {
                400 => BadRequest(result.Errors),
                404 => NotFound(result.Errors),
                409 => Conflict(result.Errors),
                _ => StatusCode(result.StatusCode, result.Errors)
            };
        }
    }
}
