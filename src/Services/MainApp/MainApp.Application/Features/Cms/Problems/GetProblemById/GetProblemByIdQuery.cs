using MediatR;

namespace MainApp.Application.Features.Cms.Problems.GetProblemById;

public record GetProblemByIdQuery(int Id) : IRequest<GetProblemResponse>;
