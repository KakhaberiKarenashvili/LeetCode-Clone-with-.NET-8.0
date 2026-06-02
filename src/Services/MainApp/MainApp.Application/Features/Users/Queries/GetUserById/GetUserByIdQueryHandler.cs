using MainApp.Application.Features.Users.Queries.GetUsers;
using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MainApp.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserResponse>
{
    private readonly AppDbContext _db;

    public GetUserByIdQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<GetUserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await (from user in _db.Users
            where user.Id == request.Id
            join userRole in _db.UserRoles on user.Id equals userRole.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join role in _db.Roles on ur.RoleId equals role.Id into roles
            from r in roles.DefaultIfEmpty()
            select new GetUserResponse(user.Id, user.UserName, user.Email, r.Name))
            .FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            throw new InvalidOperationException("User not found");

        return result;
    }
}
