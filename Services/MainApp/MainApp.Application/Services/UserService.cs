using MainApp.Application.Dto.Request;
using MainApp.Application.Dto.Response;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace MainApp.Application.Services;

public class UserService
{
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(AppDbContext appDbContext, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
        _signInManager = signInManager;
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


    public async Task<List<GetSubmissionsResponseDto>> MySubmissions(string userId)
    {
        var submissions = await _appDbContext.Submissions.Where(sol => sol.UserId == userId).ToListAsync();
        
        var getSubmissions = submissions.Select(submissions => new GetSubmissionsResponseDto
        {
            Id = submissions.Id,
            AuthUsername = submissions.AuthUsername,
            ProblemName = submissions.ProblemName,
            Language = submissions.Language,
            SubmissionTime = submissions.SubmissionTime,
            SuccessRate = $"{submissions.SuccessRate}%",
            Status = submissions.Status.ToString(),
        }).ToList();

        return getSubmissions;
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
        
        var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to change password: {errors}");
        }
    }


}