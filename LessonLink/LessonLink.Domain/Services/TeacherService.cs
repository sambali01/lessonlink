using LessonLink.BusinessLogic.DTOs.Common;
using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.DTOs.User;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class TeacherService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<TeacherGetDto[]>> GetAllAsync()
    {
        var teachers = await unitOfWork.TeacherRepository.GetAllAsync();

        var teachersDto = teachers
            .Select(TeacherMappers.TeacherToGetDto)
            .ToArray();

        return ServiceResult<TeacherGetDto[]>.Success(teachersDto);
    }

    public async Task<ServiceResult<TeacherGetDto[]>> GetFeaturedAsync()
    {
        var featuredTeachers = await unitOfWork.TeacherRepository.GetFeaturedAsync();

        var featuredTeachersDto = featuredTeachers
            .Select(TeacherMappers.TeacherToGetDto)
            .ToArray();

        return ServiceResult<TeacherGetDto[]>.Success(featuredTeachersDto);
    }

    public async Task<ServiceResult<TeacherGetDto>> GetByIdAsync(string id)
    {
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(id);
        if (teacher == null)
        {
            return ServiceResult<TeacherGetDto>.Failure("Teacher with given id not found.", 404);
        }

        var teacherDto = TeacherMappers.TeacherToGetDto(teacher);

        return ServiceResult<TeacherGetDto>.Success(teacherDto);
    }

    public async Task<ServiceResult<PaginatedResponse<TeacherGetDto>>> SearchAsync(TeacherSearchRequest request)
    {
        var (teachers, totalCount) = await unitOfWork.TeacherRepository.SearchAsync(
            request.SearchText,
            request.Subjects,
            request.MinPrice,
            request.MaxPrice,
            request.AcceptsOnline,
            request.AcceptsInPerson,
            request.Location,
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

    /// <summary>
    /// Creates a new teacher entity and associates it with the user.
    /// </summary>
    /// <param name="user">The related user entity.</param>
    /// <param name="registerTeacherDto">Teacher registration data.</param>
    /// <returns>The created teacher user.</returns>
    public async Task<ServiceResult<User>> CreateTeacherAsync(User user, RegisterTeacherDto registerTeacherDto)
    {
        var teacher = new Teacher
        {
            UserId = user.Id,
            User = user,
            AcceptsOnline = registerTeacherDto.AcceptsOnline,
            AcceptsInPerson = registerTeacherDto.AcceptsInPerson,
            Location = registerTeacherDto.Location,
            HourlyRate = registerTeacherDto.HourlyRate,
            Contact = registerTeacherDto.Contact
        };

        unitOfWork.TeacherRepository.CreateAsync(teacher);

        // Create teacher subject entities
        await CreateTeacherSubjectsAsync(teacher.UserId, registerTeacherDto.SubjectNames);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<User>.Success(user, 201);
        }

        return ServiceResult<User>.Failure("An error occurred while creating the teacher.", 500);
    }

    /// <summary>
    /// Updates the teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <param name="teacherUpdateDto">Teacher update data.</param>
    public async Task<ServiceResult<User>> UpdateTeacherAsync(string teacherId, TeacherUpdateDto teacherUpdateDto)
    {
        // Check if teacher exists
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
        {
            return ServiceResult<User>.Failure("Teacher not found", 404);
        }

        // Validate that at least one of online or in-person acceptance is true
        if (teacherUpdateDto.AcceptsOnline.HasValue || teacherUpdateDto.AcceptsInPerson.HasValue)
        {
            bool acceptsOnline = teacherUpdateDto.AcceptsOnline ?? teacher.AcceptsOnline;
            bool acceptsInPerson = teacherUpdateDto.AcceptsInPerson ?? teacher.AcceptsInPerson;
            if (!acceptsOnline && !acceptsInPerson)
            {
                return ServiceResult<User>.Failure("The teacher must accept at least one of online or in-person lessons.", 400);
            }
        }

        // Validate that location is provided when in-person lessons are accepted
        if (teacherUpdateDto.AcceptsInPerson == true && string.IsNullOrEmpty(teacherUpdateDto.Location))
        {
            return ServiceResult<User>.Failure("Location must be provided when accepting in-person lessons.", 400);
        }

        // Set online acceptance
        if (teacherUpdateDto.AcceptsOnline.HasValue)
        {
            teacher.AcceptsOnline = teacherUpdateDto.AcceptsOnline.Value;
        }

        // Set in-person acceptance
        if (teacherUpdateDto.AcceptsInPerson.HasValue)
        {
            teacher.AcceptsInPerson = teacherUpdateDto.AcceptsInPerson.Value;
        }

        // Set location
        if (teacherUpdateDto.Location != null)
        {
            teacher.Location = teacherUpdateDto.Location;
        }

        // Set hourly rate
        if (teacherUpdateDto.HourlyRate.HasValue)
        {
            teacher.HourlyRate = teacherUpdateDto.HourlyRate.Value;
        }
        // Set description
        if (teacherUpdateDto.Description != null)
        {
            teacher.Description = teacherUpdateDto.Description;
        }

        // Set contact
        if (!string.IsNullOrEmpty(teacherUpdateDto.Contact))
        {
            teacher.Contact = teacherUpdateDto.Contact;
        }

        // Update teacher
        unitOfWork.TeacherRepository.UpdateAsync(teacher);

        // Update teacher subjects
        if (teacherUpdateDto.SubjectNames != null)
        {
            unitOfWork.TeacherSubjectRepository.DeleteByTeacherIdAsync(teacherId);
            await CreateTeacherSubjectsAsync(teacherId, teacherUpdateDto.SubjectNames);
        }

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<User>.Success(null);
        }

        return ServiceResult<User>.Failure("An error occurred while updating the teacher.", 500);
    }

    /// <summary>
    /// Creates the teacher subject entities associated with the teacher.
    /// </summary>
    /// <param name="teacherId">Id of the teacher.</param>
    /// <param name="subjectNames">List of subject names.</param>
    private async Task CreateTeacherSubjectsAsync(string teacherId, List<string> subjectNames)
    {
        foreach (var subjectName in subjectNames)
        {
            var subject = await unitOfWork.SubjectRepository.GetByNameAsync(subjectName);
            if (subject != null)
            {
                var teacherSubject = new TeacherSubject
                {
                    TeacherId = teacherId,
                    SubjectId = subject.Id
                };

                unitOfWork.TeacherSubjectRepository.CreateAsync(teacherSubject);
            }
        }
    }

    /// <summary>
    /// Deletes a teacher by their ID.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher to delete.</param>
    public async Task<ServiceResult<User>> DeleteAsync(string teacherId)
    {
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(teacherId);
        if (teacher != null)
        {
            unitOfWork.TeacherRepository.DeleteAsync(teacher);

            if (await unitOfWork.CompleteAsync())
            {
                return ServiceResult<User>.Success(null, 204);
            }

            return ServiceResult<User>.Failure("An error occurred while deleting the teacher.", 500);
        }

        return ServiceResult<User>.Success(null, 204);
    }
}
