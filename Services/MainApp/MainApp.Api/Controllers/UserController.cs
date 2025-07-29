using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;


[Route("api/[controller]")]
[ApiController]

[Authorize(AuthenticationSchemes = "Bearer")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController( UserService userService)
    {

        _userService = userService;
    }
    
    [HttpGet("/account")]
    public async Task<IActionResult> Account()
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        var user = await _userService.MyAccount(userid);
        
        return  Ok(user);
    }

    [HttpGet("/submissions")]
    public async Task<IActionResult> MySubmissions()
    {
            var userid = User.Claims.First(user => user.Type == "Id").Value;
            
            var submissions = await _userService.MySubmissions(userid);

            return Ok(submissions);
    }

    [HttpPut("/change-email/{email}")]
    public async Task<IActionResult> ChangeEmail(string email)
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        await _userService.ChangeEmail(userid, email);
        
        return Ok("email changed");
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;

        await _userService.ChangePassword(userid, changePasswordDto);

        return Ok("password changed");
    }
}