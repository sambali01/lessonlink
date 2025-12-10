namespace LessonLink.BusinessLogic.Repositories;

public interface IUnitOfWork
{
    IRefreshTokenRepository RefreshTokenRepository { get; }
    ITeacherRepository TeacherRepository { get; }
    ISubjectRepository SubjectRepository { get; }
    ITeacherSubjectRepository TeacherSubjectRepository { get; }
    IAvailableSlotRepository AvailableSlotRepository { get; }
    IBookingRepository BookingRepository { get; }

    Task<bool> CompleteAsync();
}
