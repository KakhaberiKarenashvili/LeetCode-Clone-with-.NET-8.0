using System.Security.Claims;
using informaticsge.Controllers;
using informaticsge.Dto.Request;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.models;
using informaticsge.Models;
using informaticsge.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

public class SubmissionService
{
    private readonly AppDbContext _appDbContext;
    private readonly RabbitMqService _rabbitMqService;
    private readonly ILogger<SubmissionController> _logger;


    public SubmissionService(RabbitMqService rabbitMqService, ILogger<SubmissionController> logger, AppDbContext appDbContext)
    {
        _rabbitMqService = rabbitMqService;
        _logger = logger;
        _appDbContext = appDbContext;
    }
    
    public async Task<int> SaveSubmission(SubmissionDto submissionDto,ClaimsPrincipal user, string userCode)
    {
        _logger.LogInformation("starting processing submission");
        try
        {
            var problem = await _appDbContext.Problems.FirstOrDefaultAsync(pr => pr.Id == submissionDto.ProblemId);
            
            if (problem == null)
            {
                _logger.LogWarning("Problem with Id {id} not found", submissionDto.ProblemId);

                throw new InvalidOperationException("problem not found");
            }
        
            var submission = new Submissions
            {
                AuthUsername = user.Claims.First(u => u.Type == "UserName").Value,
                Code = userCode,
                ProblemId = submissionDto.ProblemId,
                Language = submissionDto.Language,
                ProblemName = problem.Name,
                Status = "Testing",
                UserId = user.Claims.First(u => u.Type == "Id").Value,
            };
            
            _appDbContext.Submissions.Add(submission);
            await _appDbContext.SaveChangesAsync();

            _logger.LogInformation("Submission successfully added in database");
            
            return submission.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing submission.");
            throw;
        }

    }


    public async Task<SubmissionRequestDto> PrepareSubmissionPayload(SubmissionDto submissionDto, string userCode, int submissionId)
    {
        _logger.LogInformation("preparing submission Request");

        try
        {
            var problem = await _appDbContext.Problems.Include(pr => pr.TestCases)
                .FirstOrDefaultAsync(problem => problem.Id == submissionDto.ProblemId);

            if (problem == null)
            {
                _logger.LogWarning("Problem with Id {id} not found", submissionDto.ProblemId);

                throw new InvalidOperationException("problem not found");
            }

            var testCaseDtoList = problem.TestCases.Select(tc => new TestCaseDto
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList();

            return new SubmissionRequestDto
            {
                SubmissionId = submissionId,
                Language = submissionDto.Language,
                Code = userCode,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                TestCases = testCaseDtoList
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error preparing submission request for problem {id}.",submissionDto.ProblemId);
            throw;
        }

    }
    
    public void HandleSubmissionResults(SubmissionResponseDto submissionResponseDto)
    {   
        var submission =  _appDbContext.Submissions.FirstOrDefault(
            s => s.Id == submissionResponseDto.SubmissionId);

        if (submission == null)
        {
            _logger.LogWarning("Submission with Id {SubmissionId} not found", submissionResponseDto.SubmissionId);
            throw new InvalidOperationException("Submission not found");
        }
        
        var checkForUnSuccessful = submissionResponseDto.Results?.FirstOrDefault(r => r.Success == false) 
                                   ?? submissionResponseDto.Results?.FirstOrDefault();

        submission.Status = checkForUnSuccessful?.Status;
        submission.Output = checkForUnSuccessful?.Output; 
        submission.Input = checkForUnSuccessful?.Input;
        submission.ExpectedOutput = checkForUnSuccessful?.ExpectedOutput;

         _appDbContext.Update(submission);
        
        _logger.LogInformation("Submission with Id {SubmissionId} updated successfully", submissionResponseDto.SubmissionId);
    }

}