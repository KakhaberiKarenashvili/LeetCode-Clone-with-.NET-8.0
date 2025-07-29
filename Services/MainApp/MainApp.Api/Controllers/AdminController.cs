using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

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

    [HttpGet("/get-users")]
    public async Task<IActionResult> GetUsers()
    {
       return Ok(await _adminService.GetUsers());
    }
    
   [HttpGet("/get-problem/{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        var problem = await _adminService.GetProblem(id);
        
        return Ok(problem);
    }
    
    [HttpPost("/add-problem")]
    public async Task<IActionResult> AddProblem(AddProblemDto problem)
    { 
        try
        {
            await _adminService.AddProblem(problem);
            
            return Created();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }


    [HttpPut("/edit-problem/{id}")]
    public async Task<IActionResult> EditProblem(int id, AddProblemDto editProblem)
    {
        try
        {
            await _adminService.EditProblem(id, editProblem);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        return Accepted();
    }

    [HttpDelete("/delete-problem/{id}")]
    public async Task<IActionResult> DeleteProblem(int id)
    {
        try
        {
            await _adminService.DeleteProblem(id);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }
}