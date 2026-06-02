using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Problems.Queries.GetProblemSubmissions;

public record GetProblemSubmissionsResponse(
    int Id,
    string? AuthUsername,
    string? ProblemName,
    Language Language,
    Status Status,
    string? SuccessRate,
    DateTime? SubmissionTime);
