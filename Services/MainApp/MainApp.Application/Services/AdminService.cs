using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AddProblemDto = MainApp.Application.Dto.Request.AddProblemDto;
using GetUsersRespoonse = MainApp.Application.Dto.Response.GetUsersRespoonse;

namespace MainApp.Application.Services;

public class AdminService
{
    private readonly AppDbContext _appDbContext;

    public AdminService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<GetUsersRespoonse>> GetUsers()
    {
          var usersWithRoles = await (from user in _appDbContext.Users
            join userRole in _appDbContext.UserRoles on user.Id equals userRole.UserId into userRoles
            from ur in userRoles.DefaultIfEmpty()
            join role in _appDbContext.Roles on ur.RoleId equals role.Id into roles
            from r in roles.DefaultIfEmpty()
            select new GetUsersRespoonse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = r.Name
            }).ToListAsync();
        
          
        return usersWithRoles;
    }
    
}