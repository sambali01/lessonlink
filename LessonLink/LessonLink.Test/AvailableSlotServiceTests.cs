using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.AvailableSlot;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Moq;

namespace LessonLink.UnitTest;

public class AvailableSlotServiceTests
{
    private readonly Mock<IAvailableSlotRepository> _mockAvailableSlotRepository;
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AvailableSlotService _availableSlotService;

    public AvailableSlotServiceTests()
    {
        _mockAvailableSlotRepository = new Mock<IAvailableSlotRepository>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUnitOfWork.Setup(x => x.AvailableSlotRepository).Returns(_mockAvailableSlotRepository.Object);
        _mockUnitOfWork.Setup(x => x.BookingRepository).Returns(_mockBookingRepository.Object);
        _availableSlotService = new AvailableSlotService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateSlotAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new CreateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        var createdSlot = new AvailableSlot
        {
            Id = 1,
            TeacherId = teacherId,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime
        };

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, createDto.StartTime, createDto.EndTime, null))
            .ReturnsAsync(false);

        _mockBookingRepository
            .Setup(x => x.HasOverlappingActiveBookingForTeacherAsync(teacherId, createDto.StartTime, createDto.EndTime))
            .ReturnsAsync(false);

        _mockAvailableSlotRepository
            .Setup(x => x.CreateAsync(It.IsAny<AvailableSlot>()))
            .Returns(createdSlot);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.TeacherId.Should().Be(teacherId);
    }

    [Fact]
    public async Task CreateSlotAsync_WithEndTimeBeforeStartTime_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new CreateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            EndTime = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("A kezdési időpontnak korábbinak kell lennie, mint a befejezési időpont.");
    }

    [Fact]
    public async Task CreateSlotAsync_WithPastStartTime_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new CreateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddHours(-1),
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Múltbeli időpontot nem adhatsz meg.");
    }

    [Fact]
    public async Task CreateSlotAsync_WithOverlappingSlot_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new CreateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, createDto.StartTime, createDto.EndTime, null))
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Az időpont átfedésben van egy meglévő időponttal.");
    }

    [Fact]
    public async Task CreateSlotAsync_WithOverlappingBooking_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new CreateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, createDto.StartTime, createDto.EndTime, null))
            .ReturnsAsync(false);

        _mockBookingRepository
            .Setup(x => x.HasOverlappingActiveBookingForTeacherAsync(teacherId, createDto.StartTime, createDto.EndTime))
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Aktív foglalásod van ebben az időszakban.");
    }

    [Fact]
    public async Task DeleteSlotAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var teacherId = "teacher-123";
        var slotId = 1;

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>()
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.DeleteSlotAsync(teacherId, slotId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DeleteSlotAsync_WithPastSlot_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var slotId = 1;

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddHours(-1),
            Bookings = new List<Booking>()
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        // Act
        var result = await _availableSlotService.DeleteSlotAsync(teacherId, slotId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Múltbeli időpontot nem törölhetsz.");
    }

    [Fact]
    public async Task DeleteSlotAsync_WithActiveBookings_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var slotId = 1;

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    Status = BookingStatus.Confirmed,
                    StudentId = "student-123",
                    AvailableSlotId = slotId
                }
            }
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        // Act
        var result = await _availableSlotService.DeleteSlotAsync(teacherId, slotId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Errors.Should().Contain("Ez az időpont nem törölhető, mert aktív foglalások vannak rajta.");
    }

    [Fact]
    public async Task GetSlotDetailsAsync_WithValidData_ShouldReturnSlot()
    {
        // Arrange
        var teacherId = "teacher-123";
        var slotId = 1;

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>(),
            Teacher = new Teacher
            {
                UserId = teacherId,
                Contact = "teacher@test.com",
                User = new User { Id = teacherId, FirstName = "John", SurName = "Doe", NickName = "JD" }
            }
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        // Act
        var result = await _availableSlotService.GetSlotDetailsAsync(teacherId, slotId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(slotId);
        result.Data.TeacherId.Should().Be(teacherId);
    }

    [Fact]
    public async Task GetSlotDetailsAsync_WithUnauthorizedTeacher_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var unauthorizedTeacherId = "teacher-456";
        var slotId = 1;

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>()
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        // Act
        var result = await _availableSlotService.GetSlotDetailsAsync(unauthorizedTeacherId, slotId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(403);
        result.Errors.Should().Contain("Nincs jogosultságod megtekinteni ezt az időpontot.");
    }

    [Fact]
    public async Task UpdateSlotAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var teacherId = "teacher-123";
        var slotId = 1;
        var updateDto = new UpdateAvailableSlotRequest
        {
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(1)
        };

        var slot = new AvailableSlot
        {
            Id = slotId,
            TeacherId = teacherId,
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Bookings = new List<Booking>()
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdWithBookingAsync(slotId))
            .ReturnsAsync(slot);

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, updateDto.StartTime, updateDto.EndTime, slotId))
            .ReturnsAsync(false);

        _mockBookingRepository
            .Setup(x => x.HasOverlappingActiveBookingForTeacherAsync(teacherId, updateDto.StartTime, updateDto.EndTime))
            .ReturnsAsync(false);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.UpdateSlotAsync(teacherId, slotId, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }
}
