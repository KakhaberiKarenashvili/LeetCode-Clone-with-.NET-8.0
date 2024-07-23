
using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.models;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

public class UserService
{
    private readonly AppDBContext _appDbContext;
    private readonly UserManager<User> _userManager;

    public UserService(AppDBContext appDbContext, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }

    public async Task<MyAccountDto> MyAccount(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        var account = new MyAccountDto()
        {
            Email = user?.Email ?? string.Empty,
            Username = user?.UserName ?? string.Empty
        };
        return account;
    }


    public async Task<List<GetSubmissionsDTO>> MySubmissions(string userId)
    {
        var submissions = await _appDbContext.Submissions.Where(sol => sol.UserId == userId).ToListAsync();
        
        var getSubmissions = submissions.Select(sub => new GetSubmissionsDTO
        {
            Id = sub.Id,
            AuthUsername = sub.AuthUsername,
            ProblemName = sub.ProblemName,
            Status = sub.Status
        }).ToList();

        return getSubmissions;
    }
    
}