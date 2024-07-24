using System.Security.Claims;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.models;
using informaticsge.Models;
using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly HttpClient _httpClient;
    private readonly UserManager<User> _userManager;

    public SubmissionController(AppDbContext appDbContext, HttpClient httpClient, UserService userService, UserManager<User> userManager)
    {
        _appDbContext = appDbContext;
        _httpClient = httpClient;
        _userManager = userManager;
    }
    

    [HttpPost("/submit")]
    [Authorize]
    public async Task<IActionResult> Submit(int problemId, [FromBody]string userCode)
    {
        try
        {
            var submissionRequest = await PrepareSubmissionRequest(problemId, userCode);
            var submissionResponse = await CallCompilationApi(submissionRequest); 
            
            await ProcessSubmissionResult(problemId, User, userCode, submissionResponse);
            
            return Ok(submissionResponse);
            
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (JsonSerializationException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }



    private async Task<SubmissionRequestDto> PrepareSubmissionRequest(int problemId, string userCode)
    {
        var problem = await _appDbContext.Problems.Include(pr => pr.TestCases).FirstOrDefaultAsync(problem => problem.Id == problemId);
    
        var testCaseDtos = problem.TestCases.Select(tc => new TestCaseDto
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();

        return new SubmissionRequestDto
        {
            Code = userCode,
            MemoryLimitMb = problem.MemoryLimit,
            TimeLimitMs = problem.RuntimeLimit,
            Testcases = testCaseDtos
        };
    }

    private async Task<List<SubmissionResultDto>> CallCompilationApi(SubmissionRequestDto submissionRequestDto)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:5144/compile", submissionRequestDto);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API call failed with status code: {response.StatusCode}");
            throw new HttpRequestException($"API call failed with status code: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var compilationResponse = JsonConvert.DeserializeObject<List<SubmissionResultDto>>(content);
        
        if (compilationResponse == null)
        {
            Console.WriteLine("Failed to deserialize API response");
            throw new JsonSerializationException("Failed to deserialize API response");
        }

        return compilationResponse;
    }


    private async Task ProcessSubmissionResult(int problemId,ClaimsPrincipal user , string userCode, List<SubmissionResultDto> results)
    {

        var problem = await _appDbContext.Problems.FirstOrDefaultAsync(pr => pr.Id == problemId);
        
        //checks if there is unsuccessful test and returns it. if there is no unsuccessful tests - returns first
        var checkForUnSuccessful = results.FirstOrDefault(r => r.Success == false) ?? results.FirstOrDefault();
        
        var submission = new Submissions
        {
            AuthUsername = user.Claims.First(u => u.Type == "UserName").Value,
            Code = userCode,
            ProblemId = problemId,
            ProblemName = problem.Name,
            Status = checkForUnSuccessful?.Status,
            Input = checkForUnSuccessful?.Input,
            ExpectedOutput = checkForUnSuccessful?.ExpectedOutput,
            Output = checkForUnSuccessful?.Output,
            UserId = user.Claims.First(u => u.Type == "Id").Value,
        };

        _appDbContext.Submissions.Add(submission);
        await _appDbContext.SaveChangesAsync();

    }
    
}