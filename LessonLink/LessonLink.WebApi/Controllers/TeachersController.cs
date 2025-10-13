using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly TeacherService _teacherService;

    public TeachersController(TeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    // GET: api/Teachers
    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        var teachersResult = await _teacherService.GetAllAsync();
        return HandleServiceResult(teachersResult);
    }

    // GET: api/Teachers/featuredteachers
    [HttpGet("featuredteachers")]
    public async Task<IActionResult> GetFeaturedTeachers()
    {
        var featuredTeachersResult = await _teacherService.GetFeaturedAsync();
        return HandleServiceResult(featuredTeachersResult);
    }

    // GET: api/Teachers/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacherById(string id)
    {
        var teacherResult = await _teacherService.GetByIdAsync(id);
        return HandleServiceResult(teacherResult);
    }

    // GET /api/Teachers/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchTeachers([FromQuery] TeacherSearchRequest request)
    {
        var result = await _teacherService.SearchAsync(request);
        return HandleServiceResult(result);
    }

    // POST: api/Teachers
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<IActionResult> PostTeacher([FromBody] TeacherCreateDto teacherCreateDto)
    {
        var result = await _teacherService.CreateAsync(teacherCreateDto);
        return HandleServiceResult(result);
    }

    // PUT: api/Teachers/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeacher(string id, [FromBody] Teacher updatedTeacher)
    {
        if (id != updatedTeacher.UserId)
        {
            return BadRequest(); // 400
        }

        try
        {
            await _teacherService.UpdateAsync(id, updatedTeacher);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await TeacherExists(id))
            {
                return NotFound(); // 404
            }
            else
            {
                throw;
            }
        }

        return NoContent(); // 204
    }

    // DELETE: api/Teachers/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(string id)
    {
        var result = await _teacherService.DeleteAsync(id);
        return HandleServiceResult(result);
    }

    private async Task<bool> TeacherExists(string id)
    {
        return await _teacherService.GetByIdAsync(id) != null;
    }

    private IActionResult HandleServiceResult<T>(ServiceResult<T> result)
    {
        if (result.Succeeded)
        {
            return result.StatusCode switch
            {
                200 => Ok(result.Data),
                201 => CreatedAtAction(nameof(GetTeacherById), new { id = (result.Data as Teacher)?.UserId }, result.Data),
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
