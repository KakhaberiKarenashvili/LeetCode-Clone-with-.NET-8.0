using MediatR;

namespace MainApp.Application.Features.Site.Problems.GetProblemById;

public record GetProblemByIdQuery(int Id) : IRequest<GetProblemResponse>;
