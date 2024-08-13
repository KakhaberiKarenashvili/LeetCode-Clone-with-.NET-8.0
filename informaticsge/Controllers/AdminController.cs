using informaticsge.Dto.Request;
using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]

[Authorize(AuthenticationSchemes = "Bearer")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AdminService adminService, ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [HttpGet("/get-users")]
    public async Task<IActionResult> GetUsers()
    {
       return Ok(await _adminService.GetUsers());
    }
    
   [HttpGet("/get-problem/{id}")]
    public async Task<IActionResult> GetProblem(int id)
    {
        _logger.LogInformation("Admin Requested Problem With Id: {id}",id);
        
        var problem = await _adminService.GetProblem(id);
        
        return Ok(problem);
    }
    
    [HttpPost("/add-problem")]
    public async Task<IActionResult> AddProblem(AddProblemDto problem)
    { 
        _logger.LogInformation("Admin Is Adding Problem");

        try
        {
            await _adminService.AddProblem(problem);
            
            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message,"Failed To Add Problem");
            
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
    }


    [HttpPut("/edit-problem/{id}")]
    public async Task<IActionResult> EditProblem(int id, AddProblemDto editProblem)
    {
        _logger.LogInformation("Admin Is Editing Problem");
        try
        {
            await _adminService.EditProblem(id, editProblem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message,"Failed To Edit Problem");
            
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        
        return Accepted();
    }

    [HttpDelete("/delete-problem/{id}")]
    public async Task<IActionResult> DeleteProblem(int id)
    {
        _logger.LogInformation("Admin Is Deleting Problem");

        try
        {
            await _adminService.DeleteProblem(id);
        }
        catch (Exception ex)
        {
           _logger.LogError(ex.Message,"Failed To Delete Problem");
            
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }

        return Ok();
    }
}