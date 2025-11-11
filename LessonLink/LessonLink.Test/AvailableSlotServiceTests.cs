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
    private readonly AvailableSlotService _availableSlotService;

    public AvailableSlotServiceTests()
    {
        _mockAvailableSlotRepository = new Mock<IAvailableSlotRepository>();
        _availableSlotService = new AvailableSlotService(_mockAvailableSlotRepository.Object);
    }

    [Fact]
    public async Task GetMySlotsAsync_WithValidTeacherId_ShouldReturnSlotsWithBookings()
    {
        // Arrange
        var teacherId = "teacher-123";
        var availableSlots = new List<AvailableSlot>
        {
            new() {
                Id = 1,
                TeacherId = teacherId,
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
                Bookings = new List<Booking>
                {
                    new Booking
                    {
                        Id = 1,
                        Status = BookingStatus.Pending,
                        Student = new User { FirstName = "John", SurName = "Doe", NickName = "JohnD" },
                        StudentId = "student-1"
                    },
                    new Booking
                    {
                        Id = 2,
                        Status = BookingStatus.Cancelled, // This should be filtered out
                        Student = new User { FirstName = "Jane", SurName = "Smith", NickName = "JaneS" },
                        StudentId = "student-2"
                    }
                }
            }
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByTeacherIdWithBookingsAsync(teacherId))
            .ReturnsAsync(availableSlots);

        // Act
        var result = await _availableSlotService.GetMySlotsAsync(teacherId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(1);

        var slot = result.Data.First();
        slot.TeacherId.Should().Be(teacherId);
        slot.Bookings.Should().HaveCount(1); // Only non-cancelled bookings
        slot.Bookings.First().Status.Should().Be(BookingStatus.Pending);
    }

    [Fact]
    public async Task GetMySlotsAsync_WithEmptyTeacherId_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = await _availableSlotService.GetMySlotsAsync("");

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Errors.Should().Contain("Teacher not found.");
    }

    [Fact]
    public async Task GetMySlotsAsync_WithNullTeacherId_ShouldReturnFailure()
    {
        // Arrange & Act
        var result = await _availableSlotService.GetMySlotsAsync(null!);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Errors.Should().Contain("Teacher not found.");
    }

    [Fact]
    public async Task GetMySlotsAsync_WithValidTeacherIdButNoSlots_ShouldReturnEmptyList()
    {
        // Arrange
        var teacherId = "teacher-123";
        var availableSlots = new List<AvailableSlot>();

        _mockAvailableSlotRepository
            .Setup(x => x.GetByTeacherIdWithBookingsAsync(teacherId))
            .ReturnsAsync(availableSlots);

        // Act
        var result = await _availableSlotService.GetMySlotsAsync(teacherId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateSlotAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new AvailableSlotCreateDto
        {
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1)
        };

        var createdSlot = new AvailableSlot
        {
            Id = 1,
            TeacherId = teacherId,
            StartTime = createDto.StartTime,
            EndTime = createDto.EndTime
        };

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, createDto.StartTime, createDto.EndTime))
            .ReturnsAsync(false);

        _mockAvailableSlotRepository
            .Setup(x => x.CreateAsync(It.IsAny<AvailableSlot>()))
            .ReturnsAsync(createdSlot);

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
        var createDto = new AvailableSlotCreateDto
        {
            StartTime = DateTime.Now.AddDays(1).AddHours(1),
            EndTime = DateTime.Now.AddDays(1) // End time before start time
        };

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("A kezdési időnek korábban kell lennie, mint a befejezési időnek");
    }

    [Fact]
    public async Task CreateSlotAsync_WithPastStartTime_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new AvailableSlotCreateDto
        {
            StartTime = DateTime.Now.AddHours(-1), // Past time
            EndTime = DateTime.Now.AddHours(1)
        };

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Nem adhatsz múltbeli időpontot");
    }

    [Fact]
    public async Task CreateSlotAsync_WithOverlappingSlot_ShouldReturnFailure()
    {
        // Arrange
        var teacherId = "teacher-123";
        var createDto = new AvailableSlotCreateDto
        {
            StartTime = DateTime.Now.AddDays(1),
            EndTime = DateTime.Now.AddDays(1).AddHours(1)
        };

        _mockAvailableSlotRepository
            .Setup(x => x.HasOverlappingSlotAsync(teacherId, createDto.StartTime, createDto.EndTime))
            .ReturnsAsync(true);

        // Act
        var result = await _availableSlotService.CreateSlotAsync(teacherId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Errors.Should().Contain("Az időpont átfedésben van egy már meglévő időponttal");
    }
}