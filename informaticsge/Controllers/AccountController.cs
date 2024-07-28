using informaticsge.Dto;
using informaticsge.Dto.Request;
using informaticsge.Services;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost("/regiser")]
    public async Task<IActionResult> Register([FromBody]RegistrationDto newuser)
    { 
        _logger.LogInformation("User Registration initiated Username: {username}", newuser.UserName);
        
        try
        {
            await _accountService.Register(newuser);
            
            _logger.LogInformation("User Registration Successful Username: {username}", newuser.UserName);
            
            return Created();
        }
        catch (Exception e)
        {
            _logger.LogError("User Registration Failed Username: {username}", newuser.UserName);
           
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
            
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDto userLogin)
    { 
        _logger.LogInformation("User Login Initiated {email}",userLogin.Email);

        try
        {
            var login = await _accountService.Login(userLogin);
           
            _logger.LogInformation("User Login Successful {email}", userLogin.Email);
        
            return Ok(login);
        }
        catch (Exception ex)
        {
            _logger.LogError("User Login Unsuccessful {email}", userLogin.Email);
            
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }

}