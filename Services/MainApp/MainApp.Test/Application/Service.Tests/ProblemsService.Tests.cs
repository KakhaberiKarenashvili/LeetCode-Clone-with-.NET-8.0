using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Test.Application.Service.Tests;

public class ProblemsServiceTests
{
    private readonly AppDbContext _fakeDbContext;
    private readonly ProblemsService _problemsService;

    public ProblemsServiceTests()
    {

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase1")
            .Options;

        _fakeDbContext = new AppDbContext(options);
        
        _problemsService = new ProblemsService(_fakeDbContext);
    }

    [Fact]
    public async Task GetAllProblems_ShouldReturnProblems_WhenDbIsNotEmpty()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 3;
        var totalPages = 1;
        var name = "";
        var difficulty = "";
        List<string> category = [];
        
        var problems = new List<Problem>
        {
            new Problem
            {
                Id = 101,
                Name = "FizzBuzz",
                ProblemText =
                    "Write a program that prints numbers from 1 to 100. For multiples of three print 'Fizz' instead of the number and for the multiples of five print 'Buzz'. For numbers which are multiples of both three and five print 'FizzBuzz'.",
                Category = Category.Graphs,
                Difficulty = Difficulty.Easy,
                RuntimeLimit = 500, // ms
                MemoryLimit = 64, // MB
                TestCases = new List<TestCase>
                {
                    new TestCase { Id = 1, Input = "1", ExpectedOutput = "1" },
                    new TestCase { Id = 2, Input = "3", ExpectedOutput = "Fizz" },
                    new TestCase { Id = 3, Input = "5", ExpectedOutput = "Buzz" },
                    new TestCase { Id = 4, Input = "15", ExpectedOutput = "FizzBuzz" }
                }
            },
            new Problem
            {
                Id = 205,
                Name = "Detect Cycle in Linked List",
                ProblemText =
                    "Given the head of a linked list, return true if there is a cycle in the linked list. Otherwise, return false.",
                Category = Category.Arrays,
                Difficulty = Difficulty.Medium,
                RuntimeLimit = 1200, // ms
                MemoryLimit = 256, // MB
                TestCases = new List<TestCase>
                {
                    new TestCase { Id = 5, Input = "1->2->3->2 (cycle)", ExpectedOutput = "true" },
                    new TestCase { Id = 6, Input = "1->2->3->4 (no cycle)", ExpectedOutput = "false" }
                }
            },
            new Problem
            {
                Id = 312,
                Name = "Longest Palindromic Substring",
                ProblemText =
                    "Given a string s, return the longest palindromic substring in s. A substring is a contiguous non-empty sequence of characters within the original string.",
                Category = Category.Strings,
                Difficulty = Difficulty.Hard,
                RuntimeLimit = 3000, // ms
                MemoryLimit = 512, // MB
                TestCases = new List<TestCase>
                {
                    new TestCase { Id = 7, Input = "babad", ExpectedOutput = "bab" }, // "aba" is also valid
                    new TestCase { Id = 8, Input = "cbbd", ExpectedOutput = "bb" },
                    new TestCase { Id = 9, Input = "a", ExpectedOutput = "a" }
                }
            }
        };
        _fakeDbContext.Problems.AddRange(problems);
        _fakeDbContext.SaveChanges();
        
        // Act
        var result = await _problemsService.GetAllProblems(pageNumber, pageSize, name, difficulty, category);
        
        // Assert
        result.Should().NotBeNull();;
        result.TotalPages.Should().Be(totalPages);
        result.TotalCount.Should().Be(totalCount);
    }
    
    
    [Fact]
    public async Task GetAllProblems_ShouldNotReturnProblems_WhenDbIsEmpty()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 0;
        var totalPages = 0;
        var name = "";
        var difficulty = "";
        List<string> category = [];
        // Act
        var result = await _problemsService.GetAllProblems(pageNumber, pageSize, name, difficulty, category);;
        // Assert

        result.Should().NotBeNull();;
        result.TotalPages.Should().Be(totalPages);
        result.TotalCount.Should().Be(totalCount);
    }

    [Fact]
    public async Task GetProblem_ShouldReturnProblem_WhenCorrectId()
    {
        // Arrange
        var problemId = 101;
        
        // Act
        var result = await _problemsService.GetProblem(problemId);
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(problemId);
        result.Name.Should().Be("FizzBuzz");
        result.ProblemText.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetProblem_ShouldThrowException_WhenIncorrectId()
    {
        // Arrange
        var problemId = 1000;
        // Act
        Func<Task> act = async () => await _problemsService.GetProblem(problemId);
        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    public async Task GetSubmissions_WhenProblemExists_ReturnsSubmissionsList()
    {
        // Arrange
        int problemId = 101;
        var pageNumber = 1;
        var pageSize = 10;
        var totalCount = 1;
        var totalPages = 1;
        var status = "TestPassed";
        var language = "";

        var mockProblemsData = new List<Problem>
        {
            new Problem { Id = problemId, Name = "Sample Problem" }
        };

        var mockSubmissionsData = new List<Submissions>
        {
            new Submissions { Id = 1, ProblemId = problemId, AuthUsername = "User1", ProblemName = "FizzBuzz", Status = Status.TestPassed },
            new Submissions { Id = 2, ProblemId = problemId, AuthUsername = "User2", ProblemName = "FizzBuzz", Status = Status.TestFailed }
        };
        
        _fakeDbContext.Submissions.AddRange(mockSubmissionsData);

        _fakeDbContext.SaveChanges();
        
        // Act
        var result = await _problemsService.GetSubmissions(problemId, pageNumber, pageSize, status, language);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(totalCount);
        result.TotalPages.Should().Be(totalPages);
        result.Items.Should().HaveCount(1);
        result.Items[0].AuthUsername.Should().Be("User1"); 
        
    }
    
      [Fact]
    public async Task AddProblem_ShouldAddProblem_WhenProblemIsValid()
    {
        // Arrange
        var problemDto = new AddProblemDto
        {
            Name = "New Problem",
            ProblemText = "Test Problem",
            Difficulty = "Easy",
            Categories = ["Arrays","Strings"],
            RuntimeLimit = 2000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>
            {
                new TestCaseDto { Input = "1 2", ExpectedOutput = "3" }
            }
        };

        // Act
        await _problemsService.AddProblem(problemDto);

        // Assert
        var problemInDb = await _fakeDbContext.Problems.FirstOrDefaultAsync(p => p.Name == "New Problem");
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
            Difficulty = Difficulty.Easy,
            Category = Category.Arrays,
            RuntimeLimit = 500,
            MemoryLimit = 128,
            TestCases = new List<TestCase>()
        };

        _fakeDbContext.Problems.Add(problem);
        await _fakeDbContext.SaveChangesAsync();

        var editProblemDto = new AddProblemDto
        {
            Name = "Updated Problem",
            ProblemText = "Problem Text",
            Difficulty = "Easy",
            Categories = ["Arrays","Strings"],
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>
            {
                new TestCaseDto { Input = "1 3", ExpectedOutput = "4" }
            }
        };

        // Act
        await _problemsService.EditProblem(1, editProblemDto);

        // Assert
        var updatedProblem = await _fakeDbContext.Problems.FirstOrDefaultAsync(p => p.Id == 1);
        updatedProblem.Should().NotBeNull();
        updatedProblem.Name.Should().Be("Updated Problem");
        updatedProblem.Difficulty.Should().Be(Difficulty.Easy);
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
            Difficulty = "Easy",
            Categories = ["Arrays","Strings"],
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCaseDto>()
        };

        // Act & Assert
        var act = async () => await _problemsService.EditProblem(999, editProblemDto);
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
            Difficulty = Difficulty.Medium,
            Category = Category.Arrays,
            RuntimeLimit = 1000,
            MemoryLimit = 256,
            TestCases = new List<TestCase>()
        };

        _fakeDbContext.Problems.Add(problem);
        await _fakeDbContext.SaveChangesAsync();

        // Act
        await _problemsService.DeleteProblem(1);

        // Assert
        var deletedProblem = await _fakeDbContext.Problems.FirstOrDefaultAsync(p => p.Id == 1);
        deletedProblem.Should().BeNull();
    }

    [Fact]
    public async Task DeleteProblem_ShouldThrowException_WhenProblemDoesNotExist()
    {
        // Act & Assert
        var act = async () => await _problemsService.DeleteProblem(999);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Problem not found");
    }

    public async Task GetSubmissions_ShouldThrowException_WhenProblemDontExists()
    {
        // Arrange
        int problemId = 1000;
        var pageNumber = 1;
        var pageSize = 10;
        // Act
        Func<Task> act = async () => await _problemsService.GetSubmissions(problemId, pageNumber, pageSize, "", "");
        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    
}