using MainApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly ProblemsService _problemsService;

    public ProblemsController(ProblemsService problemsService)
    {
        _problemsService = problemsService;
    }
    
    [HttpGet("/problems")]
    public async Task<IActionResult> GetProblems(int pageNumber)
    {
        var problems =  await _problemsService.GetAllProblems(pageNumber);

        return Ok(problems);
    }

    [HttpGet("/problems/{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        var problem = await _problemsService.GetProblem(id);
        
        return Ok(problem);
    }
    
    [HttpGet("/problems/{id}/submissions")]
    public async Task<IActionResult> GetSubmissions(int id)
    { 
        var submissions = await _problemsService.GetSubmissions(id);

        return Ok(submissions);
    }
}