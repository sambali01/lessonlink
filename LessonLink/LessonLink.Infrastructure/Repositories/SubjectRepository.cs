using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly LessonLinkDbContext _dbContext;

    public SubjectRepository(LessonLinkDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IReadOnlyCollection<Subject>> GetAllAsync()
    {
        return await _dbContext.Subjects.ToListAsync();
    }

    public async Task<Subject?> GetByIdAsync(int id)
    {
        return await _dbContext.Subjects
            .SingleOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Subject?> GetByNameAsync(string name)
    {
        return await _dbContext.Subjects
            .SingleOrDefaultAsync(s => s.Name.Equals(name));
    }

    public async Task<Subject> CreateAsync(Subject subject)
    {
        _dbContext.Subjects.Add(subject);
        await _dbContext.SaveChangesAsync();
        return subject;
    }
}
