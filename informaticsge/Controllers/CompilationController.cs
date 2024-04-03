using informaticsge.entity;
using informaticsge.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompilationController : ControllerBase
{
    private readonly AppDBcontext _appDBcontext;
    private readonly HttpClient _httpClient;

    public CompilationController(AppDBcontext appDBcontext, HttpClient httpClient)
    {
        _appDBcontext = appDBcontext;
        _httpClient = httpClient;
    }
    
    [HttpPost("/compile")]
    public async Task<IActionResult> CheckCode(int problemId, string userCode)
    {

        var problem = await _appDBcontext.Problems.Include(pr => pr.TestCases).FirstOrDefaultAsync(problem => problem.Id == problemId);
        
        var testCaseDTOs = problem.TestCases.Select(tc => new TestCaseDTO
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();

        var compilationRequest = new CompilationRequestDTO
        {
            Code = userCode,
            MemoryLimitMS = problem.MemoryLimit,
            TimeLimitMS = problem.RuntimeLimit,
            testcases = testCaseDTOs
        };

        var response = await _httpClient.PostAsJsonAsync("http://localhost:8080/compile", compilationRequest);



        return Ok(response.Content);
    }
}