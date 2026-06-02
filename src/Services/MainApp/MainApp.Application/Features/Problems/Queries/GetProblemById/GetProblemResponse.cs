using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Problems.Queries.GetProblemById;

public record GetProblemResponse(
    int Id,
    string? Name,
    string? ProblemText,
    List<Category>? Categories,
    Difficulty Difficulty,
    int? RuntimeLimitMs,
    int? MemoryLimitMb,
    List<TestCaseDto>? TestCases);
