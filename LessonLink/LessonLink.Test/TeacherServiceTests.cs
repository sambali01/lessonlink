using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.Teacher;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Moq;

namespace LessonLink.UnitTest;

public class TeacherServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITeacherRepository> _mockTeacherRepository;
    private readonly Mock<ITeacherSubjectRepository> _mockTeacherSubjectRepository;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly TeacherService _teacherService;

    public TeacherServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeacherRepository = new Mock<ITeacherRepository>();
        _mockTeacherSubjectRepository = new Mock<ITeacherSubjectRepository>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockUnitOfWork.Setup(x => x.TeacherRepository).Returns(_mockTeacherRepository.Object);
        _mockUnitOfWork.Setup(x => x.TeacherSubjectRepository).Returns(_mockTeacherSubjectRepository.Object);
        _mockUnitOfWork.Setup(x => x.SubjectRepository).Returns(_mockSubjectRepository.Object);
        _teacherService = new TeacherService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task SearchAsync_WithValidFilters_ShouldReturnPaginatedResults()
    {
        // Arrange
        var searchRequest = new TeacherSearchRequest
        {
            SearchText = "John",
            Subjects = ["Mathematics"],
            MinPrice = 1000,
            MaxPrice = 5000,
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
                AcceptsOnline = true,
                Description = "Math teacher",
                Contact = "john@example.com"
            }
        };

        var totalCount = 1;

        _mockTeacherRepository
            .Setup(x => x.SearchAsync(
                searchRequest.SearchText,
                searchRequest.Subjects,
                searchRequest.MinPrice,
                searchRequest.MaxPrice,
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