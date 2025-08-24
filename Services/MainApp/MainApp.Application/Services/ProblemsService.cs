using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using BuildingBlocks.Common.Helpers;
using MainApp.Application.Dto.Request;
using MainApp.Application.Dto.Response;
using MainApp.Application.Pagination;
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

    public async Task<PagedList<GetProblemsResponseDto>> GetAllProblems(int pageNumber,int pageSize)
    {
        var data = _appDbContext.Problems
            .AsQueryable();
        
        var problemList = await PagedList<GetProblemsResponseDto>
            .CreateAsync(pageNumber,pageSize,data,GetProblemsResponseDto.FromProblem);
        
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
        var problem = AddProblemDto.ToProblem(problemDto);

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
        
        var parsedCategory = EnumParser.ParseCategoriesFromStrings(editProblemDto.Categories);
         
        var parsedDifficulty = EnumParser.ParseDifficultyFromString(editProblemDto.Difficulty);
        
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

    public async Task<PagedList<GetSubmissionsResponseDto>> GetSubmissions(int problemId,int pageNumber,int pageSize)
    {
        var checkProblemExists = await _appDbContext.Problems.AnyAsync(p => p.Id == problemId);
        
        if (checkProblemExists == false)
        {
            throw new InvalidOperationException("problem not found");
        }

        var data = _appDbContext.Submissions
            .Where(submissions => submissions.ProblemId == problemId)
            .AsQueryable();
        
        var submissions = await PagedList<GetSubmissionsResponseDto>
            .CreateAsync(pageNumber,pageSize,data,GetSubmissionsResponseDto.FromSubmission);;
        
        return submissions;
    }
    
}