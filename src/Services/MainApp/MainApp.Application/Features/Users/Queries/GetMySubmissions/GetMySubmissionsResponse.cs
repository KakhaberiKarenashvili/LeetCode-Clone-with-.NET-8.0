using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Users.Queries.GetMySubmissions;

public record GetMySubmissionsResponse(
    int Id,
    string? AuthUsername,
    string? ProblemName,
    Language Language,
    Status Status,
    string? SuccessRate,
    DateTime? SubmissionTime);
