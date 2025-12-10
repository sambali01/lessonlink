using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers;

public static class SubjectMappers
{
    public static SubjectResponse SubjectToResponse(Subject subject)
    {
        return new SubjectResponse
        {
            Name = subject.Name
        };
    }

    public static Subject CreateRequestToSubject(CreateSubjectRequest subjectCreateDto)
    {
        return new Subject
        {
            Name = subjectCreateDto.Name
        };
    }
}
