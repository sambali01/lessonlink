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
    public async Task<ServiceResult<TeacherResponse[]>> GetAllAsync()
    {
        var teachers = await unitOfWork.TeacherRepository.GetAllAsync();

        var teachersDto = teachers
            .Select(TeacherMappers.TeacherToResponse)
            .ToArray();

        return ServiceResult<TeacherResponse[]>.Success(teachersDto);
    }

    public async Task<ServiceResult<TeacherResponse[]>> GetFeaturedAsync()
    {
        var featuredTeachers = await unitOfWork.TeacherRepository.GetFeaturedAsync();

        var featuredTeachersDto = featuredTeachers
            .Select(TeacherMappers.TeacherToResponse)
            .ToArray();

        return ServiceResult<TeacherResponse[]>.Success(featuredTeachersDto);
    }

    public async Task<ServiceResult<TeacherResponse>> GetByIdAsync(string id)
    {
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(id);
        if (teacher == null)
        {
            return ServiceResult<TeacherResponse>.Failure("A megadott azonosítóval nem található tanár.", 404);
        }

        var teacherDto = TeacherMappers.TeacherToResponse(teacher);

        return ServiceResult<TeacherResponse>.Success(teacherDto);
    }

    public async Task<ServiceResult<string>> GetTeacherContactAsync(string teacherId)
    {
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
        {
            return ServiceResult<string>.Failure("A megadott azonosítóval nem található tanár.", 404);
        }

        return ServiceResult<string>.Success(teacher.Contact);
    }

    public async Task<ServiceResult<PaginatedResponse<TeacherResponse>>> SearchAsync(TeacherSearchRequest teacherSearchRequest)
    {
        var (teachers, totalCount) = await unitOfWork.TeacherRepository.SearchAsync(
            teacherSearchRequest.SearchText,
            teacherSearchRequest.Subjects,
            teacherSearchRequest.MinPrice,
            teacherSearchRequest.MaxPrice,
            teacherSearchRequest.AcceptsOnline,
            teacherSearchRequest.AcceptsInPerson,
            teacherSearchRequest.Location,
            teacherSearchRequest.Page,
            teacherSearchRequest.PageSize
        );

        var teachersDto = teachers
            .Select(TeacherMappers.TeacherToResponse)
            .ToList();

        var response = new PaginatedResponse<TeacherResponse>
        {
            Items = teachersDto,
            TotalCount = totalCount,
            Page = teacherSearchRequest.Page,
            PageSize = teacherSearchRequest.PageSize
        };

        return ServiceResult<PaginatedResponse<TeacherResponse>>.Success(response);
    }

    /// <summary>
    /// Creates a new teacher entity and associates it with the user.
    /// </summary>
    /// <param name="user">The related user entity.</param>
    /// <param name="registerTeacherRequest">Teacher registration data.</param>
    /// <returns>The created teacher user.</returns>
    public async Task<ServiceResult<User>> CreateTeacherAsync(User user, RegisterTeacherRequest registerTeacherRequest)
    {
        var teacher = new Teacher
        {
            UserId = user.Id,
            User = user,
            AcceptsOnline = registerTeacherRequest.AcceptsOnline,
            AcceptsInPerson = registerTeacherRequest.AcceptsInPerson,
            Location = registerTeacherRequest.Location,
            HourlyRate = registerTeacherRequest.HourlyRate,
            Contact = registerTeacherRequest.Contact
        };

        unitOfWork.TeacherRepository.CreateAsync(teacher);

        // Create teacher subject entities
        await CreateTeacherSubjectsAsync(teacher.UserId, registerTeacherRequest.SubjectNames);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<User>.Success(user, 201);
        }

        return ServiceResult<User>.Failure("Hiba történt a tanár létrehozása során.", 500);
    }

    /// <summary>
    /// Updates the teacher.
    /// </summary>
    /// <param name="teacherId">The ID of the teacher.</param>
    /// <param name="teacherUpdateRequest">Teacher update data.</param>
    public async Task<ServiceResult<User>> UpdateTeacherAsync(string teacherId, TeacherUpdateRequest teacherUpdateRequest)
    {
        // Check if teacher exists
        var teacher = await unitOfWork.TeacherRepository.GetByIdAsync(teacherId);
        if (teacher == null)
        {
            return ServiceResult<User>.Failure("A tanár nem található", 404);
        }

        // Validate that at least one of online or in-person acceptance is true
        if (teacherUpdateRequest.AcceptsOnline.HasValue || teacherUpdateRequest.AcceptsInPerson.HasValue)
        {
            bool acceptsOnline = teacherUpdateRequest.AcceptsOnline ?? teacher.AcceptsOnline;
            bool acceptsInPerson = teacherUpdateRequest.AcceptsInPerson ?? teacher.AcceptsInPerson;
            if (!acceptsOnline && !acceptsInPerson)
            {
                return ServiceResult<User>.Failure("Legalább az egyik oktatási formát (online vagy személyes) el kell fogadnod.", 400);
            }
        }

        // Validate that location is provided when in-person lessons are accepted
        if (teacherUpdateRequest.AcceptsInPerson == true && string.IsNullOrEmpty(teacherUpdateRequest.Location))
        {
            return ServiceResult<User>.Failure("Személyes oktatás elfogadása esetén meg kell adnod a helyszínt.", 400);
        }

        // Set online acceptance
        if (teacherUpdateRequest.AcceptsOnline.HasValue)
        {
            teacher.AcceptsOnline = teacherUpdateRequest.AcceptsOnline.Value;
        }

        // Set in-person acceptance
        if (teacherUpdateRequest.AcceptsInPerson.HasValue)
        {
            teacher.AcceptsInPerson = teacherUpdateRequest.AcceptsInPerson.Value;

            // Clear location if in-person is disabled
            if (teacherUpdateRequest.AcceptsInPerson == false)
            {
                teacher.Location = null;
            }
            // Set location if in-person is enabled and location is provided
            else if (teacherUpdateRequest.AcceptsInPerson == true && teacherUpdateRequest.Location != null)
            {
                teacher.Location = teacherUpdateRequest.Location;
            }
        }

        // Set hourly rate
        if (teacherUpdateRequest.HourlyRate.HasValue)
        {
            teacher.HourlyRate = teacherUpdateRequest.HourlyRate.Value;
        }
        // Set description
        if (teacherUpdateRequest.Description != null)
        {
            teacher.Description = teacherUpdateRequest.Description;
        }

        // Set contact
        if (!string.IsNullOrEmpty(teacherUpdateRequest.Contact))
        {
            teacher.Contact = teacherUpdateRequest.Contact;
        }

        // Update teacher
        unitOfWork.TeacherRepository.UpdateAsync(teacher);

        // Update teacher subjects
        if (teacherUpdateRequest.SubjectNames != null)
        {
            unitOfWork.TeacherSubjectRepository.DeleteByTeacherIdAsync(teacherId);
            await CreateTeacherSubjectsAsync(teacherId, teacherUpdateRequest.SubjectNames);
        }

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<User>.Success(null);
        }

        return ServiceResult<User>.Failure("Hiba történt a tanári profilod módosítása során.", 500);
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

            return ServiceResult<User>.Failure("Hiba történt a tanár törlése során.", 500);
        }

        return ServiceResult<User>.Success(null, 204);
    }
}
