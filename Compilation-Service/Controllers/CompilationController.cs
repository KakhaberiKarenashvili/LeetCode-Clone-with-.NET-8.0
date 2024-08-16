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

    public CompilationController(ILogger<CompilationController> logger, CppTestingService cppTestingService)
    {
        _logger = logger;
        _cppTestingService = cppTestingService;
    }


    [HttpPost("/compile")]
    public async Task<List<SubmissionResponseDto>?> CompileAndRunCppCodeAsync(SubmissionRequestDto submissionRequestDto)
    {
        
        _logger.LogInformation(@"CompilationController-API Received Submission Request For Code:\n {code}", submissionRequestDto.Code);
        
        List<SubmissionResponseDto> results = new List<SubmissionResponseDto>();

        var fileId = Guid.NewGuid();
        var cppFileName = $"cpp-file_{fileId}.cpp";
        var exeFileName = $"cpp-file_{fileId}";

        _logger.LogInformation("Provided Code Was Saved In {filename}", cppFileName);
        
        try
        {
            _logger.LogInformation("Trying To Compile {filename}", cppFileName);
            
            var compile = await _cppTestingService.CompileCppCode(submissionRequestDto.Code, cppFileName, exeFileName);

            if (compile.Success)
            {
                _logger.LogInformation("Successful Compilation for File: {filename} \n Running Tests....",cppFileName);
                
                var execute = await _cppTestingService.ExecuteCppCode(exeFileName, submissionRequestDto);
                
                _logger.LogInformation("Finished Testing Returning Results");
                
                return execute;
            }
            else
            {
                _logger.LogInformation("Unsuccessful Compilation for File: {filename}",cppFileName);
                
                results.Add(new SubmissionResponseDto
                {
                    Success = false,
                    Input = submissionRequestDto.Testcases.First().Input ?? new TestCaseDto().Input,
                    ExpectedOutput = submissionRequestDto.Testcases.First().ExpectedOutput ?? new TestCaseDto().ExpectedOutput,
                    Output = compile.Error,
                    Status = "Compilation Error"
                });
                return results;
            }
        }
        finally
        {
            _logger.LogInformation("Deleting Temporary Files...");
            CleanupTemporaryFiles(cppFileName, exeFileName);
        }
    }

    
    
    private void CleanupTemporaryFiles(string cppFileName, string exeFileName)
    {
        if (System.IO.File.Exists(cppFileName))
        {
            System.IO.File.Delete(cppFileName);
        }

        if (System.IO.File.Exists(exeFileName))
        {
            System.IO.File.Delete(exeFileName);
        }
    }

}