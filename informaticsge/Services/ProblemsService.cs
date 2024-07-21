using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using informaticsge.Models;

namespace informaticsge.Services;

public class ProblemsService
{
    private readonly AppDBContext _appDbContext;

    public ProblemsService(AppDBContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Problem>> GetAllProblems(int page)
    {
        //as there can be many problems i will enable pagination 

        var skip = (page - 1) * 50; //basically saying that 1 page can have 50 problems on each page method will skip 50x problems
       
        var data = await _appDbContext.Problems
            .Skip(skip)
            .Take(50)
            .ToListAsync();
        
        return data;
    }

    
    public async Task<Problem> GetProblem(int id)
    {
        var problem = await _appDbContext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == id) ?? new Problem();
        
        return problem;
    }

    public async Task<List<SubmissionsDTO>> GetSubmissions(int id)
    {
        
        var solutions = await _appDbContext.Submissions.Where(solution => solution.ProblemId == id).ToListAsync();

        return new List<SubmissionsDTO>();
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
        

        return "problem added sucessfully";
    }
    
}