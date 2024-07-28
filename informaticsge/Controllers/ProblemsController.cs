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

    [HttpPost("/add-problem")]
    public async Task<IActionResult> AddProblem(AddProblemDto problem)
    { 
        _logger.LogInformation(" Adding Problem Initiated");

        try
        {
            await _problemsService.AddProblem(problem);
            
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message,"Failed To Add Problem");
            
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }


    }


    [HttpPut("/edit-problem")]
    public async Task<IActionResult> EditProblem(int id, AddProblemDto editProblem)
    {
        _logger.LogInformation(" Editing Problem Initiated");
        try
        {
            await _problemsService.EditProblem(id, editProblem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message,"Failed To Edit Problem");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        
        return Accepted();
    }
    
    

    
    
    
    
}