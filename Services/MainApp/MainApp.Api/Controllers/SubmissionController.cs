using MainApp.Application.Dto.Request;
using MainApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private readonly SubmissionService _submissionService;

    public SubmissionController( SubmissionService submissionService)
    {
        _submissionService = submissionService;
    }
    
    [HttpGet("/submissions/{id}")]
    public async Task<IActionResult> GetSubmissionById(int id)
    {
        var submission = await _submissionService.GetSubmissionById(id);
    
        if (submission == null)
            return NotFound($"Submission with ID {id} not found.");

        return Ok(submission);
    }
    

    [HttpPost("/submit")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Submit(SubmissionDto submissionDto) //for now i need my code plain text not json
    {
        var username = User.Claims.First(u => u.Type == "UserName").Value;
        
            try
            {
                await _submissionService.HandleSubmission(submissionDto, User);
                
                return Accepted("Submission request received and being processed.");
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
    }



    
    
    

   
    
   
    
}