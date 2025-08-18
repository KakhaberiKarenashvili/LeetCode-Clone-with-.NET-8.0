using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using MainApp.Application.Dto.Request;
using MainApp.Application.Dto.Response;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


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

        var problemList = data.
            Select(GetProblemsResponseDto.FromProblem)
            .ToList();
        
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
       
       var exampleTestCases = problem.TestCases?
           .Select(@case => new TestCaseDto
           {
               Input = @case.Input,
               ExpectedOutput = @case.ExpectedOutput
           })
           .Take(3)
           .ToList() ?? new List<TestCaseDto>();
       
       var problemResponse = GetProblemResponseDto.FromProblem(problem, exampleTestCases);;
        
        return problemResponse;
    }

    public async Task AddProblem(AddProblemDto problemDto)
    {
        var parsedCategory = ParseCategories(problemDto.Categories);

        var parsedDifficulty = ParseDifficulty(problemDto.Difficulty);


        var problem = new Problem
        {
            Name = problemDto.Name,
            ProblemText = problemDto.ProblemText,
            Category = parsedCategory,
            Difficulty = parsedDifficulty,
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
        }
        catch (Exception e)
        {
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
        
        var parsedCategory = ParseCategories(editProblemDto.Categories);
         
        var parsedDifficulty = ParseDifficulty(editProblemDto.Difficulty);
        
        problem.Name = editProblemDto.Name;
        problem.ProblemText = editProblemDto.ProblemText;
        problem.Category = parsedCategory;
        problem.Difficulty = parsedDifficulty;
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
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        
    }
    
    public async Task DeleteProblem(int id)
    { 
        var problem = await _appDbContext.Problems.FirstOrDefaultAsync(p => p.Id == id);
        
        if (problem == null)
        {
            throw new InvalidOperationException("Problem not found");
        }

        try
        {
            _appDbContext.Problems.Remove(problem);
            await _appDbContext.SaveChangesAsync();
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<GetSubmissionsResponseDto>> GetSubmissions(int problemId)
    {
        var checkProblemExists = await _appDbContext.Problems.AnyAsync(p => p.Id == problemId);
        
        if (checkProblemExists == false)
        {
            throw new InvalidOperationException("problem not found");
        }
        
        var submissionsList = await _appDbContext.Submissions.Where(submissions => submissions.ProblemId == problemId).ToListAsync();
        

        var getSubmissions = submissionsList
            .Select(GetSubmissionsResponseDto.FromSubmission)
            .ToList();
        
        return getSubmissions;
    }
    
    
    private static  Category ParseCategories(List<string> categoryStrings)
    {
        Category categories = Category.None;

        foreach (var categoryStr in categoryStrings)
        {
            if (Enum.TryParse<Category>(categoryStr, out var category))
            {
                categories |= category; // Add to the bitmask
            }
            else
            {
                throw new ArgumentException($"Invalid category: {categoryStr}");
            }
        }

        return categories;
    }
    
    private static Difficulty ParseDifficulty(string difficulty)
    {
        if (Enum.TryParse<Difficulty>(difficulty, out var parsedDifficulty))
        {
            return parsedDifficulty;
        }

        return Difficulty.Easy;
    }
}