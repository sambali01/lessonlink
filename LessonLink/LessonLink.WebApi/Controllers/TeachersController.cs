using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;

namespace LessonLink.WebApi.Controllers;

public class TeachersController(TeacherService teacherService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachersResult = await teacherService.GetAllAsync();
        return HandleServiceResult(teachersResult);
    }

    [HttpGet("featuredteachers")]
    public async Task<IActionResult> GetFeaturedTeachers()
    {
        var featuredTeachersResult = await teacherService.GetFeaturedAsync();
        return HandleServiceResult(featuredTeachersResult);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacherById(string id)
    {
        var teacherResult = await teacherService.GetByIdAsync(id);
        return HandleServiceResult(teacherResult);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTeachers([FromQuery] TeacherSearchRequest searchRequest)
    {
        var result = await teacherService.SearchAsync(searchRequest);
        return HandleServiceResult(result);
    }
}
