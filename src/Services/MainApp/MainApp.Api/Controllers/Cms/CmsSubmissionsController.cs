using MainApp.Application.Features.Submissions.Commands;
using MainApp.Application.Features.Submissions.Commands.DeleteSubmission;
using MainApp.Application.Features.Submissions.Queries;
using MainApp.Application.Features.Submissions.Queries.GetAllSubmissions;
using MainApp.Application.Features.Submissions.Queries.GetSubmissionById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Cms;

[Route("api/cms/submissions")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class CmsSubmissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CmsSubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubmissions([FromQuery] GetAllSubmissionsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubmission(int id)
    {
        var result = await _mediator.Send(new GetSubmissionByIdQuery(id));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubmission(int id)
    {
        await _mediator.Send(new DeleteSubmissionCommand(id));
        return NoContent();
    }
}
