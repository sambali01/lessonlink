using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

public class SubjectsController(SubjectService subjectService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjectsResult = await subjectService.GetAllAsync();
        return HandleServiceResult(subjectsResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubjectById(int id)
    {
        var subjectResult = await subjectService.GetByIdAsync(id);
        return HandleServiceResult(subjectResult);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest createSubjectRequest)
    {
        var result = await subjectService.CreateAsync(createSubjectRequest);
        return HandleServiceResult(result);
    }
}
