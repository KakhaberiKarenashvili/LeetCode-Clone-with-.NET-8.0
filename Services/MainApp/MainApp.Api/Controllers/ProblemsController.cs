using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

[Route("api/problems")]
[ApiController]
public class ProblemsController : ControllerBase
{
    private readonly ProblemsService _problemsService;

    public ProblemsController(ProblemsService problemsService)
    {
        _problemsService = problemsService;
    }
    
    [HttpGet()]
    public async Task<IActionResult> GetProblems([FromQuery]string? name,
        [FromQuery]string? difficulty,
        [FromQuery]List<string>? categories, 
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 20)
    {
        var problems =  await _problemsService
            .GetAllProblems(pageNumber,pageSize,name,difficulty,categories);

        return Ok(problems);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        var problem = await _problemsService.GetProblem(id);
        
        return Ok(problem);
    }
    
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    [HttpPost()]
    public async Task<IActionResult> AddProblem(AddProblemDto problem)
    { 
        try
        {
            await _problemsService.AddProblem(problem);
            
            return Created();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> EditProblem(int id, AddProblemDto editProblem)
    {
        try
        {
            await _problemsService.EditProblem(id, editProblem);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        return Accepted();
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProblem(int id)
    {
        try
        {
            await _problemsService.DeleteProblem(id);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }
    
    [HttpGet("{id}/submissions")]
    public async Task<IActionResult> GetSubmissions(int id, string? language, 
        string? status ,
        [FromQuery]int pageNumber = 1,
        [FromQuery]int pageSize = 20)
    { 
        var submissions = await _problemsService.GetSubmissions(id, pageNumber, pageSize, status, language);

        return Ok(submissions);
    }
}