using informaticsge.Dto;
using informaticsge.models;
using informaticsge.Models;
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
    public async Task<List<Problem>> GetProblems(int pagenumber)
    {
        return await _problemsService.GetAllProblems(pagenumber);
    }

    [HttpGet("/problems/{id}")]
    public async Task<Problem> GetProblem(int id)
    {
        return await _problemsService.GetProblem(id);
    }

    [HttpPost("/add-problem")]
    public async Task<string> AddProblem(AddProblemDto problem)
    {
        return await _problemsService.AddProblem(problem);
    }

    [HttpGet("/problems/{id}/submissions")]
    public async Task<List<GetSubmissionsDTO>> GetSubmissions(int id)
    {
        return await _problemsService.GetSubmissions(id);
    }
    
    

    
    
    
    
}