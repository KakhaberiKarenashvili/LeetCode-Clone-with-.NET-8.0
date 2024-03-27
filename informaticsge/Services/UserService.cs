
using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.models;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

public class UserService
{
    private readonly AppDBcontext _appDBcontext;
    private readonly UserManager<User> _userManager;

    public UserService(AppDBcontext appDBcontext, UserManager<User> userManager)
    {
        _appDBcontext = appDBcontext;
        _userManager = userManager;
    }

    public async Task<MyAccountDTO> MyAccount(string Id)
    {
        var user = await _userManager.FindByIdAsync(Id);

        var myaccount = new MyAccountDTO()
        {
            Email = user?.Email ?? string.Empty,
            Username = user?.UserName ?? string.Empty
        };
        return myaccount;
    }


    public async Task<List<Solution>> MySolutions(string UserId)
    {
        var solutions = await _appDBcontext.Solutions.Where(sol => sol.UserId == UserId).ToListAsync();

        return solutions;
    }
}