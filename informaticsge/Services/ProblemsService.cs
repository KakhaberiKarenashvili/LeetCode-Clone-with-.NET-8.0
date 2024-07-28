using Microsoft.EntityFrameworkCore;
using informaticsge.Dto.Request;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Models;

namespace informaticsge.Services;

public class ProblemsService
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<ProblemsService> _logger;

    public ProblemsService(AppDbContext appDbContext, ILogger<ProblemsService> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
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
           _logger.LogWarning("Problem with Id {problemId} not found", problemId);

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
            _logger.LogWarning("Problem with Id {problemId} not found", problemId);

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
    
    public async Task AddProblem(AddProblemDto problemDto)
    {
        var problem = new Problem
        {
        
            Name = problemDto.Name,
            ProblemText = problemDto.ProblemText,
            Tag = problemDto.Tag,
            Difficulty = problemDto.Difficulty,
            RuntimeLimit = problemDto.RuntimeLimit,
            MemoryLimit = problemDto.MemoryLimit,
            TestCases = problemDto.TestCases.Select(tc => new TestCase
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList() 
        };

        try
        {
            await _appDbContext.Problems.AddAsync(problem);
            await _appDbContext.SaveChangesAsync();
            
            _logger.LogInformation("Problem Added Successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message,"Error While Adding Problem");
            throw;
        }
        
    }

    public async Task EditProblem(int id, AddProblemDto editProblemDto)
    {
        var problem = await _appDbContext.Problems.FirstOrDefaultAsync(p => p.Id == id);
        
        if (problem == null)
        {
            throw new InvalidOperationException("Problem not found");
        }
        
        problem.Name = editProblemDto.Name;
        problem.ProblemText = editProblemDto.ProblemText;
        problem.Tag = editProblemDto.Tag;
        problem.Difficulty = editProblemDto.Difficulty;
        problem.RuntimeLimit = editProblemDto.RuntimeLimit;
        problem.MemoryLimit = editProblemDto.MemoryLimit;
        problem.TestCases = editProblemDto.TestCases.Select(tc => new TestCase
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();


        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message,"Error While Editing Problem");
            throw;
        }
        
    }

    
}