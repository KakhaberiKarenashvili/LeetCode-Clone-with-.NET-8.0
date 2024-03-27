using informaticsge.Dto;
using informaticsge.models;
using informaticsge.Services;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProblemsControler : ControllerBase
{
    private readonly ProblemsService _problemsService;

    public ProblemsControler(ProblemsService problemsService)
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

    [HttpPost("/addproblem")]
    public async Task<string> AddProblem(AddProblemDTO problem)
    {
        return await _problemsService.AddProblem(problem);
    }

}