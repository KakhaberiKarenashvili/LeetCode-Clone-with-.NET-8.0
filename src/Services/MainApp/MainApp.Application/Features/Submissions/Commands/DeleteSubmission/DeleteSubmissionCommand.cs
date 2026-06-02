using MediatR;

namespace MainApp.Application.Features.Submissions.Commands.DeleteSubmission;

public record DeleteSubmissionCommand(int Id) : IRequest<Unit>;
