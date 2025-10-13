using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Models;

namespace LessonLink.BusinessLogic.Mappers
{
    public static class SubjectMappers
    {
        public static SubjectGetDto SubjectToGetDto(Subject subject)
        {
            return new SubjectGetDto
            {
                Name = subject.Name
            };
        }

        public static Subject CreateDtoToSubject(SubjectCreateDto subjectCreateDto)
        {
            return new Subject
            {
                Name = subjectCreateDto.Name
            };
        }
    }
}
