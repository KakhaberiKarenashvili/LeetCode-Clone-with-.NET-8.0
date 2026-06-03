using MediatR;

namespace MainApp.Application.Features.Cms.Submissions.DeleteSubmission;

public record DeleteSubmissionCommand(int Id) : IRequest<Unit>;
