using MainApp.Application.Common.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Cms.Users.GetUsers;

public record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<GetUserResponse>>;
