using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Cms.Submissions.GetAllSubmissions;

public record GetSubmissionsResponse(
    int Id,
    string? AuthUsername,
    string? ProblemName,
    Language Language,
    Status Status,
    string? SuccessRate,
    DateTime? SubmissionTime);
