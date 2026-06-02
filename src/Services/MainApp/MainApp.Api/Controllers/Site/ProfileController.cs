using MainApp.Application.Features.Users.Commands;
using MainApp.Application.Features.Users.Commands.ChangeEmail;
using MainApp.Application.Features.Users.Commands.ChangePassword;
using MainApp.Application.Features.Users.Queries;
using MainApp.Application.Features.Users.Queries.GetMyAccount;
using MainApp.Application.Features.Users.Queries.GetMySubmissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Site;

[Route("api/site/profile")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.Claims.First(c => c.Type == "Id").Value;
        var result = await _mediator.Send(new GetMyAccountQuery(userId));
        return Ok(result);
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetMySubmissions([FromQuery] GetMySubmissionsQuery query)
    {
        var userId = User.Claims.First(c => c.Type == "Id").Value;
        var result = await _mediator.Send(query with { UserId = userId });
        return Ok(result);
    }

    [HttpPut("email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command)
    {
        var userId = User.Claims.First(c => c.Type == "Id").Value;
        await _mediator.Send(command with { UserId = userId });
        return Ok("Email changed");
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var userId = User.Claims.First(c => c.Type == "Id").Value;
        await _mediator.Send(command with { UserId = userId });
        return Ok("Password changed");
    }
}
