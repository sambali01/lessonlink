using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class TeacherService
{
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;

    public TeacherService(ITeacherRepository teacherRepository, IUserRepository userRepository)
    {
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<TeacherGetDto[]>> GetAllAsync()
    {
        try
        {
            var teachers = await _teacherRepository.GetAllAsync();

            var teachersDto = teachers
                .Select(TeacherMappers.TeacherToGetDto)
                .ToArray();

            return ServiceResult<TeacherGetDto[]>.Success(teachersDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<TeacherGetDto[]>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<TeacherGetDto[]>> GetFeaturedAsync()
    {
        try
        {
            var featuredTeachers = await _teacherRepository.GetFeaturedAsync();

            var featuredTeachersDto = featuredTeachers
                .Select(TeacherMappers.TeacherToGetDto)
                .ToArray();

            return ServiceResult<TeacherGetDto[]>.Success(featuredTeachersDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<TeacherGetDto[]>.Failure(ex.Message, 500);
        }
    }


    public async Task<ServiceResult<TeacherGetDto>> GetByIdAsync(string id)
    {
        try
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
            {
                return ServiceResult<TeacherGetDto>.Failure("Teacher with given id not found.", 404);
            }

            var teacherDto = TeacherMappers.TeacherToGetDto(teacher);

            return ServiceResult<TeacherGetDto>.Success(teacherDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<TeacherGetDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<PaginatedResponse<TeacherGetDto>>> SearchAsync(TeacherSearchRequest request)
    {
        try
        {
            var (teachers, totalCount) = await _teacherRepository.SearchAsync(
                request.SearchQuery,
                request.Subjects,
                request.MinPrice,
                request.MaxPrice,
                request.MinRating,
                request.AcceptsOnline,
                request.AcceptsInPerson,
                request.Page,
                request.PageSize
            );

            var teachersDto = teachers
                .Select(TeacherMappers.TeacherToGetDto)
                .ToList();

            var response = new PaginatedResponse<TeacherGetDto>
            {
                Items = teachersDto,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ServiceResult<PaginatedResponse<TeacherGetDto>>.Success(response);
        }
        catch (Exception ex)
        {
            return ServiceResult<PaginatedResponse<TeacherGetDto>>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<Teacher>> CreateAsync(TeacherCreateDto teacherCreateDto)
    {
        try
        {
            var existingTeacher = await _teacherRepository.GetByIdAsync(teacherCreateDto.UserId);
            if (existingTeacher != null)
                return ServiceResult<Teacher>.Failure("Teacher already exists.", 409);

            var user = await _userRepository.GetByIdAsync(teacherCreateDto.UserId);
            if (user == null)
                return ServiceResult<Teacher>.Failure("Corresponding user not found", 404);

            var addToRoleResult = await _userRepository.AddToRoleAsync(user, Role.Teacher.ToString());
            if (!addToRoleResult.Succeeded)
                return ServiceResult<Teacher>.Failure(addToRoleResult.Errors.Select(e => e.Description).ToList(), 500);

            var teacher = TeacherMappers.CreateDtoToTeacher(teacherCreateDto);
            teacher.User = user;

            await _teacherRepository.CreateAsync(teacher);

            return ServiceResult<Teacher>.Success(teacher, 201);
        }
        catch (Exception ex)
        {
            return ServiceResult<Teacher>.Failure(ex.Message, 500);
        }
    }

    public Task UpdateAsync(string id, Teacher updatedTeacher)
    {
        return _teacherRepository.UpdateAsync(id, updatedTeacher);
    }

    public async Task<ServiceResult<Teacher>> DeleteAsync(string id)
    {
        try
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null)
            {
                return ServiceResult<Teacher>.Failure("Teacher not found", 404);
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ServiceResult<Teacher>.Failure("Corresponding user not found", 404);

            var removeFromRoleResult = await _userRepository.RemoveFromRolesAsync(user, [Role.Teacher.ToString()]);
            if (!removeFromRoleResult.Succeeded)
            {
                return ServiceResult<Teacher>.Failure(removeFromRoleResult.Errors.Select(e => e.Description).ToList(), 500);
            }

            await _teacherRepository.DeleteAsync(teacher);

            return ServiceResult<Teacher>.Success(teacher, 204);
        }
        catch (Exception ex)
        {
            return ServiceResult<Teacher>.Failure(ex.Message, 500);
        }
    }
}
