using MainApp.Application.Common.Extensions.Filtering;
using MainApp.Application.Common.Extensions.Pagination;
using MainApp.Infrastructure.Data;
using MediatR;

namespace MainApp.Application.Features.Cms.Problems.GetProblems;

public class GetProblemsQueryHandler : IRequestHandler<GetProblemsQuery, PagedList<GetProblemsResponse>>
{
    private readonly AppDbContext _db;

    public GetProblemsQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedList<GetProblemsResponse>> Handle(GetProblemsQuery request, CancellationToken cancellationToken)
    {
        var data = _db.Problems
            .AsQueryable()
            .ApplyFilter(request.Name, request.Difficulty, request.Categories);

        return await PagedList<GetProblemsResponse>.CreateAsync(
            request.PageNumber,
            request.PageSize,
            data,
            p => new GetProblemsResponse(
                p.Id,
                p.Name,
                p.Categories,
                p.Difficulty));
    }
}
