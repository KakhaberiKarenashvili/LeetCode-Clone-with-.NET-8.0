using informaticsge.Dto;
using informaticsge.models;
using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;
    
    public UserController(UserManager<User> userManager, UserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    [HttpGet("/hello")]
    [Authorize]
    public async Task<User> Hello()
    {
        
        var userid = User.Claims.First(user => user.Type == "Email");
        var test = userid.Value;
        var user = await _userManager.FindByEmailAsync(test);
        Console.WriteLine(test);
        return user;
    }
    
    [HttpGet("/myaccount")]
    [Authorize]
    public async Task<MyAccountDto> MyAccount()
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        var user = await _userService.MyAccount(userid);
        return  user;
    }

    [HttpGet("/submissions")]
    [Authorize]
    public async Task<IActionResult> MySubmissions()
    {
        try
        {
            var userid = User.Claims.First(user => user.Type == "Id").Value;
            var solutions = await _userService.MySubmissions(userid);

            return Ok(solutions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}