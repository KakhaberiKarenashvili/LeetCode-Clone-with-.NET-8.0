using MainApp.Application.Features.Users.Commands.ConfirmEmail;
using MainApp.Application.Features.Users.Commands.ForgotPassword;
using MainApp.Application.Features.Users.Commands.LoginUser;
using MainApp.Application.Features.Users.Commands.RegisterUser;
using MainApp.Application.Features.Users.Commands.ResetPassword;
using MainApp.Application.Features.Users.Commands.SetPassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Site;

[Route("api/site/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(token);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok("Email confirmed successfully");
    }

    [HttpPost("set-password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok("Password set successfully");
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok("If an account with that email exists, a reset link has been sent");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok("Password reset successfully");
    }
}
