using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class SubjectRepository(LessonLinkDbContext dbContext) : ISubjectRepository
{
    public async Task<IReadOnlyCollection<Subject>> GetAllAsync()
    {
        return await dbContext.Subjects.ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id)
    {
        return await dbContext.Subjects
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Subject?> GetByNameAsync(string name)
    {
        return await dbContext.Subjects
            .SingleOrDefaultAsync(s => s.Name.Equals(name));
    }

    public Subject CreateAsync(Subject subject)
    {
        dbContext.Subjects.Add(subject);
        return subject;
    }
}
