using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;


[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UserController> _logger;
    
    public UserController( UserService userService, ILogger<UserController> logger)
    {

        _userService = userService;
        _logger = logger;
    }
    
    [HttpGet("/account")]
    [Authorize]
    public async Task<IActionResult> Account()
    {
        var userid = User.Claims.First(user => user.Type == "Id").Value;
        
        _logger.LogInformation("User Requested MyAccount UserId: {id}", userid);
        
        var user = await _userService.MyAccount(userid);
        
        return  Ok(user);
    }

    [HttpGet("/submissions")]
    [Authorize]
    public async Task<IActionResult> MySubmissions()
    {
            var userid = User.Claims.First(user => user.Type == "Id").Value;
            
            _logger.LogInformation("User Requested MySubmissions UserId: {id}", userid);

            var submissions = await _userService.MySubmissions(userid);

            return Ok(submissions);

    }
}