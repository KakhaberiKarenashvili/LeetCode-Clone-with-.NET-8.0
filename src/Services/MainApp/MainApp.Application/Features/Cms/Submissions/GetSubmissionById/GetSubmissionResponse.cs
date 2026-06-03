using BuildingBlocks.Common.Enums;

namespace MainApp.Application.Features.Cms.Submissions.GetSubmissionById;

public record GetSubmissionResponse(
    int Id,
    string? AuthUsername,
    Language Language,
    string? Code,
    int ProblemId,
    string? ProblemName,
    Status Status,
    string? SuccessRate,
    DateTime SubmissionTime,
    string? Input,
    string? ExpectedOutput,
    string? Output);
