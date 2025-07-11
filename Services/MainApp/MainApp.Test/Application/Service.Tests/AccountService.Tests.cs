using FakeItEasy;
using FluentAssertions;
using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.JWT;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Test.Application.Service.Tests;

public class AccountServiceTests
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _userManager = A.Fake<UserManager<User>>();
        _jwtService = A.Fake<IJwtService>();
        _accountService = new AccountService(_userManager, _jwtService);
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

        A.CallTo(() => _userManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Success));
        A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, "User"))
            .Returns(Task.FromResult(IdentityResult.Success));

        // Act
        var act = async () => await _accountService.Register(registrationDto);

        // Assert
        await act.Should().NotThrowAsync();

        A.CallTo(() => _userManager.CreateAsync(A<User>.That.Matches(u => 
            u.Email == registrationDto.Email && 
            u.UserName == registrationDto.UserName), registrationDto.Password))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, "User"))
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
        A.CallTo(() => _userManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult(existingUser));

        // Act
        var act = async () => await _accountService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Email already exists");

        A.CallTo(() => _userManager.CreateAsync(A<User>._, A<string>._))
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
        A.CallTo(() => _userManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult(existingUser));

        // Act
        var act = async () => await _accountService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("username already exists");

        A.CallTo(() => _userManager.CreateAsync(A<User>._, A<string>._))
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

        A.CallTo(() => _userManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Failed(identityErrors)));

        // Act
        var act = async () => await _accountService.Register(registrationDto);

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("Password too weak\nUsername invalid");

        A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, A<string>._))
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

        A.CallTo(() => _userManager.FindByEmailAsync(registrationDto.Email))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.FindByNameAsync(registrationDto.UserName))
            .Returns(Task.FromResult<User>(null));
        A.CallTo(() => _userManager.CreateAsync(A<User>._, registrationDto.Password))
            .Returns(Task.FromResult(IdentityResult.Success));
        A.CallTo(() => _userManager.AddToRoleAsync(A<User>._, "User"))
            .Returns(Task.FromResult(IdentityResult.Failed(roleErrors)));

        // Act
        var act = async () => await _accountService.Register(registrationDto);

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

        A.CallTo(() => _userManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _userManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _userManager.GetRolesAsync(user))
            .Returns(Task.FromResult<IList<string>>(userRoles));
        A.CallTo(() => _jwtService.CreateJwt(user, userRoles))
            .Returns(expectedJwt);

        // Act
        var result = await _accountService.Login(userLoginDto);

        // Assert
        result.Should().Be(expectedJwt);
        A.CallTo(() => _jwtService.CreateJwt(user, userRoles))
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

        A.CallTo(() => _userManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult<User>(null));

        // Act
        var act = async () => await _accountService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Email or Password");

        A.CallTo(() => _userManager.CheckPasswordAsync(A<User>._, A<string>._))
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

        A.CallTo(() => _userManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _userManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(false));

        // Act
        var act = async () => await _accountService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Invalid Email or Password");

        A.CallTo(() => _userManager.GetRolesAsync(A<User>._))
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

        A.CallTo(() => _userManager.FindByEmailAsync(userLoginDto.Email))
            .Returns(Task.FromResult(user));
        A.CallTo(() => _userManager.CheckPasswordAsync(user, userLoginDto.Password))
            .Returns(Task.FromResult(true));
        A.CallTo(() => _userManager.GetRolesAsync(user))
            .Returns(Task.FromResult<IList<string>>(userRoles));
        A.CallTo(() => _jwtService.CreateJwt(user, userRoles))
            .Throws(new Exception("JWT generation failed"));

        // Act
        var act = async () => await _accountService.Login(userLoginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Failed To Generate JWT Token");
    }
    
}
