using BuildingBlocks.Common.Enums;
using MainApp.Application.Common.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Cms.Submissions.GetAllSubmissions;

public record GetAllSubmissionsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Status? Status = null,
    Language? Language = null) : IRequest<PagedList<GetSubmissionsResponse>>;
