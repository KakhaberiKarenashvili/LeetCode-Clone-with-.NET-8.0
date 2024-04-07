using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

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
    public async Task<IActionResult> CheckCode(int problemId, [FromBody]string userCode)
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
        

        var response = await _httpClient.PostAsJsonAsync("http://localhost:5144/compile", compilationRequest);

        if (!response.IsSuccessStatusCode)
        {
            // Handle API call failure
            Console.WriteLine($"API call failed with status code: {response.StatusCode}");
            return StatusCode((int)response.StatusCode);
        }
        
        var content = await response.Content.ReadAsStringAsync(); //stupidest thing in .net receiving JSON and have to read as string
        
        var compilationResponse = JsonConvert.DeserializeObject<List<CompilationResultDTO>>(content);
        
        if (compilationResponse == null)
        {
            // Handle deserialization failure
            Console.WriteLine("Failed to deserialize API response");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        

        return Ok(compilationResponse);
    }
}