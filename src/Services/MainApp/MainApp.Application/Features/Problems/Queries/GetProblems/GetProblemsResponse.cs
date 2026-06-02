using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Problems.Queries.GetProblems;

public record GetProblemsResponse(
    int Id,
    string? Name,
    List<Category>? Categories,
    Difficulty Difficulty);
