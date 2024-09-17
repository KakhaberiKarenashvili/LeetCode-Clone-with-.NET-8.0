using System.Diagnostics;
using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;

namespace Compilation_Service.Services;

public class PythonTestingService
{
    private readonly MemoryMonitorService _memoryMonitorService;
    private readonly ILogger<PythonTestingService> _logger;

    public PythonTestingService(MemoryMonitorService memoryMonitorService, ILogger<PythonTestingService> logger)
    {
        _memoryMonitorService = memoryMonitorService;
        _logger = logger;
    }

    public async Task<List<SubmissionResponseDto>?> TestPythonCode(SubmissionRequestDto submissionRequestDto)
    {
        _logger.LogInformation(@"CompilationController-API Received Submission Request For Python Code:\n {code}", submissionRequestDto.Code);
    
        List<SubmissionResponseDto> results = new List<SubmissionResponseDto>();

        var fileId = Guid.NewGuid();
        var pythonFileName = $"python-file_{fileId}.py";

        _logger.LogInformation("Provided Python Code Was Saved In {filename}", pythonFileName);
    
        try
        {
            await System.IO.File.WriteAllTextAsync(pythonFileName, submissionRequestDto.Code);
            
            var execute = await ExecutePythonCode(pythonFileName, submissionRequestDto);
        
            _logger.LogInformation("Finished Testing Python Code, Returning Results");
        
            return execute;
        }
        finally
        {
            _logger.LogInformation("Deleting Temporary Files...");
            CleanupTemporaryFiles(pythonFileName); // Python doesn't produce an executable
        }
    }
    
     public async Task<List<SubmissionResponseDto>> ExecutePythonCode(string pythonFileName, SubmissionRequestDto submissionRequestDto)
    {
        List<SubmissionResponseDto> results = new List<SubmissionResponseDto>();

        foreach (var testCase in submissionRequestDto.Testcases)
        {
            // Create a cancellation token source for monitoring memory usage and timeout
            CancellationTokenSource memoryCancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            var processInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"{pythonFileName}",
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
                Task monitorMemoryTask = _memoryMonitorService.MonitorMemoryUsage(process, submissionRequestDto.MemoryLimitMb, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(submissionRequestDto.TimeLimitMs, timeoutCancellationTokenSource.Token);

                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    process.Kill();
                    results.Add(new SubmissionResponseDto
                    {
                        Success = false,
                        Status = "Memory limit exceeded."
                    });

                    await memoryCancellationTokenSource.CancelAsync();
                }
                else if (completedTask == timeoutTask)
                {
                    process.Kill();
                    results.Add(new SubmissionResponseDto
                    {
                        Success = false,
                        Status = "Time limit exceeded."
                    });

                    await memoryCancellationTokenSource.CancelAsync();
                }
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
                            Status = "Error occurred during execution."
                        });
                    }
                }
            }
        }

        return results;
    }
     
     private void CleanupTemporaryFiles(string exeFileName)
     {
         if (System.IO.File.Exists(exeFileName))
         {
             System.IO.File.Delete(exeFileName);
         }
     }

}