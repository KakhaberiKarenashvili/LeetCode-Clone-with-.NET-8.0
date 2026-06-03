using BuildingBlocks.Common.Enums;
using MainApp.Application.Common.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Site.Profile.GetMySubmissions;

public record GetMySubmissionsQuery(
    string UserId = "",
    int PageNumber = 1,
    int PageSize = 20,
    Status? Status = null,
    Language? Language = null) : IRequest<PagedList<GetMySubmissionsResponse>>;
