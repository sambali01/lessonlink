using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.Booking;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Moq;

namespace LessonLink.UnitTest;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepository;
    private readonly Mock<IAvailableSlotRepository> _mockAvailableSlotRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _mockBookingRepository = new Mock<IBookingRepository>();
        _mockAvailableSlotRepository = new Mock<IAvailableSlotRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _bookingService = new BookingService(
            _mockBookingRepository.Object,
            _mockAvailableSlotRepository.Object,
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var studentId = "student-123";
        var createDto = new BookingCreateDto
        {
            AvailableSlotId = 1,
            Notes = "Test booking"
        };

        var availableSlot = new AvailableSlot
        {
            Id = 1,
            TeacherId = "teacher-123",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1)
        };

        var student = new User
        {
            Id = studentId,
            FirstName = "John",
            SurName = "Doe",
            NickName = "JohnD"
        };

        var createdBooking = new Booking
        {
            Id = 1,
            StudentId = studentId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            Notes = "Test booking",
            CreatedAt = DateTime.UtcNow
        };

        var fullBooking = new Booking
        {
            Id = 1,
            StudentId = studentId,
            AvailableSlotId = 1,
            Status = BookingStatus.Pending,
            Notes = "Test booking",
            CreatedAt = DateTime.UtcNow,
            Student = student,
            AvailableSlot = availableSlot
        };

        _mockAvailableSlotRepository
            .Setup(x => x.GetByIdAsync(createDto.AvailableSlotId))
            .ReturnsAsync(availableSlot);

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(studentId))
            .ReturnsAsync(student);

        _mockBookingRepository
            .Setup(x => x.CreateAsync(It.IsAny<Booking>()))
            .ReturnsAsync(createdBooking);

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(createdBooking.Id))
            .ReturnsAsync(fullBooking);

        // Act
        var result = await _bookingService.CreateBookingAsync(studentId, createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.StudentId.Should().Be(studentId);
        result.Data.Notes.Should().Be("Test booking");
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidStudentId_ShouldReturnFailure()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            AvailableSlotId = 1,
            Notes = "Test booking"
        };

        // Act
        var result = await _bookingService.CreateBookingAsync("", createDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Errors.Should().Contain("Student not found.");
    }

    [Fact]
    public async Task CreateBookingAsync_WithNonExistentAvailableSlot_ShouldReturnFailure()
    {
        // Arrange
        var studentId = "student-123";
        var createDto = new BookingCreateDto
        {
            AvailableSlotId = 999,
            Notes = "Test booking"
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
        result.Errors.Should().Contain("Available slot not found.");
    }

    [Fact]
    public async Task UpdateBookingStatusAsync_WithUnauthorizedUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = "unauthorized-user";
        var bookingId = 1;
        var updateDto = new BookingUpdateStatusDto { Status = BookingStatus.Confirmed };

        var booking = new Booking
        {
            Id = bookingId,
            StudentId = "student-123",
            AvailableSlot = new AvailableSlot
            {
                TeacherId = "teacher-123" // Different from userId
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
        result.Errors.Should().Contain("Unauthorized to modify this booking.");
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
            StudentId = studentId,
            AvailableSlot = new AvailableSlot
            {
                TeacherId = "teacher-123"
            }
        };

        _mockBookingRepository
            .Setup(x => x.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        _mockBookingRepository
            .Setup(x => x.DeleteAsync(booking))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CancelBookingAsync(studentId, bookingId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(204);
        _mockBookingRepository.Verify(x => x.DeleteAsync(booking), Times.Once);
    }
}
