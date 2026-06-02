using MainApp.Application.Extensions.Filtering;
using MainApp.Application.Extensions.Pagination;
using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Problems.Queries.GetProblemSubmissions;

public class GetProblemSubmissionsQueryHandler : IRequestHandler<GetProblemSubmissionsQuery, PagedList<GetProblemSubmissionsResponse>>
{
    private readonly AppDbContext _db;

    public GetProblemSubmissionsQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedList<GetProblemSubmissionsResponse>> Handle(GetProblemSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var problemExists = await _db.Problems.AnyAsync(p => p.Id == request.ProblemId, cancellationToken);

        if (!problemExists)
            throw new InvalidOperationException("Problem not found");

        var data = _db.Submissions
            .Where(s => s.ProblemId == request.ProblemId)
            .ApplyFilter(request.Status, request.Language)
            .AsQueryable();

        return await PagedList<GetProblemSubmissionsResponse>.CreateAsync(
            request.PageNumber,
            request.PageSize,
            data,
            s => new GetProblemSubmissionsResponse(
                s.Id,
                s.AuthUsername,
                s.ProblemName,
                s.Language,
                s.Status,
                $"{s.SuccessRate}%",
                s.SubmissionTime));
    }
}
