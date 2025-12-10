using FluentAssertions;
using LessonLink.BusinessLogic.DTOs.Subject;
using LessonLink.BusinessLogic.Models;
using LessonLink.BusinessLogic.Repositories;
using LessonLink.BusinessLogic.Services;
using Moq;

namespace LessonLink.UnitTest;

public class SubjectServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISubjectRepository> _mockSubjectRepository;
    private readonly SubjectService _subjectService;

    public SubjectServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSubjectRepository = new Mock<ISubjectRepository>();
        _mockUnitOfWork.Setup(x => x.SubjectRepository).Returns(_mockSubjectRepository.Object);
        _subjectService = new SubjectService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var subjectCreateDto = new CreateSubjectRequest
        {
            Name = "Mathematics"
        };

        var subject = new Subject
        {
            Id = 1,
            Name = "Mathematics"
        };

        _mockSubjectRepository
            .Setup(x => x.GetByNameAsync(subjectCreateDto.Name))
            .ReturnsAsync((Subject?)null);

        _mockSubjectRepository
            .Setup(x => x.CreateAsync(It.IsAny<Subject>()))
            .Returns(subject);

        _mockUnitOfWork
            .Setup(x => x.CompleteAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _subjectService.CreateAsync(subjectCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Mathematics");
        _mockSubjectRepository.Verify(x => x.CreateAsync(It.IsAny<Subject>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingSubjectName_ShouldReturnFailure()
    {
        // Arrange
        var subjectCreateDto = new CreateSubjectRequest
        {
            Name = "Mathematics"
        };

        var existingSubject = new Subject
        {
            Id = 1,
            Name = "Mathematics"
        };

        _mockSubjectRepository
            .Setup(x => x.GetByNameAsync(subjectCreateDto.Name))
            .ReturnsAsync(existingSubject);

        // Act
        var result = await _subjectService.CreateAsync(subjectCreateDto);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.Errors.Should().Contain("Subject already exists.");
        _mockSubjectRepository.Verify(x => x.CreateAsync(It.IsAny<Subject>()), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSubjects()
    {
        // Arrange
        var subjects = new List<Subject>
        {
            new Subject { Id = 1, Name = "Mathematics" },
            new Subject { Id = 2, Name = "Physics" },
            new Subject { Id = 3, Name = "Chemistry" }
        };

        _mockSubjectRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(subjects);

        // Act
        var result = await _subjectService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.Select(s => s.Name).Should().Contain(new[] { "Mathematics", "Physics", "Chemistry" });
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSubject()
    {
        // Arrange
        var subjectId = 1;
        var subject = new Subject
        {
            Id = subjectId,
            Name = "Mathematics"
        };

        _mockSubjectRepository
            .Setup(x => x.GetByIdAsync(subjectId))
            .ReturnsAsync(subject);

        // Act
        var result = await _subjectService.GetByIdAsync(subjectId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Mathematics");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
    {
        // Arrange
        var subjectId = 999;

        _mockSubjectRepository
            .Setup(x => x.GetByIdAsync(subjectId))
            .ReturnsAsync((Subject?)null);

        // Act
        var result = await _subjectService.GetByIdAsync(subjectId);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Errors.Should().Contain("Subject with given id not found.");
    }
}