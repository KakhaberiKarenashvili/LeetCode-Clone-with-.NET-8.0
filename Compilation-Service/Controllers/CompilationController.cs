using System.Diagnostics;
using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;
using Compilation_Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Compilation_Service.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class CompilationController : ControllerBase
{
    private readonly ILogger<CompilationController> _logger;
    private readonly CppTestingService _cppTestingService;
    private readonly PythonTestingService _pythonTestingService;

    public CompilationController(ILogger<CompilationController> logger, CppTestingService cppTestingService, PythonTestingService pythonTestingService)
    {
        _logger = logger;
        _cppTestingService = cppTestingService;
        _pythonTestingService = pythonTestingService;
    }


    [HttpPost("/submit")]
    public async Task<List<SubmissionResponseDto>?> Submit(SubmissionRequestDto submissionRequestDto)
    {
        _logger.LogInformation(@" Received Submission Request For {language} Code:\n {code}", submissionRequestDto.Language, submissionRequestDto.Code);
        
        var result = new List<SubmissionResponseDto>();
        
        switch (submissionRequestDto.Language)
        {
            case "C++" :
                result = await _cppTestingService.TestCppCode(submissionRequestDto);
                break;
            case  "Python":
                result = await _pythonTestingService.TestPythonCode(submissionRequestDto);
                break;
        }
        
        return result;
    }


}