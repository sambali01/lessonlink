using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace LessonLink.BusinessLogic.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITeacherRepository _teacherRepository;


    public UserService(IUserRepository userRepository, ITeacherRepository teacherRepository)
    {
        _userRepository = userRepository;
        _teacherRepository = teacherRepository;
    }

    public Task<IReadOnlyCollection<User>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }

    public Task<User?> GetByIdAsync(string id)
    {
        return _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<ServiceResult<User>> CreateAsync(RegisterDto registerDto)
    {
        try
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return ServiceResult<User>.Failure("User with given email already exists.", 409);

            if (!await _userRepository.RoleExistsAsync(registerDto.Role))
                return ServiceResult<User>.Failure($"Role '{registerDto.Role}' not found.", 404);

            var user = UserMappers.RegisterDtoToUser(registerDto);

            var createResult = await _userRepository.CreateAsync(user, registerDto.Password);
            if (!createResult.Succeeded)
                return ServiceResult<User>.Failure(createResult.Errors.Select(e => e.Description).ToList(), 500);

            var addToStudentRoleResult = await _userRepository.AddToRoleAsync(user, Role.Student.ToString());
            if (!addToStudentRoleResult.Succeeded)
            {
                return ServiceResult<User>.Failure(addToStudentRoleResult.Errors.Select(e => e.Description).ToList(), 500);
            }

            if (registerDto.Role == Role.Teacher.ToString())
            {
                var addToTeacherRoleResult = await _userRepository.AddToRoleAsync(user, Role.Teacher.ToString());
                if (!addToTeacherRoleResult.Succeeded)
                    return ServiceResult<User>.Failure(addToTeacherRoleResult.Errors.Select(e => e.Description).ToList(), 500);

                var teacher = new Teacher
                {
                    UserId = user.Id,
                    User = user
                };

                await _teacherRepository.CreateAsync(teacher);
            }

            return ServiceResult<User>.Success(user, 201);
        }
        catch (Exception ex)
        {
            return ServiceResult<User>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<User>> UpdateAsync(string userId, ProfileUpdateDto updateDto)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ServiceResult<User>.Failure("User not found", 404);

            // Profilkép kezelése
            if (updateDto.ProfilePicture != null)
            {
                using var memoryStream = new MemoryStream();
                await updateDto.ProfilePicture.CopyToAsync(memoryStream);
                user.ProfilePicture = memoryStream.ToArray();
            }

            // Mezők frissítése
            if (!string.IsNullOrEmpty(updateDto.NickName))
                user.NickName = updateDto.NickName;

            var result = await _userRepository.UpdateAsync(user);
            if (!result.Succeeded)
                return ServiceResult<User>.Failure(result.Errors.Select(e => e.Description).ToList(), 500);

            return ServiceResult<User>.Success(user);
        }
        catch (Exception ex)
        {
            return ServiceResult<User>.Failure(ex.Message, 500);
        }
    }

    public async Task<IList<string>> GetRolesAsync()
    {
        return await _userRepository.GetAllRolesAsync();
    }

    public async Task<ServiceResult<IList<string>>> GetUserRolesAsync(User user)
    {
        try
        {
            var userRoles = await _userRepository.GetUserRolesAsync(user);
            return ServiceResult<IList<string>>.Success(userRoles);
        }
        catch (Exception ex)
        {
            return ServiceResult<IList<string>>.Failure(ex.Message, 500);
        }
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        return await _userRepository.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveFromRolesAsync(User user, IEnumerable<string> roles)
    {
        return await _userRepository.RemoveFromRolesAsync(user, roles);
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _userRepository.RoleExistsAsync(roleName);
    }

    public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password)
    {
        return await _userRepository.CheckPasswordSignInAsync(user, password);
    }

    public async Task<ServiceResult<User>> DeleteAsync(string id)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return ServiceResult<User>.Failure("User not found.", 404);
            }

            var userRoles = await _userRepository.GetUserRolesAsync(user);
            var removeFromRoleResult = await _userRepository.RemoveFromRolesAsync(user, userRoles);
            if (!removeFromRoleResult.Succeeded)
            {
                return ServiceResult<User>.Failure(removeFromRoleResult.Errors.Select(e => e.Description).ToList(), 500);
            }

            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher != null)
                await _teacherRepository.DeleteAsync(teacher);

            await _userRepository.DeleteAsync(user);

            return ServiceResult<User>.Success(user, 204);
        }
        catch (Exception ex)
        {
            return ServiceResult<User>.Failure(ex.Message, 500);
        }
    }
}
