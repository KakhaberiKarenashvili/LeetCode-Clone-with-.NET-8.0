using MainApp.Application.Dto.Request;
using MainApp.Application.Dto.Response;
using MainApp.Application.Extensions.Filtering;
using MainApp.Application.Extensions.Pagination;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MainApp.Infrastructure.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MainApp.Application.Services;

public class UserService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;

    public UserService(AppDbContext appDbContext, UserManager<User> userManager,
        SignInManager<User> signInManager, IJwtService jwtService)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }
    
    
    public async Task<string> Login(UserLoginDto userLogin)
    {
        var user = await _userManager.FindByEmailAsync(userLogin.Email);

        if (user == null)
        {
            throw new InvalidOperationException("Invalid Email or Password");
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, userLogin.Password);

        if (checkPassword == false)
        {
            throw new InvalidOperationException("Invalid Email or Password");
        }

        var userRole = await _userManager.GetRolesAsync(user);
        
        try
        {
            var jwt = _jwtService.CreateJwt(user,userRole);
            
            return jwt;
        }
        catch(Exception ex)
        {
            throw new Exception("Failed To Generate JWT Token");
        }
    }
    
    
    public async Task Register(RegistrationDto newUser)
    {
        if (await CheckEmailExists(newUser.Email))
        {
            throw new Exception("Email already exists");
        }
     

        if (await CheckUsernameExists(newUser.UserName))  
        {
            throw new Exception("username already exists");
        }
       
        var userToAdd = new User()
        {
            Email = newUser.Email,
            UserName = newUser.UserName,
        };
       
        //errors are quite tricky hard to debug in case of errors use debugger 
        var createProcess = await _userManager.CreateAsync(userToAdd, newUser.Password);
        
        if (!createProcess.Succeeded)
        {
            var errorMessage = string.Join("\n", createProcess.Errors.Select(e => e.Description));
           
            throw new AggregateException(errorMessage); 
        }

        var roleAssignmentResult = await _userManager.AddToRoleAsync(userToAdd, "User");
      
        if (!roleAssignmentResult.Succeeded)
        {
            var roleErrorMessage = string.Join("\n", roleAssignmentResult.Errors.Select(e => e.Description));
           
            throw new InvalidOperationException("Failed To Assign Role.");
        }
    }

    public async Task<MyAccountResponseDto> MyAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        var account = new MyAccountResponseDto()
        {
            Username = user?.UserName,
            Email = user?.Email 
        };
        return account;
    }


    public async Task<PagedList<GetSubmissionsResponseDto>> MySubmissions(string userId,int pageNumber,
        int pageSize,
        string? status,
        string? language)
    {
        var data =  _appDbContext.Submissions
            .Where(sol => sol.UserId == userId)
            .ApplyFilter(status, language)
            .AsQueryable();

        var submissions = await PagedList<GetSubmissionsResponseDto>
            .CreateAsync(pageNumber,pageSize,data,GetSubmissionsResponseDto.FromSubmission);
        
        return submissions;
    }
    
    public async Task ChangeEmail(string userId, string newEmail)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

        var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to change email: {errors}");
        }
    }

    public async Task ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        var result = await _userManager.
            ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to change password: {errors}");
        }
    }


    private async Task<bool> CheckEmailExists(string email)
    {
        var result =  await _userManager.FindByEmailAsync(email);
     
        if (result == null)
        {
            return false;
        }
        return true;
    }
    
    private async Task<bool> CheckUsernameExists(string username)
    {
        var result = await _userManager.FindByNameAsync(username);
        
        if (result == null)
        {
            return false;
        }
        return true;
    }
    
}