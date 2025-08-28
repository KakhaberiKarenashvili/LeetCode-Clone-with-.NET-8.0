using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

[Route("api/admin")]
[ApiController]

[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 20)
    {
       return Ok(await _adminService.GetUsers(pageNumber, pageSize));
    }
    
}