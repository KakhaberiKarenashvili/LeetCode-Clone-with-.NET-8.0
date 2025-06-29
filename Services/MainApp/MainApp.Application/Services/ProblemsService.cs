using MainApp.Domain.Models;
using MainApp.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GetProblemResponseDto = MainApp.Application.Dto.Response.GetProblemResponseDto;
using GetProblemsResponseDto = MainApp.Application.Dto.Response.GetProblemsResponseDto;
using GetSubmissionsResponseDto = MainApp.Application.Dto.Response.GetSubmissionsResponseDto;

namespace MainApp.Application.Services;

public class ProblemsService
{
    private readonly AppDbContext _appDbContext;

    public ProblemsService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<GetProblemsResponseDto>> GetAllProblems(int page)
    {
        if (page is < 1 or > int.MaxValue - 1) page = 1;
        
        var skip = (page - 1) * 50; 
       
        var data = await _appDbContext.Problems
            .Skip(skip)
            .Take(50)
            .ToListAsync();

        var problemList = data.Select(d => new GetProblemsResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            Tag = d.Tag,
            Difficulty = d.Difficulty
        }).ToList();
        
        return problemList;
    }

    
    public async Task<GetProblemResponseDto> GetProblem(int problemId)
    {
       var problem = await _appDbContext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == problemId);

       if (problem == null)
       {
           throw new InvalidOperationException("problem not found");
       }
       
       var exampleTestCase = problem.TestCases?.First() ?? new TestCase();
       
       var problemResponse = new GetProblemResponseDto
       {
           Id = problem.Id,
           Name = problem.Name,
           ProblemText = problem.ProblemText,
           Tag = problem.Tag,
           Difficulty = problem.Difficulty,
           TimelimitMs = problem.RuntimeLimit,
           MemoryLimitMb = problem.MemoryLimit,
           ExampleInput = exampleTestCase.Input,
           ExampleOutput = exampleTestCase.ExpectedOutput
       };
        
        return problemResponse;
    }

    public async Task<List<GetSubmissionsResponseDto>> GetSubmissions(int problemId)
    {
        var checkProblemExists = await _appDbContext.Problems.AnyAsync(p => p.Id == problemId);
        
        if (checkProblemExists == false)
        {
            throw new InvalidOperationException("problem not found");
        }
        
        var submissionsList = await _appDbContext.Submissions.Where(submissions => submissions.ProblemId == problemId).ToListAsync();
        

        var getSubmissions = submissionsList.Select(submissions => new GetSubmissionsResponseDto
        {
            Id = submissions.Id,
            AuthUsername = submissions.AuthUsername,
            ProblemName = submissions.ProblemName,
            Status = submissions.Status
        }).ToList();
        
        return getSubmissions;
    }
    
   

    
}