using BuildingBlocks.Common.Enums;
using MediatR;

namespace MainApp.Application.Features.Submissions.Commands.CreateSubmission;

public record CreateSubmissionCommand(
    int ProblemId,
    Language Language,
    string Code,
    string UserId,
    string Username) : IRequest<Unit>;
