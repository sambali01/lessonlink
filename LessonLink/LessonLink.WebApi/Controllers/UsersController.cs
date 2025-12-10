using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Role = LessonLink.BusinessLogic.Models.Role;

namespace LessonLink.WebApi.Controllers;

public class UsersController(UserManager<User> userManager, TeacherService teacherService, PhotoService photoService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> FindUserById(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new student user.
    /// </summary>
    /// <param name="registerStudentRequest">Student registration data.</param>
    [HttpPost("register-student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest registerStudentRequest)
    {
        // Check if user with the given email already exists
        var existingUser = await userManager.FindByEmailAsync(registerStudentRequest.Email);
        if (existingUser != null)
        {
            return Conflict("User with given email already exists.");
        }

        var user = UserMappers.RegisterStudentRequestToUser(registerStudentRequest);

        // Create user entity
        var createResult = await userManager.CreateAsync(user, registerStudentRequest.Password);
        if (!createResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. createResult.Errors.Select(e => e.Description)], 500));
        }

        // Give student role
        var addToStudentRoleResult = await userManager.AddToRoleAsync(user, Role.Student.ToString());
        if (!addToStudentRoleResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. addToStudentRoleResult.Errors.Select(e => e.Description)], 500));
        }

        return Created();
    }

    /// <summary>
    /// Creates a new teacher user.
    /// </summary>
    /// <param name="registerTeacherRequest">Teacher registration data.</param>
    [HttpPost("register-teacher")]
    public async Task<IActionResult> RegisterTeacher([FromBody] RegisterTeacherRequest registerTeacherRequest)
    {
        // Check if user with the given email already exists
        var existingUser = await userManager.FindByEmailAsync(registerTeacherRequest.Email);
        if (existingUser != null)
        {
            return Conflict("User with given email already exists.");
        }

        var user = UserMappers.RegisterTeacherRequestToUser(registerTeacherRequest);

        // Create user entity
        var createResult = await userManager.CreateAsync(user, registerTeacherRequest.Password);
        if (!createResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. createResult.Errors.Select(e => e.Description)], 500));
        }

        // Give student and teacher role
        var addToStudentRoleResult = await userManager.AddToRoleAsync(user, Role.Student.ToString());
        if (!addToStudentRoleResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. addToStudentRoleResult.Errors.Select(e => e.Description)], 500));
        }

        var addToTeacherRoleResult = await userManager.AddToRoleAsync(user, Role.Teacher.ToString());
        if (!addToTeacherRoleResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. addToTeacherRoleResult.Errors.Select(e => e.Description)], 500));
        }

        // Create teacher entity
        var result = await teacherService.CreateTeacherAsync(user, registerTeacherRequest);
        return HandleServiceResult(result);
    }

    /// <summary>
    /// Updates the student user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="studentUpdateRequest">Student update data.</param>
    /// <returns>The updated user.</returns>
    [HttpPut("update-student/{id}")]
    public async Task<IActionResult> UpdateStudent(string id, [FromForm] StudentUpdateRequest studentUpdateRequest)
    {
        var commonFieldsResult = await UpdateCommonFields(id, studentUpdateRequest.ProfilePicture, studentUpdateRequest.NickName);
        return commonFieldsResult;
    }

    /// <summary>
    /// Updates the teacher user.
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <param name="teacherUpdateRequest">Teacher update data.</param>
    /// <returns>The updated user.</returns>
    [HttpPut("update-teacher/{id}")]
    public async Task<IActionResult> UpdateTeacher(string id, [FromForm] TeacherUpdateRequest teacherUpdateRequest)
    {
        // Update teacher-specific fields
        var updateTeacherResult = await teacherService.UpdateTeacherAsync(id, teacherUpdateRequest);
        if (!updateTeacherResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure(updateTeacherResult.Errors, updateTeacherResult.StatusCode));
        }

        // Update student's and teacher's common fields
        var commonFieldsResult = await UpdateCommonFields(id, teacherUpdateRequest.ProfilePicture, teacherUpdateRequest.NickName);
        return commonFieldsResult;
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        // Check if user exists
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Remove user from all roles
        var userRoles = await userManager.GetRolesAsync(user);
        var removeFromRoleResult = await userManager.RemoveFromRolesAsync(user, userRoles);
        if (!removeFromRoleResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. removeFromRoleResult.Errors.Select(e => e.Description)], 500));
        }

        // Remove the associated teacher entity if there is
        await teacherService.DeleteAsync(id);

        // Remove user entity
        var deleteResult = await userManager.DeleteAsync(user);
        if (!deleteResult.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. deleteResult.Errors.Select(e => e.Description)], 500));
        }

        return NoContent();
    }

    /// <summary>
    /// Updates common fields for both students and teachers.
    /// </summary>
    /// <param name="userId">Id of the user.</param>
    /// <param name="profilePicture">Profile picture file.</param>
    /// <param name="nickName">Nickname of the user.</param>
    /// <returns>The updated user.</returns>
    private async Task<IActionResult> UpdateCommonFields(string userId, IFormFile? profilePicture, string? nickName)
    {
        // Check if user exists
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Set profile picture
        if (profilePicture != null)
        {
            var cloudinaryResult = await photoService.UploadPhotoAsync(profilePicture);
            if (cloudinaryResult.Error != null)
            {
                return HandleServiceResult(ServiceResult<User>.Failure("Photo upload failed: " + cloudinaryResult.Error.Message, 500));
            }

            user.ImageUrl = cloudinaryResult.SecureUrl.ToString();
        }

        // Set nickname
        if (!string.IsNullOrEmpty(nickName))
        {
            user.NickName = nickName;
        }

        // Update user
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return HandleServiceResult(ServiceResult<User>.Failure([.. result.Errors.Select(e => e.Description)], 500));
        }

        return Ok(user);
    }
}
