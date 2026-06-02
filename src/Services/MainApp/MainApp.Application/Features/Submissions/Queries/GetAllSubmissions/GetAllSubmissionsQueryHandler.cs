using MainApp.Application.Extensions.Filtering;
using MainApp.Application.Extensions.Pagination;
using MainApp.Infrastructure.Data;
using MediatR;

namespace MainApp.Application.Features.Submissions.Queries.GetAllSubmissions;

public class GetAllSubmissionsQueryHandler : IRequestHandler<GetAllSubmissionsQuery, PagedList<GetSubmissionsResponse>>
{
    private readonly AppDbContext _db;

    public GetAllSubmissionsQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedList<GetSubmissionsResponse>> Handle(GetAllSubmissionsQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Submissions
            .ApplyFilter(request.Status, request.Language)
            .AsQueryable();

        return await PagedList<GetSubmissionsResponse>.CreateAsync(
            request.PageNumber,
            request.PageSize,
            data,
            s => new GetSubmissionsResponse(
                s.Id,
                s.AuthUsername,
                s.ProblemName,
                s.Language,
                s.Status,
                $"{s.SuccessRate}%",
                s.SubmissionTime));
    }
}
