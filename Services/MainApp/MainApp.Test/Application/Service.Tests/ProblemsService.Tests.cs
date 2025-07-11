using BuildingBlocks.Common.Enums;
using FluentAssertions;
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
        var result = await _problemsService.GetAllProblems(pageNumber);
        
        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(3);
    }
    
    
    [Fact]
    public async Task GetAllProblems_ShouldNotReturnProblems_WhenDbIsEmpty()
    {
        // Arrange
        var pageNumber = 1;
        // Act
        var result = await _problemsService.GetAllProblems(pageNumber);
        // Assert

        result.Should().BeEmpty();
        result.Should().NotBeNull();
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
        var result = await _problemsService.GetSubmissions(problemId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); 
        result[0].AuthUsername.Should().Be("User1"); 
        result[1].AuthUsername.Should().Be("User2");
    }

    public async Task GetSubmissions_ShouldThrowException_WhenProblemDontExists()
    {
        // Arrange
        int problemId = 1000;
        // Act
        Func<Task> act = async () => await _problemsService.GetSubmissions(problemId);
        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    
    
}