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

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("/regiser")]
    public async Task<IActionResult> Register([FromBody]RegistrationDto newuser)
    {
            var register = await _accountService.Register(newuser);

            return Ok(register);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDto userLogin)
    { 
        var login = await _accountService.Login(userLogin);
        
        return Ok(login);
            
    }

}