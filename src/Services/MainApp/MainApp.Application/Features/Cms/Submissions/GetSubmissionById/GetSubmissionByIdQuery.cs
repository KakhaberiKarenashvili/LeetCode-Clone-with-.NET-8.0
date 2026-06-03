using MediatR;

namespace MainApp.Application.Features.Cms.Submissions.GetSubmissionById;

public record GetSubmissionByIdQuery(int Id) : IRequest<GetSubmissionResponse>;
