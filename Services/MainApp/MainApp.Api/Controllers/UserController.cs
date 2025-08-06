using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;


[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    
    public UserController( UserService userService)
    {

        _userService = userService;
    }
    
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegistrationDto newuser)
    { 
        await _userService.Register(newuser);

        return Created();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDto userLogin)
    { 
        var login = await _userService.Login(userLogin);
            
        return Ok(login);
    }
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("profile")]
    public async Task<IActionResult> Account()
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        var user = await _userService.MyAccount(userid);
        
        return  Ok(user);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("submissions")]
    public async Task<IActionResult> MySubmissions()
    {
            var userid = User.Claims.First(user => user.Type == "Id").Value;
            
            var submissions = await _userService.MySubmissions(userid);

            return Ok(submissions);
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut("change-email/{email}")]
    public async Task<IActionResult> ChangeEmail(string email)
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        await _userService.ChangeEmail(userid, email);
        
        return Ok("email changed");
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;

        await _userService.ChangePassword(userid, changePasswordDto);

        return Ok("password changed");
    }
}