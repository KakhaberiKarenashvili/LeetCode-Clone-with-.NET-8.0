using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Cms.Problems.GetProblems;

public record GetProblemsResponse(
    int Id,
    string? Name,
    List<Category>? Categories,
    Difficulty Difficulty);
