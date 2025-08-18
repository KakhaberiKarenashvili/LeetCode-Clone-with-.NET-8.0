using BuildingBlocks.Common.Helpers;
using MainApp.Domain.Entity;

namespace MainApp.Application.Dto.Response;

public class GetProblemsResponseDto
{
    public int Id { set; get;  }
    
    public string? Name { set; get; }
    
    public List<string>? Categories { set; get; }
    
    public string? Difficulty { set; get; }

    public static GetProblemsResponseDto FromProblem(Problem problem)
    {
        return new GetProblemsResponseDto
        {
            Id = problem.Id,
            Name = problem.Name,
            Categories = EnumParser.ParseCategories(problem.Category),
            Difficulty = problem.Difficulty.ToString(),
        };
    }
    
}