using informaticsge.Dto.Request;
using informaticsge.Services;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly ProblemsService _problemsService;
    private readonly ILogger<ProblemsController> _logger;

    public ProblemsController(ProblemsService problemsService, ILogger<ProblemsController> logger)
    {
        _problemsService = problemsService;
        _logger = logger;
    }
    
    [HttpGet("/problems")]
    public async Task<IActionResult> GetProblems(int pageNumber)
    {
        _logger.LogInformation("Someone Requested Problems On Page: {page}",pageNumber);
        
        var problems =  await _problemsService.GetAllProblems(pageNumber);

        return Ok(problems);
    }

    [HttpGet("/problems/{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        _logger.LogInformation("Someone Requested Problem With Id: {id}",id);
        
        var problem = await _problemsService.GetProblem(id);
        
        return Ok(problem);
    }
    
    [HttpGet("/problems/{id}/submissions")]
    public async Task<IActionResult> GetSubmissions(int id)
    {
        _logger.LogInformation("Someone Requested Submissions on Problem With Id: {id}",id);
        
        var submissions = await _problemsService.GetSubmissions(id);

        return Ok(submissions);
    }
}