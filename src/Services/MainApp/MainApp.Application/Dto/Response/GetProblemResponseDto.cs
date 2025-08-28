using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Helpers;
using MainApp.Domain.Entity;

namespace MainApp.Application.Dto.Response;

public class GetProblemResponseDto
{
    public int Id { set; get; }
    
    public string? Name { set; get; }
    
    public string? ProblemText { set; get; }
    
    public List<string>? Categories { set; get; }
    
    public string? Difficulty { set; get; }
    
    public int? TimelimitMs { set; get; }
    
    public int? MemoryLimitMb { set; get; }
    
    public List<TestCaseDto>? TestCases { set; get; }

    public static GetProblemResponseDto FromProblem(Problem problem, List<TestCaseDto> exampleTestCases)
    {
        return new GetProblemResponseDto
        {
            Id = problem.Id,
            Name = problem.Name,
            ProblemText = problem.ProblemText,
            Categories = EnumParser.ExtractCategoryFlags(problem.Category),
            Difficulty = problem.Difficulty.ToString(),
            TimelimitMs = problem.RuntimeLimit,
            MemoryLimitMb = problem.MemoryLimit,
            TestCases = exampleTestCases
        };
    }
}