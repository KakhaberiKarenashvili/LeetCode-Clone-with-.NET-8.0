using MediatR;

namespace MainApp.Application.Features.Problems.Commands.DeleteProblem;

public record DeleteProblemCommand(int Id) : IRequest<Unit>;
