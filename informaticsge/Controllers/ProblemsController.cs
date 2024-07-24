using informaticsge.Dto.Request;
using informaticsge.Dto.Response;
using informaticsge.Services;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

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
    public async Task<IActionResult> GetProblems(int pagenumber)
    {
        var problems =  await _problemsService.GetAllProblems(pagenumber);

        return Ok(problems);
    }

    [HttpGet("/problems/{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        var problem = await _problemsService.GetProblem(id);
        
        return Ok(problem);
    }

    [HttpPost("/add-problem")]
    public async Task<string> AddProblem(AddProblemDto problem)
    {
        return await _problemsService.AddProblem(problem);
    }

    [HttpGet("/problems/{id}/submissions")]
    public async Task<List<GetSubmissionsResponseDto>> GetSubmissions(int id)
    {
        return await _problemsService.GetSubmissions(id);
    }
    
    

    
    
    
    
}