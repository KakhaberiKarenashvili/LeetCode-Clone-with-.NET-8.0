using BuildingBlocks.Common.Enums;
using MainApp.Application.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Submissions.Queries.GetAllSubmissions;

public record GetAllSubmissionsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Status? Status = null,
    Language? Language = null) : IRequest<PagedList<GetSubmissionsResponse>>;
