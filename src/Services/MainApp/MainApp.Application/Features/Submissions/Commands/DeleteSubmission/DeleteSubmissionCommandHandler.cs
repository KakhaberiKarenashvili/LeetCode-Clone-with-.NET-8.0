using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Submissions.Commands.DeleteSubmission;

public class DeleteSubmissionCommandHandler : IRequestHandler<DeleteSubmissionCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteSubmissionCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = await _db.Submissions.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (submission == null)
            throw new InvalidOperationException("Submission not found");

        _db.Submissions.Remove(submission);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
