using informaticsge.Dto.Request;
using informaticsge.RabbitMQ;
using informaticsge.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private readonly RabbitMqService _rabbitMqService;
    private readonly SubmissionService _submissionService;
    private readonly ILogger<SubmissionController> _logger;

    public SubmissionController(ILogger<SubmissionController> logger, RabbitMqService rabbitMqService, SubmissionService submissionService)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
        _submissionService = submissionService;
    }
    

    [HttpPost("/submit")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Submit(SubmissionDto submissionDto,[FromBody]string userCode) //for now i need my code plain text not json
    {
        var username = User.Claims.First(u => u.Type == "UserName").Value;
        
            _logger.LogInformation("Received submission request for problem Id: {ProblemId} from User: {Username}. Code: {UserCode}",
            submissionDto.ProblemId, username, userCode);

            try
            {
                var submissionId = await _submissionService.SaveSubmission(submissionDto, User, userCode);
                
                var submissionPayload = await _submissionService.PrepareSubmissionPayload(submissionDto, userCode, submissionId);
                
                _rabbitMqService.SendRequest(submissionPayload);
                
                return Accepted("Submission request received and being processed.");
            }
            catch(Exception ex)
            {
                _logger.LogError( "submission request for problem Id: {ProblemId} from User: {Username} failed. ",
                    submissionDto.ProblemId,username);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
    }



    
    
    

   
    
   
    
}