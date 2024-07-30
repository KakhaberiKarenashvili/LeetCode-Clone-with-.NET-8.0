using System.Diagnostics;
using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace Compilation_Service.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class Compiler : ControllerBase
{
    private readonly ILogger<Compiler> _logger;

    public Compiler(ILogger<Compiler> logger)
    {
        _logger = logger;
    }


    [HttpPost("/compile")]
    public async Task<List<SubmissionResponseDto>?> CompileAndRunCppCodeAsync(SubmissionRequestDto submissionRequestDto)
    {
        
        _logger.LogInformation(@"Compiler-API Received Submission Request For Code:\n {code}", submissionRequestDto.Code);
        
        List<SubmissionResponseDto> results = new List<SubmissionResponseDto>();

        var fileId = Guid.NewGuid();
        var cppFileName = $"cpp-file_{fileId}.cpp";
        var exeFileName = $"cpp-file_{fileId}";

        _logger.LogInformation("Provided Code Was Saved In {filename}", cppFileName);
        
        
        try
        {
            _logger.LogInformation("Trying To Compile {filename}", cppFileName);
            
            var compile = await CompileCppCode(submissionRequestDto.Code, cppFileName, exeFileName);

            if (compile.Success)
            {
                _logger.LogInformation("Successful Compilation for File: {filename} \n Running Tests....",cppFileName);
                
                var execute = await ExecuteCppCode(exeFileName, submissionRequestDto);
                
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

    private async Task<List<SubmissionResponseDto>> ExecuteCppCode(string? exeFileName, SubmissionRequestDto submissionRequestDto)
    {
        List<SubmissionResponseDto> results = new List<SubmissionResponseDto>();


        foreach (var testCase in submissionRequestDto.Testcases)
        {
            // Create a cancellation token source for monitoring memory usage and timeout
            CancellationTokenSource memoryCancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            // Execute the C++ code file using the command prompt
            var processInfo = new ProcessStartInfo
            {
                FileName = $"{exeFileName}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Environment =
                {
                    ["MEMORY_LIMIT_MB"] = submissionRequestDto.MemoryLimitMb.ToString(),
                    ["TIME_LIMIT_MS"] = submissionRequestDto.TimeLimitMs.ToString()
                }
            };

            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                // Pass the input string to the running process
                await process.StandardInput.WriteLineAsync(testCase.Input);
                process.StandardInput.Close();

                // Create tasks for monitoring memory usage and timeout
                Task monitorMemoryTask = MonitorMemoryUsage(process, submissionRequestDto.MemoryLimitMb, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(submissionRequestDto.TimeLimitMs, timeoutCancellationTokenSource.Token);


                // Wait for either the process to exit, memory limit exceeded, or timeout
                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    // If the memory limit is exceeded, terminate the process
                    process.Kill();
                    results.Add(new SubmissionResponseDto
                    {
                        Success = false,
                        Status = "Memory limit exceeded."
                    });

                    // Cancel the memory monitoring task
                    await memoryCancellationTokenSource.CancelAsync();
                }
                else if (completedTask == timeoutTask)
                {
                    // If the timeout is reached, terminate the process
                    process.Kill();
                    results.Add(new SubmissionResponseDto
                    {
                        Success = false,
                        Status = "Time limit exceeded."
                    });
                    // Cancel the memory monitoring task
                    await memoryCancellationTokenSource.CancelAsync();
                }

                //in case there is not problem with memory or timeout
                else
                {
                    var output = process.StandardOutput.ReadToEndAsync(timeoutCancellationTokenSource.Token).Result;
                    var error = process.StandardError.ReadToEndAsync(timeoutCancellationTokenSource.Token).Result;

                    if (string.IsNullOrEmpty(error))
                    {
                        if (output.Trim() == testCase.ExpectedOutput?.Trim())
                        {
                            results.Add(new SubmissionResponseDto
                            {
                                Success = true,
                                Input = testCase.Input,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output,
                                Status = "Successful"
                            });
                        }
                        else
                        {
                            results.Add(new SubmissionResponseDto
                            {
                                Success = false,
                                Input = testCase.Input,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output,
                                Status = "Output does not match expected output."
                            });
                        }
                    }
                    else
                    {
                        results.Add(new SubmissionResponseDto
                        {
                            Success = false,
                            Input = testCase.Input,
                            ExpectedOutput = testCase.ExpectedOutput,
                            Output = error,
                            Status = "Output does not match expected output."
                        });
                    }

                }
            }
        }

        return results;
    }

    private async Task<CompilationResultDto> CompileCppCode(string code, string cppFileName, string? exeFileName)
    {

        await System.IO.File.WriteAllTextAsync(cppFileName, code);

        var processInfo = new ProcessStartInfo
        {
            FileName = "g++",
            Arguments = $"{cppFileName} -o {exeFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (var process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            await process.WaitForExitAsync();

            var error = process.StandardError.ReadToEndAsync().Result;

            if (string.IsNullOrEmpty(error))
            {
                return new CompilationResultDto()
                {
                    Success = true,
                    Error = error,
                };
            }
            else
            {
                return new CompilationResultDto()
                {
                    Success = false,
                    Error = error,
                };
            }
        }
    }


    private async Task MonitorMemoryUsage(Process process, int memoryLimitMb, CancellationToken cancellationToken)
    {
        while (!process.HasExited && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (process.WorkingSet64 > memoryLimitMb * 1024 * 1024)
                {
                    process.Kill();
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error monitoring memory usage: {Message}",ex.Message);
            }

            await Task.Delay(100, cancellationToken);
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