using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Services;
using MainApp.Domain.Models;
using MainApp.Infrastructure.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MainApp.Test.Application.Service.Tests;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly AppDbContext _fakeDbContext;
    private readonly UserManager<User> _fakeUserManager;
    private readonly SignInManager<User> _fakeSignInManager;

    public UserServiceTests()
    {
        var fakeUserStore = A.Fake<IUserStore<User>>();
        _fakeUserManager = A.Fake<UserManager<User>>(x => x.WithArgumentsForConstructor(
            new object[]
            {
                fakeUserStore,
                A.Fake<IOptions<IdentityOptions>>(),
                A.Fake<IPasswordHasher<User>>(),
                new List<IUserValidator<User>> { A.Fake<IUserValidator<User>>() },
                new List<IPasswordValidator<User>> { A.Fake<IPasswordValidator<User>>() },
                A.Fake<ILookupNormalizer>(),
                A.Fake<IdentityErrorDescriber>(),
                A.Fake<IServiceProvider>(),
                A.Fake<ILogger<UserManager<User>>>()
            }));
        
        _fakeSignInManager = A.Fake<SignInManager<User>>(x => x.WithArgumentsForConstructor(
            new object[]
            {
                _fakeUserManager,
                A.Fake<IHttpContextAccessor>(),
                A.Fake<IUserClaimsPrincipalFactory<User>>(),
                A.Fake<IOptions<IdentityOptions>>(),
                A.Fake<ILogger<SignInManager<User>>>(),
                A.Fake<IAuthenticationSchemeProvider>(),
                A.Fake<IUserConfirmation<User>>()
            }));



        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _fakeDbContext = new AppDbContext(options);

        _userService = new UserService(_fakeDbContext, _fakeUserManager, _fakeSignInManager);
    }

    [Fact]
    public async Task MyAccount_ShouldReturnCorrectAccountInfo_WhenUserIdIsValid()
    {
        // Arrange
        var userId = "123";
        var fakeUser = new User { Id = userId, UserName = "testuser", Email = "testuser@example.com" };

        A.CallTo(() => _fakeUserManager.FindByIdAsync(userId)).Returns(Task.FromResult(fakeUser));

        // Act
        var result = await _userService.MyAccount(userId);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("testuser@example.com");
    }

    [Fact]
    public async Task MyAccount_ShouldReturnNullValues_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = "invalid";
        A.CallTo(() => _fakeUserManager.FindByIdAsync(userId)).Returns(Task.FromResult<User>(null));

        // Act
        var result = await _userService.MyAccount(userId);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().BeNull();
        result.Email.Should().BeNull();
    }

    [Fact]
    public async Task MySubmissions_ShouldReturnUserSubmissions_WhenUserExists()
    {
        // Arrange
        var userId = "123";
        var submissions = new List<Submissions>
        {
            new Submissions { Id = 1, UserId = userId, AuthUsername = "author1", Language = "C++", Code = "Testcode", ProblemId = 1,  ProblemName = "Problem1", Status = "Completed", },
            new Submissions { Id = 2, UserId = userId, AuthUsername = "author2",  Language = "Python", Code = "Testcode", ProblemId = 2, ProblemName = "Problem2", Status = "Pending" }
        };

        await _fakeDbContext.Submissions.AddRangeAsync(submissions);
        await _fakeDbContext.SaveChangesAsync();

        // Act
        var result = await _userService.MySubmissions(userId);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);

        result[0].Id.Should().Be(1);
        result[0].AuthUsername.Should().Be("author1");
        result[0].ProblemName.Should().Be("Problem1");
        result[0].Status.Should().Be("Completed");

        result[1].Id.Should().Be(2);
        result[1].AuthUsername.Should().Be("author2");
        result[1].ProblemName.Should().Be("Problem2");
        result[1].Status.Should().Be("Pending");
    }

    [Fact]
    public async Task MySubmissions_ShouldReturnEmptyList_WhenUserHasNoSubmissions()
    {
        // Arrange
        var userId = "unknown";

        // Act
        var result = await _userService.MySubmissions(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
