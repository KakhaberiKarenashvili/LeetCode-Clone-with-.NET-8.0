using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using BuildingBlocks.Messaging.Events;
using MainApp.Infrastructure.Data;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubmissionEntity = MainApp.Domain.Entity.Submissions;

namespace MainApp.Application.Features.Site.Submissions.CreateSubmission;

public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Unit>
{
    private readonly AppDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateSubmissionCommandHandler(AppDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Unit> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var problem = await _db.Problems
            .Include(p => p.TestCases)
            .FirstOrDefaultAsync(p => p.Id == request.ProblemId, cancellationToken);

        if (problem == null)
            throw new InvalidOperationException("Problem not found");

        var submission = new SubmissionEntity
        {
            AuthUsername = request.Username,
            Code = request.Code,
            ProblemId = request.ProblemId,
            Language = request.Language,
            ProblemName = problem.Name,
            Status = Status.TestRunning,
            UserId = request.UserId,
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync(cancellationToken);

        var testCaseDtos = problem.TestCases.Select(tc => new TestCaseDto
        {
            Input = tc.Input,
            ExpectedOutput = tc.ExpectedOutput
        }).ToList();

        if (request.Language == Language.Cpp)
        {
            await _publishEndpoint.Publish(new CppSubmissionRequestedEvent
            {
                SubmissionId = submission.Id,
                Code = request.Code,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                Testcases = testCaseDtos
            }, cancellationToken);
        }
        else
        {
            await _publishEndpoint.Publish(new PythonSubmissionRequestedEvent
            {
                SubmissionId = submission.Id,
                Code = request.Code,
                MemoryLimitMb = problem.MemoryLimit,
                TimeLimitMs = problem.RuntimeLimit,
                Testcases = testCaseDtos
            }, cancellationToken);
        }

        return Unit.Value;
    }
}
