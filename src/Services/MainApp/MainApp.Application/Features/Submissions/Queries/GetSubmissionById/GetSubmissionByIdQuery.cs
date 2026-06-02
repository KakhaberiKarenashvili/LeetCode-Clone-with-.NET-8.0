using MediatR;

namespace MainApp.Application.Features.Submissions.Queries.GetSubmissionById;

public record GetSubmissionByIdQuery(int Id) : IRequest<GetSubmissionResponse>;
