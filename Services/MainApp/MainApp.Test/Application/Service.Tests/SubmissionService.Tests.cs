using System.Security.Claims;
using BuildingBlocks.Messaging.Events;
using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Test.Application.Service.Tests;

public class SubmissionServiceTests
{
     private readonly AppDbContext _fakeDbContext;
    private readonly IPublishEndpoint _fakePublishEndpoint;
    private readonly SubmissionService _sut;

    public SubmissionServiceTests()
    {
        // Setup in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _fakeDbContext = new AppDbContext(options);
        _fakePublishEndpoint = A.Fake<IPublishEndpoint>();
        _sut = new SubmissionService(_fakeDbContext, _fakePublishEndpoint);
    }

    [Fact]
    public async Task HandleSubmission_ShouldSaveSubmissionAndPublishEvent_WhenValidSubmissionProvided()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 256,
            RuntimeLimit = 1000,
            TestCases = new List<TestCase>
            {
                new() { Input = "input1", ExpectedOutput = "output1" },
                new() { Input = "input2", ExpectedOutput = "output2" }
            }
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "testuser"),
            new Claim("Id", "user123")
        }));

        // Act
        await _sut.HandleSubmission(submissionDto, user);

        // Assert
        var savedSubmission = await _fakeDbContext.Submissions.FirstOrDefaultAsync();
        savedSubmission.Should().NotBeNull();
        savedSubmission.AuthUsername.Should().Be("testuser");
        savedSubmission.Code.Should().Be("test code");
        savedSubmission.ProblemId.Should().Be(1);
        savedSubmission.Language.Should().Be("C++");
        savedSubmission.Status.Should().Be("Testing");
        savedSubmission.UserId.Should().Be("user123");

        A.CallTo(() => _fakePublishEndpoint.Publish(A<CppSubmissionRequestedEvent>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleSubmission_ShouldThrowInvalidOperationException_WhenProblemNotFound()
    {
        // Arrange
        var submissionDto = new SubmissionDto
        {
            ProblemId = 999,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "testuser"),
            new Claim("Id", "user123")
        }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.HandleSubmission(submissionDto, user));
        
        exception.Message.Should().Be("problem not found");
    }

    [Fact]
    public async Task HandleSubmission_ShouldPublishCppEvent_WhenLanguageIsCpp()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 256,
            RuntimeLimit = 1000,
            TestCases = new List<TestCase>
            {
                new() { Input = "input1", ExpectedOutput = "output1" }
            }
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "testuser"),
            new Claim("Id", "user123")
        }));

        // Act
        await _sut.HandleSubmission(submissionDto, user);

        // Assert
        A.CallTo(() => _fakePublishEndpoint.Publish(
            A<CppSubmissionRequestedEvent>.That.Matches(e => 
                e.Code == "test code" && 
                e.MemoryLimitMb == 256 && 
                e.TimeLimitMs == 1000 &&
                e.Testcases.Count == 1),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleSubmission_ShouldPublishPythonEvent_WhenLanguageIsPython()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 512,
            RuntimeLimit = 2000,
            TestCases = new List<TestCase>
            {
                new() { Input = "input1", ExpectedOutput = "output1" }
            }
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "print('hello')",
            Language = "Python"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "pythonuser"),
            new Claim("Id", "user456")
        }));

        // Act
        await _sut.HandleSubmission(submissionDto, user);

        // Assert
        A.CallTo(() => _fakePublishEndpoint.Publish(
            A<PythonSubmissionRequestedEvent>.That.Matches(e => 
                e.Code == "print('hello')" && 
                e.MemoryLimitMb == 512 && 
                e.TimeLimitMs == 2000 &&
                e.Testcases.Count == 1),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleSubmission_ShouldMapTestCasesCorrectly_WhenProblemHasMultipleTestCases()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 256,
            RuntimeLimit = 1000,
            TestCases = new List<TestCase>
            {
                new() { Input = "1 2", ExpectedOutput = "3" },
                new() { Input = "5 7", ExpectedOutput = "12" },
                new() { Input = "10 15", ExpectedOutput = "25" }
            }
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "testuser"),
            new Claim("Id", "user123")
        }));

        // Act
        await _sut.HandleSubmission(submissionDto, user);

        // Assert
        A.CallTo(() => _fakePublishEndpoint.Publish(
            A<CppSubmissionRequestedEvent>.That.Matches(e => 
                e.Testcases.Count == 3 &&
                e.Testcases[0].Input == "1 2" &&
                e.Testcases[0].ExpectedOutput == "3" &&
                e.Testcases[1].Input == "5 7" &&
                e.Testcases[1].ExpectedOutput == "12" &&
                e.Testcases[2].Input == "10 15" &&
                e.Testcases[2].ExpectedOutput == "25"),
            A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task HandleSubmission_ShouldThrowInvalidOperationException_WhenUserNameClaimMissing()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 256,
            RuntimeLimit = 1000,
            TestCases = new List<TestCase>()
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("Id", "user123")
            // Missing UserName claim
        }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.HandleSubmission(submissionDto, user));
        
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleSubmission_ShouldThrowInvalidOperationException_WhenUserIdClaimMissing()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 1,
            Name = "Test Problem",
            ProblemText = "test problem text",
            MemoryLimit = 256,
            RuntimeLimit = 1000,
            TestCases = new List<TestCase>()
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 1,
            Code = "test code",
            Language = "C++"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "testuser")
            // Missing Id claim
        }));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.HandleSubmission(submissionDto, user));
        
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleSubmission_ShouldSetCorrectSubmissionProperties_WhenValidDataProvided()
    {
        // Arrange
        var problem = new Problem
        {
            Id = 5,
            Name = "Algorithm Problem",
            ProblemText = "test problem text",
            MemoryLimit = 128,
            RuntimeLimit = 500,
            TestCases = new List<TestCase>()
        };

        await _fakeDbContext.Problems.AddAsync(problem);
        await _fakeDbContext.SaveChangesAsync();

        var submissionDto = new SubmissionDto
        {
            ProblemId = 5,
            Code = "complex algorithm code",
            Language = "Python"
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("UserName", "algorithmuser"),
            new Claim("Id", "user789")
        }));

        // Act
        await _sut.HandleSubmission(submissionDto, user);

        // Assert
        var savedSubmission = await _fakeDbContext.Submissions.FirstOrDefaultAsync();
        savedSubmission.Should().NotBeNull();
        savedSubmission.AuthUsername.Should().Be("algorithmuser");
        savedSubmission.Code.Should().Be("complex algorithm code");
        savedSubmission.ProblemId.Should().Be(5);
        savedSubmission.Language.Should().Be("Python");
        savedSubmission.ProblemName.Should().Be("Algorithm Problem");
        savedSubmission.Status.Should().Be("Testing");
        savedSubmission.UserId.Should().Be("user789");
    }

    public void Dispose()
    {
        _fakeDbContext.Dispose();
    }

}