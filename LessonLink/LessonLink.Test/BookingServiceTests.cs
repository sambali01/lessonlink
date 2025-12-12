using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Moq;

namespace LessonLink.UnitTest;

public class BookingServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IAvailableSlotRepository> _mockAvailableSlotRepository;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockAvailableSlotRepository = new Mock<IAvailableSlotRepository>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUnitOfWork.Setup(x => x.BookingRepository).Returns(_mockBookingRepository.Object);
        _mockUnitOfWork.Setup(x => x.AvailableSlotRepository).Returns(_mockAvailableSlotRepository.Object);
        _mockUnitOfWork.Setup(x => x.TeacherRepository).Returns(_mockTeacherRepository.Object);
        _bookingService = new BookingService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var studentId = "student-123";
        var createDto = new CreateBookingRequest
        {
            AvailableSlotId = 1
        };

        var availableSlot = new AvailableSlot
        {
            Id = 1,
            TeacherId = "teacher-123",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        var createdBooking = new Booking
        {
            Id = 1,
            StudentId = studentId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdAsync(createDto.AvailableSlotId))
            .ReturnsAsync(availableSlot);

        _mockBookingRepository
            .Setup(x => x.HasOverlappingActiveBookingForStudentAsync(studentId, availableSlot.StartTime, availableSlot.EndTime))
            .ReturnsAsync(false);

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync((Teacher?)null);

        _mockBookingRepository
            .Setup(x => x.CreateAsync(It.IsAny<Booking>()))
            .Returns(createdBooking);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _bookingService.CreateBookingAsync(studentId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.StudentId.Should().Be(studentId);
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidStudentId_ShouldReturnFailure()
    {
        // Arrange
        var createDto = new CreateBookingRequest
        {
            AvailableSlotId = 1
        };

        // Act
        var result = await _bookingService.CreateBookingAsync("", createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Errors.Should().Contain("Nem vagy bejelentkezve.");
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistentAvailableSlot_ShouldReturnFailure()
    {
        // Arrange
        var studentId = "student-123";
        var createDto = new CreateBookingRequest
        {
            AvailableSlotId = 999
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdAsync(createDto.AvailableSlotId))
            .ReturnsAsync((AvailableSlot?)null);

        // Act
        var result = await _bookingService.CreateBookingAsync(studentId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Errors.Should().Contain("Az időpont nem található.");
    }

    [Fact]
    public async Task CreateBookingAsync_WithOverlappingBooking_ShouldReturnFailure()
    {
        // Arrange
        var studentId = "student-123";
        var createDto = new CreateBookingRequest
        {
            AvailableSlotId = 1
        };

        var availableSlot = new AvailableSlot
        {
            Id = 1,
            TeacherId = "teacher-123",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdAsync(createDto.AvailableSlotId))
            .ReturnsAsync(availableSlot);

        _mockBookingRepository
            .Setup(x => x.HasOverlappingActiveBookingForStudentAsync(studentId, availableSlot.StartTime, availableSlot.EndTime))
            .ReturnsAsync(true);

        // Act
        var result = await _bookingService.CreateBookingAsync(studentId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Már van foglalásod ebben az időszakban.");
    }

    [Fact]
    public async Task UpdateBookingStatusAsync_WithUnauthorizedUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = "unauthorized-user";
        var bookingId = 1;
        var updateDto = new BookingAcceptanceRequest { Status = BookingStatus.Confirmed };

        var booking = new Booking
        {
            Id = bookingId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            StudentId = "student-123",
            AvailableSlot = new AvailableSlot
            {
                TeacherId = "teacher-123",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.UpdateBookingStatusAsync(userId, bookingId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(403);
        result.Errors.Should().Contain("Nincs jogosultságod módosítani ezt a foglalást.");
    }

    [Fact]
    public async Task UpdateBookingStatusAsync_WithPastBooking_ShouldReturnFailure()
    {
        // Arrange
        var userId = "teacher-123";
        var bookingId = 1;
        var updateDto = new BookingAcceptanceRequest { Status = BookingStatus.Confirmed };

        var booking = new Booking
        {
            Id = bookingId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            StudentId = "student-123",
            AvailableSlot = new AvailableSlot
            {
                TeacherId = userId,
                StartTime = DateTime.UtcNow.AddHours(-2),
                EndTime = DateTime.UtcNow.AddHours(-1)
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.UpdateBookingStatusAsync(userId, bookingId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Múltbeli időpontra vonatkozó foglalást nem fogadhatsz el vagy utasíthatsz el.");
    }

    [Fact]
    public async Task CancelBookingAsync_WithAuthorizedStudent_ShouldReturnSuccess()
    {
        // Arrange
        var studentId = "student-123";
        var bookingId = 1;

        var booking = new Booking
        {
            Id = bookingId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            StudentId = studentId,
            AvailableSlot = new AvailableSlot
            {
                TeacherId = "teacher-123",
                StartTime = DateTime.UtcNow.AddDays(2),
                EndTime = DateTime.UtcNow.AddDays(2).AddHours(1)
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _bookingService.CancelBookingAsync(studentId, bookingId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(204);
        _mockBookingRepository.Verify(x => x.DeleteAsync(booking), Times.Once);
    }

    [Fact]
    public async Task CancelBookingAsync_WithLessThan24Hours_ShouldReturnFailure()
    {
        // Arrange
        var studentId = "student-123";
        var bookingId = 1;

        var booking = new Booking
        {
            Id = bookingId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            StudentId = studentId,
            AvailableSlot = new AvailableSlot
            {
                TeacherId = "teacher-123",
                StartTime = DateTime.UtcNow.AddHours(20),
                EndTime = DateTime.UtcNow.AddHours(21)
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CancelBookingAsync(studentId, bookingId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("A foglalást legalább 24 órával az időpont előtt lehet lemondani.");
    }

    [Fact]
    public async Task GetMyBookingsAsStudentAsync_WithValidStudentId_ShouldReturnBookings()
    {
        // Arrange
        var studentId = "student-123";
        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                StudentId = studentId,
                AvailableSlotId = 1,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                AvailableSlot = new AvailableSlot
                {
                    Id = 1,
                    TeacherId = "teacher-123",
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Teacher = new Teacher
                    {
                        UserId = "teacher-123",
                        Contact = "teacher123@test.com",
                        User = new User { Id = "teacher-123", FirstName = "John", SurName = "Doe", NickName = "JD" }
                    }
                },
                Student = new User { Id = studentId, FirstName = "Jane", SurName = "Smith", NickName = "JS" }
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByStudentIdAsync(studentId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetMyBookingsAsStudentAsync(studentId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data[0].StudentId.Should().Be(studentId);
    }

    [Fact]
    public async Task GetMyBookingsAsTeacherAsync_WithValidTeacherId_ShouldReturnBookings()
    {
        // Arrange
        var teacherId = "teacher-123";
        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = 1,
                StudentId = "student-123",
                AvailableSlotId = 1,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow,
                AvailableSlot = new AvailableSlot
                {
                    Id = 1,
                    TeacherId = teacherId,
                    StartTime = DateTime.UtcNow.AddDays(1),
                    EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                    Teacher = new Teacher
                    {
                        UserId = teacherId,
                        Contact = "teacher@test.com",
                        User = new User { Id = teacherId, FirstName = "John", SurName = "Doe", NickName = "JD" }
                    }
                },
                Student = new User { Id = "student-123", FirstName = "Jane", SurName = "Smith", NickName = "JS" }
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByTeacherIdAsync(teacherId))
            .ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetMyBookingsAsTeacherAsync(teacherId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }
}
