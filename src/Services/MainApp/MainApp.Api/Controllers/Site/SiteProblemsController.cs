using MainApp.Application.Features.Problems.Queries;
using MainApp.Application.Features.Problems.Queries.GetProblemById;
using MainApp.Application.Features.Problems.Queries.GetProblems;
using MainApp.Application.Features.Problems.Queries.GetProblemSubmissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers.Site;

[Route("api/site/problems")]
[ApiController]
public class SiteProblemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SiteProblemsController(IMediator mediator)
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

    [HttpGet("{id}/submissions")]
    public async Task<IActionResult> GetProblemSubmissions(int id, [FromQuery] GetProblemSubmissionsQuery query)
    {
        var result = await _mediator.Send(query with { ProblemId = id });
        return Ok(result);
    }
}
