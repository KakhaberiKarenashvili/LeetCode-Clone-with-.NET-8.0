using BuildingBlocks.Common.Enums;
using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MainApp.Infrastructure.JWT;
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
    private readonly IJwtService _fakeJwtService;

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

        _fakeJwtService = A.Fake<IJwtService>();


        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _fakeDbContext = new AppDbContext(options);

        _userService = new UserService(_fakeDbContext, _fakeUserManager, _fakeSignInManager, _fakeJwtService);
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
            new Submissions { Id = 1, UserId = userId, AuthUsername = "author1", Language = "C++", Code = "Testcode", ProblemId = 1,  ProblemName = "Problem1", Status = Status.TestPassed, },
            new Submissions { Id = 2, UserId = userId, AuthUsername = "author2",  Language = "Python", Code = "Testcode", ProblemId = 2, ProblemName = "Problem2", Status = Status.TestRunning }
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
        result[0].Status.Should().Be("TestPassed");

        result[1].Id.Should().Be(2);
        result[1].AuthUsername.Should().Be("author2");
        result[1].ProblemName.Should().Be("Problem2");
        result[1].Status.Should().Be("TestRunning");;
    }
    
    [Fact]
    public async Task Register_ShouldSucceed_WhenUserIsValid()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "Password123!"
        };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Success));
        A.CallTo(() => _fakeUserManager.AddToRoleAsync(A<User>._, "User"))
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var act = async () => await _userService.Register(registrationDto);

        // Assert
        await act.Should().NotThrowAsync();

        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>.That.Matches(u => 
            u.Email == registrationDto.Email && 
            u.UserName == registrationDto.UserName), registrationDto.Password))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUserManager.AddToRoleAsync(A<User>._, "User"))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Email = "existing@example.com",
            UserName = "testuser",
            Password = "Password123!"
        };

        var existingUser = new User { Email = "existing@example.com" };
        A.CallTo(() => _fakeUserManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult(existingUser));

        // Act
        var act = async () => await _userService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Email already exists");

        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, A<string>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Register_ShouldThrowException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Email = "test@example.com",
            UserName = "existinguser",
            Password = "Password123!"
        };

        var existingUser = new User { UserName = "existinguser" };
        A.CallTo(() => _fakeUserManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult(existingUser));

        // Act
        var act = async () => await _userService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("username already exists");

        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, A<string>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Register_ShouldThrowAggregateException_WhenUserCreationFails()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "Password123!"
        };

        var identityErrors = new[]
        {
            new IdentityError { Description = "Password too weak" },
            new IdentityError { Description = "Username invalid" }
        };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Failed(identityErrors)));

        // Act
        var act = async () => await _userService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("Password too weak\nUsername invalid");

        A.CallTo(() => _fakeUserManager.AddToRoleAsync(A<User>._, A<string>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Register_ShouldThrowInvalidOperationException_WhenRoleAssignmentFails()
    {
        // Arrange
        var registrationDto = new RegistrationDto
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "Password123!"
        };

        var roleErrors = new[]
        {
            new IdentityError { Description = "Role does not exist" }
        };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Success));
        A.CallTo(() => _fakeUserManager.AddToRoleAsync(A<User>._, "User"))
            .Returns(Task.FromResult(IdentityResult.Failed(roleErrors)));

        // Act
        var act = async () => await _userService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Failed To Assign Role.");
    }
    
    [Fact]
    public async Task Login_ShouldReturnJwtToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userLoginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User { Email = "test@example.com", UserName = "testuser" };
        var userRoles = new List<string> { "User" };
        const string expectedJwt = "jwt.token.here";

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _fakeUserManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _fakeUserManager.GetRolesAsync(user))
            .Returns(Task.FromResult<IList<string>>(userRoles));
        A.CallTo(() => _fakeJwtService.CreateJwt(user, userRoles))
            .Returns(expectedJwt);

        // Act
        var result = await _userService.Login(userLoginDto);

        // Assert
        result.Should().Be(expectedJwt);
        A.CallTo(() => _fakeJwtService.CreateJwt(user, userRoles))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Login_ShouldThrowInvalidOperationException_WhenEmailDoesNotExist()
    {
        // Arrange
        var userLoginDto = new UserLoginDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult<User>(null));

        // Act
        var act = async () => await _userService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Email or Password");

        A.CallTo(() => _fakeUserManager.CheckPasswordAsync(A<User>._, A<string>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Login_ShouldThrowInvalidOperationException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var userLoginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new User { Email = "test@example.com", UserName = "testuser" };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _fakeUserManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(false));

        // Act
        var act = async () => await _userService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Email or Password");

        A.CallTo(() => _fakeUserManager.GetRolesAsync(A<User>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task Login_ShouldThrowException_WhenJwtGenerationFails()
    {
        // Arrange
        var userLoginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new User { Email = "test@example.com", UserName = "testuser" };
        var userRoles = new List<string> { "User" };

        A.CallTo(() => _fakeUserManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _fakeUserManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _fakeUserManager.GetRolesAsync(user))
            .Returns(Task.FromResult<IList<string>>(userRoles));
        A.CallTo(() => _fakeJwtService.CreateJwt(user, userRoles))
            .Throws(new Exception("JWT generation failed"));

        // Act
        var act = async () => await _userService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Failed To Generate JWT Token");
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
