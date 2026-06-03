using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Cms.Problems.DeleteProblem;

public class DeleteProblemCommandHandler : IRequestHandler<DeleteProblemCommand, Unit>
{
    private readonly AppDbContext _db;

    public DeleteProblemCommandHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteProblemCommand request, CancellationToken cancellationToken)
    {
        var problem = await _db.Problems.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (problem == null)
            throw new InvalidOperationException("Problem not found");

        _db.Problems.Remove(problem);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
