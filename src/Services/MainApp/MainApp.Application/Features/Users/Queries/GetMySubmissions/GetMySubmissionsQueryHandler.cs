using MainApp.Application.Extensions.Filtering;
using MainApp.Application.Extensions.Pagination;
using MainApp.Infrastructure.Data;
using MediatR;

namespace MainApp.Application.Features.Users.Queries.GetMySubmissions;

public class GetMySubmissionsQueryHandler : IRequestHandler<GetMySubmissionsQuery, PagedList<GetMySubmissionsResponse>>
{
    private readonly AppDbContext _db;

    public GetMySubmissionsQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedList<GetMySubmissionsResponse>> Handle(GetMySubmissionsQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Submissions
            .Where(s => s.UserId == request.UserId)
            .ApplyFilter(request.Status, request.Language)
            .AsQueryable();

        return await PagedList<GetMySubmissionsResponse>.CreateAsync(
            request.PageNumber,
            request.PageSize,
            data,
            s => new GetMySubmissionsResponse(
                s.Id,
                s.AuthUsername,
                s.ProblemName,
                s.Language,
                s.Status,
                $"{s.SuccessRate}%",
                s.SubmissionTime));
    }
}
