using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AddProblemDto = MainApp.Application.Dto.Request.AddProblemDto;
using GetUsersRespoonse = MainApp.Application.Dto.Response.GetUsersRespoonse;

namespace MainApp.Application.Services;

public class AdminService
{
    private readonly AppDbContext _appDbContext;

    public AdminService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<GetUsersRespoonse>> GetUsers()
    {
          var usersWithRoles = await (from user in _appDbContext.Users
            join userRole in _appDbContext.UserRoles on user.Id equals userRole.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join role in _appDbContext.Roles on ur.RoleId equals role.Id into roles
            from r in roles.DefaultIfEmpty()
            select new GetUsersRespoonse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = r.Name
            }).ToListAsync();
        
          
        return usersWithRoles;
    }
    
    public async Task<Problem> GetProblem(int problemId)
    {
        var problem = await _appDbContext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == problemId);

        if (problem == null)
        {

            throw new InvalidOperationException("problem not found");
        }
        
        return problem;
    }
    
     public async Task AddProblem(AddProblemDto problemDto)
     {

         var parsedCategory = ParseCategories(problemDto.Categories);
         
         var parsedDifficulty = ParseDifficulty(problemDto.Difficulty);
         
        
        var problem = new Problem
        {
        
            Name = problemDto.Name,
            ProblemText = problemDto.ProblemText,
            Category =  parsedCategory,
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