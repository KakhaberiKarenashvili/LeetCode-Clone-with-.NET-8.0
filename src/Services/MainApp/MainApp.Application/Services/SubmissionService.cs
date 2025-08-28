using System.Security.Claims;
using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using BuildingBlocks.Messaging.Events;
using MainApp.Application.Dto.Response;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SubmissionDto = MainApp.Application.Dto.Request.SubmissionDto;


namespace MainApp.Application.Services;

public class SubmissionService
{
    private readonly AppDbContext _appDbContext;
    private readonly IPublishEndpoint _publishEndpoint;


    public SubmissionService(AppDbContext appDbContext, IPublishEndpoint publishEndpoint)
    {
        _appDbContext = appDbContext;
        _publishEndpoint = publishEndpoint;
    }


    public async Task<GetSubmissionResponseDto> GetSubmissionById(int id)
    {
        var submission = await _appDbContext.Submissions.FirstOrDefaultAsync(s => s.Id == id);
        
        if (submission == null)
        {
            throw new InvalidOperationException("Submission not found");
        }
        
        return GetSubmissionResponseDto.FromSubmission(submission);
    }
    

    public async Task HandleSubmission(SubmissionDto submission,ClaimsPrincipal user)
    {
        var submissionId = await SaveSubmission(submission, user);
        
        var submissionPayload = await PrepareSubmissionPayload(submission, submissionId);
        
        await _publishEndpoint.Publish(submissionPayload);
    }
    
    
    private async Task<int> SaveSubmission(SubmissionDto submissionDto,ClaimsPrincipal user)
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
                Status = Status.TestRunning,
                UserId = user.Claims.First(u => u.Type == "Id").Value,
            };
            try
            {
                _appDbContext.Submissions.Add(submission);
                await _appDbContext.SaveChangesAsync();

                return submission.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

    }


    private async Task<dynamic> PrepareSubmissionPayload(SubmissionDto submissionDto, int submissionId)
    {
        var problem = await _appDbContext.Problems.Include(pr => pr.TestCases)
            .FirstOrDefaultAsync(problem => problem.Id == submissionDto.ProblemId);

        if (problem == null) throw new InvalidOperationException("problem not found");

        var testCaseDtoList = problem.TestCases.Select(tc => new TestCaseDto
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();

        if (submissionDto.Language == "C++")
            return new CppSubmissionRequestedEvent
            {
                SubmissionId = submissionId,
                Code = submissionDto.Code,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                Testcases = testCaseDtoList
            };
        else
            return new PythonSubmissionRequestedEvent
            {
                SubmissionId = submissionId,
                Code = submissionDto.Code,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                Testcases = testCaseDtoList
            };
    }
    
}