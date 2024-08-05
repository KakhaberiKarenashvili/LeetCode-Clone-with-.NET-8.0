using informaticsge.Dto.Request;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace informaticsge.Services;

public class AdminService
{
    private readonly ILogger<AdminService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<User> _userManager;

    public AdminService(ILogger<AdminService> logger, AppDbContext appDbContext, UserManager<User> userManager)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _userManager = userManager;
    }

    public async Task<List<GetUsersRespoonse>> GetUsers()
    {
        _logger.LogInformation("Retrieving All Users From Database");
        
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
        
          _logger.LogInformation("Successfully Retrieved All Users From Database");
          
        return usersWithRoles;
    }
    
    public async Task<Problem> GetProblem(int problemId)
    {
        var problem = await _appDbContext.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == problemId);

        if (problem == null)
        {
            _logger.LogWarning("Problem with Id {problemId} not found", problemId);

            throw new InvalidOperationException("problem not found");
        }
        
        return problem;
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