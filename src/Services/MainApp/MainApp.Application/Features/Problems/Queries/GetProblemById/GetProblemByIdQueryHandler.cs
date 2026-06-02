using BuildingBlocks.Common.Dtos;
using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Problems.Queries.GetProblemById;

public class GetProblemByIdQueryHandler : IRequestHandler<GetProblemByIdQuery, GetProblemResponse>
{
    private readonly AppDbContext _db;

    public GetProblemByIdQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetProblemResponse> Handle(GetProblemByIdQuery request, CancellationToken cancellationToken)
    {
        var problem = await _db.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (problem == null)
            throw new InvalidOperationException("Problem not found");

        var exampleTestCases = problem.TestCases?
            .Select(tc => new TestCaseDto { Input = tc.Input, ExpectedOutput = tc.ExpectedOutput })
            .Take(3)
            .ToList() ?? new List<TestCaseDto>();

        return new GetProblemResponse(
            problem.Id,
            problem.Name,
            problem.ProblemText,
            problem.Categories,
            problem.Difficulty,
            problem.RuntimeLimit,
            problem.MemoryLimit,
            exampleTestCases);
    }
}
