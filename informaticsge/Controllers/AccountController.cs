using informaticsge.models;
using informaticsge.Dto;
using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Register([FromBody]RegistrationDTO newuser)
    {
        try
        {
            var register = await _accountService.Register(newuser);

            return Ok(register);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDTO userLogin)
    {
        Console.WriteLine(userLogin);
        try
        {
            var login = await _accountService.Login(userLogin);
            return Ok(login);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}