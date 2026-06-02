using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using MediatR;

namespace MainApp.Application.Features.Problems.Commands.UpdateProblem;

public record UpdateProblemCommand(
    int Id,
    string Name,
    string ProblemText,
    List<Category> Categories,
    Difficulty Difficulty,
    int RuntimeLimitMs,
    int MemoryLimitMb,
    ICollection<TestCaseDto>? TestCases) : IRequest<Unit>;
