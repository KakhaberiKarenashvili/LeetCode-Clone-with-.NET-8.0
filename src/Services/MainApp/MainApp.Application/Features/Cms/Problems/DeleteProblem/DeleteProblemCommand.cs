using MediatR;

namespace MainApp.Application.Features.Cms.Problems.DeleteProblem;

public record DeleteProblemCommand(int Id) : IRequest<Unit>;
