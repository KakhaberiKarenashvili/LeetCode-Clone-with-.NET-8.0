using MainApp.Application.Features.Cms.Problems.CreateProblem;
using MainApp.Application.Features.Cms.Problems.DeleteProblem;
using MainApp.Application.Features.Cms.Problems.GetProblemById;
using MainApp.Application.Features.Cms.Problems.GetProblems;
using MainApp.Application.Features.Cms.Problems.UpdateProblem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Cms;

[Route("api/cms/problems")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class CmsProblemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CmsProblemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProblems([FromQuery] GetProblemsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        var result = await _mediator.Send(new GetProblemByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProblem([FromBody] CreateProblemCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProblem(int id, [FromBody] UpdateProblemCommand command)
    {
        await _mediator.Send(command with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProblem(int id)
    {
        await _mediator.Send(new DeleteProblemCommand(id));
        return NoContent();
    }
}
