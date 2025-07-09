using BuildingBlocks.Common.Classes;
using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Models;
using MainApp.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MainApp.Test.Application.Service.Tests;

public class AdminServiceTests
{
    private readonly AdminService _adminService;
    private readonly AppDbContext _appDbContext;

    public AdminServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        _adminService = new AdminService(_appDbContext);
    }

    [Fact]
    public async Task GetProblem_ShouldReturnProblem_WhenProblemExists()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "Test problem text",
            Difficulty = "easy",
            Tag = "test",
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCase>()
        };

        _appDbContext.Problems.Add(problem);
        await _appDbContext.SaveChangesAsync();

        // Act
        var result = await _adminService.GetProblem(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Problem");
    }

    [Fact]
    public async Task GetProblem_ShouldThrowException_WhenProblemDoesNotExist()
    {
        // Act & Assert
        var act = async () => await _adminService.GetProblem(999);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("problem not found");
    }

    [Fact]
    public async Task AddProblem_ShouldAddProblem_WhenProblemIsValid()
    {
        // Arrange
        var problemDto = new AddProblemDto
        {
            Name = "New Problem",
            ProblemText = "Test Problem",
            Difficulty = "easy",
            Tag = "Math",
            RuntimeLimit = 2000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>
            {
                new TestCaseDto { Input = "1 2", ExpectedOutput = "3" }
            }
        };

        // Act
        await _adminService.AddProblem(problemDto);

        // Assert
        var problemInDb = await _appDbContext.Problems.FirstOrDefaultAsync(p => p.Name == "New Problem");
        problemInDb.Should().NotBeNull();
        problemInDb.Name.Should().Be("New Problem");
        problemInDb.TestCases.Should().HaveCount(1);
    }

    [Fact]
    public async Task EditProblem_ShouldEditProblem_WhenProblemExists()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Old Problem",
            ProblemText = "Old text",
            Difficulty = "easy",
            Tag = "old",
            RuntimeLimit = 500,
            MemoryLimit = 128,
            TestCases = new List<TestCase>()
        };

        _appDbContext.Problems.Add(problem);
        await _appDbContext.SaveChangesAsync();

        var editProblemDto = new AddProblemDto
        {
            Name = "Updated Problem",
            ProblemText = "Problem Text",
            Difficulty = "Hard",
            Tag = "Algorithms",
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>
            {
                new TestCaseDto { Input = "1 3", ExpectedOutput = "4" }
            }
        };

        // Act
        await _adminService.EditProblem(1, editProblemDto);

        // Assert
        var updatedProblem = await _appDbContext.Problems.FirstOrDefaultAsync(p => p.Id == 1);
        updatedProblem.Should().NotBeNull();
        updatedProblem.Name.Should().Be("Updated Problem");
        updatedProblem.Difficulty.Should().Be("Hard");
        updatedProblem.TestCases.Should().HaveCount(1);
    }

    [Fact]
    public async Task EditProblem_ShouldThrowException_WhenProblemDoesNotExist()
    {
        // Arrange
        var editProblemDto = new AddProblemDto
        {
            Name = "Updated Problem",
            ProblemText = "Problem Text",
            Difficulty = "Hard",
            Tag = "Algorithms",
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>()
        };

        // Act & Assert
        var act = async () => await _adminService.EditProblem(999, editProblemDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Problem not found");
    }

    [Fact]
    public async Task DeleteProblem_ShouldRemove_WhenProblemExists()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "Test problem text",
            Difficulty = "easy",
            Tag = "test",
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCase>()
        };

        _appDbContext.Problems.Add(problem);
        await _appDbContext.SaveChangesAsync();

        // Act
        await _adminService.DeleteProblem(1);

        // Assert
        var deletedProblem = await _appDbContext.Problems.FirstOrDefaultAsync(p => p.Id == 1);
        deletedProblem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteProblem_ShouldThrowException_WhenProblemDoesNotExist()
    {
        // Act & Assert
        var act = async () => await _adminService.DeleteProblem(999);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Problem not found");
    }

    public void Dispose()
    {
        _appDbContext.Dispose();
    }
}
