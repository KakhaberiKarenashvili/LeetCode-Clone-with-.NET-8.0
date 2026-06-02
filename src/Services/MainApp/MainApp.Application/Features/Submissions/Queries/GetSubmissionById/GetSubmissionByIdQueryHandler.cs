using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Submissions.Queries.GetSubmissionById;

public class GetSubmissionByIdQueryHandler : IRequestHandler<GetSubmissionByIdQuery, GetSubmissionResponse>
{
    private readonly AppDbContext _db;

    public GetSubmissionByIdQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetSubmissionResponse> Handle(GetSubmissionByIdQuery request, CancellationToken cancellationToken)
    {
        var submission = await _db.Submissions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (submission == null)
            throw new InvalidOperationException("Submission not found");

        return new GetSubmissionResponse(
            submission.Id,
            submission.AuthUsername,
            submission.Language,
            submission.Code,
            submission.ProblemId,
            submission.ProblemName,
            submission.Status,
            $"{submission.SuccessRate}%",
            submission.SubmissionTime,
            submission.Input,
            submission.ExpectedOutput,
            submission.Output);
    }
}
