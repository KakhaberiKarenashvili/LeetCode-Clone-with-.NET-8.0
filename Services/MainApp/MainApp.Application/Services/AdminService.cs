using MainApp.Application.Dto.Response;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace MainApp.Application.Services;

public class AdminService
{
    private readonly AppDbContext _appDbContext;

    public AdminService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<GetUsersResponseDto>> GetUsers()
    {
          var usersWithRoles = await (from user in _appDbContext.Users
            join userRole in _appDbContext.UserRoles on user.Id equals userRole.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join role in _appDbContext.Roles on ur.RoleId equals role.Id into roles
            from r in roles.DefaultIfEmpty()
            select new GetUsersResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = r.Name
            }).ToListAsync();
        
          
        return usersWithRoles;
    }
    
}