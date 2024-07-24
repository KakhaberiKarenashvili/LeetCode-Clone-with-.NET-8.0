using Microsoft.EntityFrameworkCore;
using informaticsge.Dto.Request;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Models;

namespace informaticsge.Services;

public class ProblemsService
{
    private readonly AppDbContext _appDbContext;

    public ProblemsService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<GetProblemsResponseDto>> GetAllProblems(int page)
    {
        //as there can be many problems i will enable pagination 

        var skip = (page - 1) * 50; //basically saying that 1 page can have 50 problems on each page method will skip 50x problems
       
        var data = await _appDbContext.Problems
            .Skip(skip)
            .Take(50)
            .ToListAsync();

        var problemlist = data.Select(d => new GetProblemsResponseDto
        {
            Id = d.Id,
            Name = d.Name,
            Tag = d.Tag,
            Difficulty = d.Difficulty
        }).ToList();
        
        return problemlist;
    }

    
    public async Task<Problem> GetProblem(int id)
    {
        var problem = await _appDbContext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == id) ?? new Problem();
        
        return problem;
    }

    public async Task<List<GetSubmissionsResponseDto>> GetSubmissions(int id)
    {
        
        var submissionsList = await _appDbContext.Submissions.Where(submissions => submissions.ProblemId == id).ToListAsync();


        var getSubmissions = submissionsList.Select(submissions => new GetSubmissionsResponseDto
        {
            Id = submissions.Id,
            AuthUsername = submissions.AuthUsername,
            ProblemName = submissions.ProblemName,
            Status = submissions.Status
        }).ToList();
        
        return getSubmissions;
    }
    
    public async Task<string> AddProblem(AddProblemDto problemDto)
    {

        var problem = new Problem
        {
        
            Name = problemDto.Name,
            ProblemText = problemDto.Problem,
            Tag = problemDto.Tag,
            Difficulty = problemDto.Difficulty,
            RuntimeLimit = problemDto.RuntimeLimit,
            MemoryLimit = problemDto.MemoryLimit,
            TestCases = problemDto.TestCases?.Select(tc => new TestCase
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList() ?? new List<TestCase>()
        };
      

        await _appDbContext.Problems.AddAsync(problem);
        await _appDbContext.SaveChangesAsync();
        

        return "problem added successfully";
    }
    
}