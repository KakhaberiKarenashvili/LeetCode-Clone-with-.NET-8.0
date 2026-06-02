using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Problems.Commands.UpdateProblem;

public class UpdateProblemCommandHandler : IRequestHandler<UpdateProblemCommand, Unit>
{
    private readonly AppDbContext _db;

    public UpdateProblemCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(UpdateProblemCommand request, CancellationToken cancellationToken)
    {
        var problem = await _db.Problems.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (problem == null)
            throw new InvalidOperationException("Problem not found");

        problem.Name = request.Name;
        problem.ProblemText = request.ProblemText;
        problem.Categories = request.Categories;
        problem.Difficulty = request.Difficulty;
        problem.RuntimeLimit = request.RuntimeLimitMs;
        problem.MemoryLimit = request.MemoryLimitMb;
        problem.TestCases = request.TestCases?.Select(tc => new TestCase
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList() ?? new List<TestCase>();

        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
