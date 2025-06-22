using System.Security.Claims;
using MainApp.Domain.Models;
using MainApp.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SubmissionDto = MainApp.Application.Dto.Request.SubmissionDto;
using SubmissionRequestDto = MainApp.Application.Dto.Request.SubmissionRequestDto;
using SubmissionResponseDto = MainApp.Application.Dto.Response.SubmissionResponseDto;
using TestCaseDto = MainApp.Application.Dto.Request.TestCaseDto;

namespace MainApp.Application.Services;

public class SubmissionService
{
    private readonly AppDbContext _appDbContext;


    public SubmissionService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    public async Task<int> SaveSubmission(SubmissionDto submissionDto,ClaimsPrincipal user)
    {
        try
        {
            var problem = await _appDbContext.Problems.FirstOrDefaultAsync(pr => pr.Id == submissionDto.ProblemId);
            
            if (problem == null)
            {
                throw new InvalidOperationException("problem not found");
            }
        
            var submission = new Submissions
            {
                AuthUsername = user.Claims.First(u => u.Type == "UserName").Value,
                Code = submissionDto.Code,
                ProblemId = submissionDto.ProblemId,
                Language = submissionDto.Language,
                ProblemName = problem.Name,
                Status = "Testing",
                UserId = user.Claims.First(u => u.Type == "Id").Value,
            };
            
            _appDbContext.Submissions.Add(submission);
            await _appDbContext.SaveChangesAsync();
            
            return submission.Id;
        }
        catch (Exception ex)
        {
            throw;
        }

    }


    public async Task<SubmissionRequestDto> PrepareSubmissionPayload(SubmissionDto submissionDto, int submissionId)
    {
        try
        {
            var problem = await _appDbContext.Problems.Include(pr => pr.TestCases)
                .FirstOrDefaultAsync(problem => problem.Id == submissionDto.ProblemId);

            if (problem == null)
            {
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
                Code = submissionDto.Code,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                TestCases = testCaseDtoList
            };
        }
        catch (Exception ex)
        {
            throw;
        }

    }
    
    public async Task HandleSubmissionResults(SubmissionResponseDto submissionResponseDto)
    {   
        var submission =  _appDbContext.Submissions.FirstOrDefault(
            s => s.Id == submissionResponseDto.SubmissionId);

        if (submission == null)
        {
            throw new InvalidOperationException("Submission not found");
        }
        
        var checkForUnSuccessful = submissionResponseDto.Results?.FirstOrDefault(r => r.Success == false) 
                                   ?? submissionResponseDto.Results?.FirstOrDefault();

        submission.Status = checkForUnSuccessful?.Status;
        submission.Output = checkForUnSuccessful?.Output; 
        submission.Input = checkForUnSuccessful?.Input;
        submission.ExpectedOutput = checkForUnSuccessful?.ExpectedOutput;

        try
        {
            _appDbContext.Update(submission);

            await _appDbContext.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw;
        }
    }

}