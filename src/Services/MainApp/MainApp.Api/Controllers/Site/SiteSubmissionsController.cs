using MainApp.Application.Features.Submissions.Commands;
using MainApp.Application.Features.Submissions.Commands.CreateSubmission;
using MainApp.Application.Features.Submissions.Queries;
using MainApp.Application.Features.Submissions.Queries.GetSubmissionById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Site;

[Route("api/site/submissions")]
[ApiController]
public class SiteSubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SiteSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubmission(int id)
    {
        var result = await _mediator.Send(new GetSubmissionByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Submit([FromBody] CreateSubmissionCommand command)
    {
        var userId = User.Claims.First(c => c.Type == "Id").Value;
        var username = User.Claims.First(c => c.Type == "UserName").Value;

        await _mediator.Send(command with { UserId = userId, Username = username });
        return Accepted("Submission received and being processed.");
    }
}
