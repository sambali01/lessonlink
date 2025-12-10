using LessonLink.BusinessLogic.Repositories;
using LessonLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LessonLink.Infrastructure.Repositories;

public class UnitOfWork(LessonLinkDbContext dbContext) : IUnitOfWork
{
    private IRefreshTokenRepository? _refreshTokenRepository;
    private ITeacherRepository? _teacherRepository;
    private ISubjectRepository? _subjectRepository;
    private ITeacherSubjectRepository? _teacherSubjectRepository;
    private IAvailableSlotRepository? _availableSlotRepository;
    private IBookingRepository? _bookingRepository;

    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(dbContext);

    public ITeacherRepository TeacherRepository => _teacherRepository ??= new TeacherRepository(dbContext);

    public ISubjectRepository SubjectRepository => _subjectRepository ??= new SubjectRepository(dbContext);

    public ITeacherSubjectRepository TeacherSubjectRepository => _teacherSubjectRepository ??= new TeacherSubjectRepository(dbContext);

    public IAvailableSlotRepository AvailableSlotRepository => _availableSlotRepository ??= new AvailableSlotRepository(dbContext);

    public IBookingRepository BookingRepository => _bookingRepository ??= new BookingRepository(dbContext);

    public async Task<bool> CompleteAsync()
    {
        try
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("An error occured while saving changes to the database.", ex);
        }
    }
}
