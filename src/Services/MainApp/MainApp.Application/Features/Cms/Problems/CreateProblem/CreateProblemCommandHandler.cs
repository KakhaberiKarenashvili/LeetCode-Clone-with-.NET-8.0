using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MediatR;

namespace MainApp.Application.Features.Cms.Problems.CreateProblem;

public class CreateProblemCommandHandler : IRequestHandler<CreateProblemCommand, Unit>
{
    private readonly AppDbContext _db;

    public CreateProblemCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CreateProblemCommand request, CancellationToken cancellationToken)
    {
        var problem = new Problem
        {
            Name = request.Name,
            ProblemText = request.ProblemText,
            Categories = request.Categories,
            Difficulty = request.Difficulty,
            RuntimeLimit = request.RuntimeLimitMs,
            MemoryLimit = request.MemoryLimitMb,
            TestCases = request.TestCases?.Select(tc => new TestCase
            {
                Input = tc.Input,
                ExpectedOutput = tc.ExpectedOutput
            }).ToList() ?? new List<TestCase>()
        };

        await _db.Problems.AddAsync(problem, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
