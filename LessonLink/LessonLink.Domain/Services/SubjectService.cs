using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Helpers;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class SubjectService(IUnitOfWork unitOfWork)
{
    public async Task<ServiceResult<SubjectResponse[]>> GetAllAsync()
    {
        var subjects = await unitOfWork.SubjectRepository.GetAllAsync();

        var subjectsDto = subjects
            .Select(SubjectMappers.SubjectToResponse)
            .ToArray();

        return ServiceResult<SubjectResponse[]>.Success(subjectsDto);
    }

    public async Task<ServiceResult<SubjectResponse>> GetByIdAsync(int id)
    {
        var subject = await unitOfWork.SubjectRepository.GetByIdAsync(id);
        if (subject == null)
        {
            return ServiceResult<SubjectResponse>.Failure("Subject with given id not found.", 404);
        }

        var subjectDto = SubjectMappers.SubjectToResponse(subject);

        return ServiceResult<SubjectResponse>.Success(subjectDto);
    }

    public async Task<ServiceResult<Subject>> CreateAsync(CreateSubjectRequest createSubjectRequest)
    {
        var existingSubject = await unitOfWork.SubjectRepository.GetByNameAsync(createSubjectRequest.Name);
        if (existingSubject != null)
        {
            return ServiceResult<Subject>.Failure("Subject already exists.", 409);
        }

        Subject subject = SubjectMappers.CreateRequestToSubject(createSubjectRequest);

        unitOfWork.SubjectRepository.CreateAsync(subject);

        if (await unitOfWork.CompleteAsync())
        {
            return ServiceResult<Subject>.Success(subject, 201);
        }

        return ServiceResult<Subject>.Failure("An error occurred while creating the subject.", 500);
    }
}
