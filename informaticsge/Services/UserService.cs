using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

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
        
        var getSubmissions = submissions.Select(sub => new GetSubmissionsResponseDto
        {
            Id = sub.Id,
            AuthUsername = sub.AuthUsername,
            ProblemName = sub.ProblemName,
            Status = sub.Status
        }).ToList();

        return getSubmissions;
    }


}