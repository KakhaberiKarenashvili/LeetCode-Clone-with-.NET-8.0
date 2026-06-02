using MainApp.Application.Extensions.Pagination;
using MediatR;

namespace MainApp.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedList<GetUserResponse>>;
