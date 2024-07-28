using System.Security.Claims;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.models;
using informaticsge.Models;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ILogger<SubmissionController> _logger;

    public SubmissionController(AppDbContext appDbContext, HttpClient httpClient,  ILogger<SubmissionController> logger)
    {
        _appDbContext = appDbContext;
        _httpClient = httpClient;
        _logger = logger;
    }
    

    [HttpPost("/submit")]
    [Authorize]
    public async Task<IActionResult> Submit(int problemId, [FromBody]string userCode)
    {
        var username = User.Claims.First(u => u.Type == "UserName").Value;
        
            _logger.LogInformation("Received submission request for problem Id: {ProblemId} from User: {Username}. Code: {UserCode}",
            problemId, username, userCode);

            try
            {
                var submissionRequest = await PrepareSubmissionRequest(problemId, userCode);

                var submissionResponse = await CallCompilationApi(submissionRequest);

                await ProcessSubmissionResult(problemId, User, userCode, submissionResponse);

                return Ok(submissionResponse);
            }
            catch(Exception ex)
            {
                _logger.LogError( "submission request for problem Id: {ProblemId} from User: {Username} failed. ",problemId,username);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
    }



    private async Task<SubmissionRequestDto> PrepareSubmissionRequest(int problemId, string userCode)
    {
        _logger.LogInformation("preparing submission Request");

        try
        {

            var problem = await _appDbContext.Problems.Include(pr => pr.TestCases)
                .FirstOrDefaultAsync(problem => problem.Id == problemId);

            if (problem == null)
            {
                _logger.LogWarning("Problem with Id {id} not found", problemId);

                throw new InvalidOperationException("problem not found");
            }

            var testCaseDtoList = problem.TestCases.Select(tc => new TestCaseDto
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList();

            return new SubmissionRequestDto
            {
                Code = userCode,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                Testcases = testCaseDtoList
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing submission request for problem {id}.",problemId);
            throw;
        }

    }

    private async Task<List<SubmissionResultDto>> CallCompilationApi(SubmissionRequestDto submissionRequestDto)
    {
        _logger.LogInformation("calling compilation-API");
     
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5144/compile", submissionRequestDto);
            
            var content = await response.Content.ReadAsStringAsync();
            var compilationResponse = JsonConvert.DeserializeObject<List<SubmissionResultDto>>(content);
            
            if (compilationResponse == null)
            { 
                _logger.LogError("Failed to deserialize API response");
                
                throw new JsonSerializationException("Failed to deserialize API response");
            }
            return compilationResponse;
        }
        catch (Exception ex)
        {
            _logger.LogCritical( ex.Message);
            
            throw new HttpRequestException(" Failed To Connect Submission-API Try Again later");
        }
    }


    private async Task ProcessSubmissionResult(int problemId,ClaimsPrincipal user , string userCode, List<SubmissionResultDto> results)
    {
        _logger.LogInformation("starting processing submission result");

        try
        {
            var problem = await _appDbContext.Problems.FirstOrDefaultAsync(pr => pr.Id == problemId);
        
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
            
            _logger.LogInformation("Submission result successfully added in database");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing submission result.");
            throw;
        }

    }
    
}