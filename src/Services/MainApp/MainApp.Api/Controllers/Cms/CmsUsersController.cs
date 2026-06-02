using MainApp.Application.Features.Users.Commands.AdminResetPassword;
using MainApp.Application.Features.Users.Commands.ChangeEmail;
using MainApp.Application.Features.Users.Commands.CreateUser;
using MainApp.Application.Features.Users.Commands.DeleteUser;
using MainApp.Application.Features.Users.Queries.GetUserById;
using MainApp.Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Cms;

[Route("api/cms/users")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class CmsUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CmsUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id}/email")]
    public async Task<IActionResult> ChangeEmail(string id, [FromBody] ChangeEmailCommand command)
    {
        await _mediator.Send(command with { UserId = id });
        return Ok("Email updated");
    }

    [HttpPut("{id}/password")]
    public async Task<IActionResult> ResetPassword(string id, [FromBody] AdminResetPasswordCommand command)
    {
        await _mediator.Send(command with { UserId = id });
        return Ok("Password reset");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}
