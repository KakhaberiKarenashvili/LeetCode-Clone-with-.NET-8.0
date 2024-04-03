using informaticsge.Dto;
using informaticsge.entity;
using informaticsge.models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using informaticsge.Models;

namespace informaticsge.Services;

public class ProblemsService
{
    private readonly AppDBcontext _appDBcontext;

    public ProblemsService(AppDBcontext appDBcontext)
    {
        _appDBcontext = appDBcontext;
    }

    public async Task<List<Problem>> GetAllProblems(int page)
    {
        //as there can be many problems i will enable pagination 

        var skip = (page - 1) * 50; //basically saying that 1 page can have 50 problems on each page method will skip 50x problems
       
        var data = await _appDBcontext.Problems
            .Skip(skip)
            .Take(50)
            .ToListAsync();
        
        return data;
    }

    
    public async Task<Problem> GetProblem(int id)
    {
        var problem = await _appDBcontext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == id) ?? new Problem();
        
        return problem;
    }

    public async Task<List<SolutionsDTO>> GetSolutions(int id)
    {
        
        var solutions = await _appDBcontext.Solutions.Where(Solution => Solution.Problem_id == id).ToListAsync();

        return new List<SolutionsDTO>();
    }
    public async Task<string> AddProblem(AddProblemDTO problemDto)
    {

        var problem = new Problem
        {
        
            Name = problemDto.Name,
            problem = problemDto.Problem,
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
      

        await _appDBcontext.Problems.AddAsync(problem);
        await _appDBcontext.SaveChangesAsync();
        

        return "problem added sucessfully";
    }
    
}