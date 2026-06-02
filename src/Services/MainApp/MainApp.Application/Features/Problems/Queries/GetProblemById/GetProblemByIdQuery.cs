using MediatR;

namespace MainApp.Application.Features.Problems.Queries.GetProblemById;

public record GetProblemByIdQuery(int Id) : IRequest<GetProblemResponse>;
