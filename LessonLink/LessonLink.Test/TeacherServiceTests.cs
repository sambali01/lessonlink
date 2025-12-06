using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace LessonLink.UnitTest;

public class TeacherServiceTests
{
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly TeacherService _teacherService;

    public TeacherServiceTests()
    {
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _teacherService = new TeacherService(
            _mockTeacherRepository.Object,
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task SearchAsync_WithValidFilters_ShouldReturnPaginatedResults()
    {
        // Arrange
        var searchRequest = new TeacherSearchRequest
        {
            SearchText = "John",
            Subjects = new List<string> { "Mathematics" },
            MinPrice = 1000,
            MaxPrice = 5000,
            MinRating = 4.0,
            AcceptsOnline = true,
            Page = 1,
            PageSize = 10
        };

        var teachers = new List<Teacher>
        {
            new() {
                UserId = "teacher-1",
                User = new User
                {
                    Id = "teacher-1",
                    FirstName = "John",
                    SurName = "Smith",
                    Email = "john@example.com",
                    NickName = "JohnS"
                },
                HourlyRate = 2000,
                Rating = 4.5,
                AcceptsOnline = true,
                Description = "Math teacher"
            }
        };

        var totalCount = 1;

        _mockTeacherRepository
            .Setup(x => x.SearchAsync(
                searchRequest.SearchText,
                searchRequest.Subjects,
                searchRequest.MinPrice,
                searchRequest.MaxPrice,
                searchRequest.MinRating,
                searchRequest.AcceptsOnline,
                searchRequest.AcceptsInPerson,
                searchRequest.Location,
                searchRequest.Page,
                searchRequest.PageSize))
            .ReturnsAsync((teachers, totalCount));

        // Act
        var result = await _teacherService.SearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().HaveCount(1);
        result.Data.TotalCount.Should().Be(totalCount);
        result.Data.Page.Should().Be(searchRequest.Page);
        result.Data.PageSize.Should().Be(searchRequest.PageSize);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var teacherCreateDto = new TeacherCreateDto
        {
            UserId = "user-123"
        };

        var user = new User
        {
            Id = "user-123",
            FirstName = "John",
            SurName = "Doe",
            Email = "john@example.com",
            NickName = "JohnD"
        };

        var teacher = new Teacher
        {
            UserId = teacherCreateDto.UserId,
            User = user
        };

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherCreateDto.UserId))
            .ReturnsAsync((Teacher?)null);

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(teacherCreateDto.UserId))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(x => x.AddToRoleAsync(user, "Teacher"))
            .ReturnsAsync(IdentityResult.Success);

        _mockTeacherRepository
            .Setup(x => x.CreateAsync(It.IsAny<Teacher>()))
            .ReturnsAsync(teacher);

        // Act
        var result = await _teacherService.CreateAsync(teacherCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.UserId.Should().Be(teacherCreateDto.UserId);
    }

    [Fact]
    public async Task CreateAsync_WithExistingTeacher_ShouldReturnFailure()
    {
        // Arrange
        var teacherCreateDto = new TeacherCreateDto
        {
            UserId = "user-123"
        };

        var existingTeacher = new Teacher
        {
            UserId = "user-123"
        };

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherCreateDto.UserId))
            .ReturnsAsync(existingTeacher);

        // Act
        var result = await _teacherService.CreateAsync(teacherCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Errors.Should().Contain("Teacher already exists.");
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentUser_ShouldReturnFailure()
    {
        // Arrange
        var teacherCreateDto = new TeacherCreateDto
        {
            UserId = "nonexistent-user"
        };

        _mockTeacherRepository
            .Setup(x => x.GetByIdAsync(teacherCreateDto.UserId))
            .ReturnsAsync((Teacher?)null);

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(teacherCreateDto.UserId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _teacherService.CreateAsync(teacherCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Errors.Should().Contain("Corresponding user not found");
    }

    [Fact]
    public async Task SearchAsync_WithEmptyResults_ShouldReturnEmptyPaginatedResponse()
    {
        // Arrange
        var searchRequest = new TeacherSearchRequest
        {
            SearchText = "NonExistentTeacher",
            Page = 1,
            PageSize = 10
        };

        var teachers = new List<Teacher>();
        var totalCount = 0;

        _mockTeacherRepository
            .Setup(x => x.SearchAsync(
                searchRequest.SearchText,
                searchRequest.Subjects,
                searchRequest.MinPrice,
                searchRequest.MaxPrice,
                searchRequest.MinRating,
                searchRequest.AcceptsOnline,
                searchRequest.AcceptsInPerson,
                searchRequest.Location,
                searchRequest.Page,
                searchRequest.PageSize))
            .ReturnsAsync((teachers, totalCount));

        // Act
        var result = await _teacherService.SearchAsync(searchRequest);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
        result.Data.Page.Should().Be(searchRequest.Page);
        result.Data.PageSize.Should().Be(searchRequest.PageSize);
    }
}