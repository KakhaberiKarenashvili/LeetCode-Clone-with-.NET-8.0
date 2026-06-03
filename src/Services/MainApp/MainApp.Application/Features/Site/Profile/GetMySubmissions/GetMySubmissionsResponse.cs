using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Site.Profile.GetMySubmissions;

public record GetMySubmissionsResponse(
    int Id,
    string? AuthUsername,
    string? ProblemName,
    Language Language,
    Status Status,
    string? SuccessRate,
    DateTime? SubmissionTime);
