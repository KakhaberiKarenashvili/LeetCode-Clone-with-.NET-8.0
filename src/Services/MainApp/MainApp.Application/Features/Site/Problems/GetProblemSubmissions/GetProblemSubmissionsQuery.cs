using BuildingBlocks.Common.Enums;
using MainApp.Application.Common.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Site.Problems.GetProblemSubmissions;

public record GetProblemSubmissionsQuery(
    int ProblemId = 0,
    int PageNumber = 1,
    int PageSize = 20,
    Status? Status = null,
    Language? Language = null) : IRequest<PagedList<GetProblemSubmissionsResponse>>;
