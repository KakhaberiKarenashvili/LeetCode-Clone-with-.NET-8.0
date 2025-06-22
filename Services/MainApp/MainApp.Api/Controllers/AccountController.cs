using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

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
            await _accountService.Register(newuser);

            return Created();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDto userLogin)
    { 
            var login = await _accountService.Login(userLogin);
            
            return Ok(login);
    }
    

}