using LessonLink.BusinessLogic.Common;
using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Mappers;
using LessonLink.BusinessLogic.Repositories;

namespace LessonLink.BusinessLogic.Services;

public class SubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<ServiceResult<SubjectGetDto[]>> GetAllAsync()
    {
        try
        {
            var subjects = await _subjectRepository.GetAllAsync();

            var subjectsDto = subjects
                .Select(SubjectMappers.SubjectToGetDto)
                .ToArray();

            return ServiceResult<SubjectGetDto[]>.Success(subjectsDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<SubjectGetDto[]>.Failure(ex.Message, 500);
        }
    }

    public async Task<ServiceResult<SubjectGetDto>> GetByIdAsync(int id)
    {
        try
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return ServiceResult<SubjectGetDto>.Failure("Subject with given id not found.", 404);
            }

            var subjectDto = SubjectMappers.SubjectToGetDto(subject);

            return ServiceResult<SubjectGetDto>.Success(subjectDto);
        }
        catch (Exception ex)
        {
            return ServiceResult<SubjectGetDto>.Failure(ex.Message, 500);
        }
    }
}
