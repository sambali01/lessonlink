using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
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
    public async Task<IActionResult> PostSubject([FromBody] SubjectCreateDto subjectCreateDto)
    {
        var result = await subjectService.CreateAsync(subjectCreateDto);
        return HandleServiceResult(result);
    }
}
