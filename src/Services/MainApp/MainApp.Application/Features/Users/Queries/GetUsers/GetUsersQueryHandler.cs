using MainApp.Application.Extensions.Pagination;
using MainApp.Infrastructure.Data;
using MediatR;

namespace MainApp.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedList<GetUserResponse>>
{
    private readonly AppDbContext _db;

    public GetUsersQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedList<GetUserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = (from user in _db.Users
            join userRole in _db.UserRoles on user.Id equals userRole.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join role in _db.Roles on ur.RoleId equals role.Id into roles
            from r in roles.DefaultIfEmpty()
            select new GetUserResponse(user.Id, user.UserName, user.Email, r.Name)).AsQueryable();

        return await PagedList<GetUserResponse>.CreateAsync(request.PageNumber, request.PageSize, query);
    }
}
