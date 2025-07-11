using System.Diagnostics;
using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Messaging.Events;

namespace Compilation.Application.Services;

public class PythonTestingService
{
    private readonly MemoryMonitorService _memoryMonitorService;

    public PythonTestingService(MemoryMonitorService memoryMonitorService)
    {
        _memoryMonitorService = memoryMonitorService;
    }

    public async Task<List<TestResultDto>?> TestPythonCode(PythonSubmissionRequestedEvent submissionRequestedDto)
    {
        List<TestResultDto> results = new List<TestResultDto>();

        var fileId = Guid.NewGuid();
        var pythonFileName = $"python-file_{fileId}.py";
    
        try
        {
            await System.IO.File.WriteAllTextAsync(pythonFileName, submissionRequestedDto.Code);
            
            var execute = await ExecutePythonCode(pythonFileName, submissionRequestedDto);
            
            return execute;
        }
        finally
        {
            CleanupTemporaryFiles(pythonFileName); // Python doesn't produce an executable
        }
    }
    
     public async Task<List<TestResultDto>> ExecutePythonCode(string pythonFileName, PythonSubmissionRequestedEvent submissionRequested)
    {
        List<TestResultDto> results = new List<TestResultDto>();

        foreach (var testCase in submissionRequested.Testcases)
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
                    ["MEMORY_LIMIT_MB"] = submissionRequested.MemoryLimitMb.ToString(),
                    ["TIME_LIMIT_MS"] = submissionRequested.TimeLimitMs.ToString()
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
                Task monitorMemoryTask = _memoryMonitorService.MonitorMemoryUsage(process, submissionRequested.MemoryLimitMb, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(submissionRequested.TimeLimitMs, timeoutCancellationTokenSource.Token);

                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    process.Kill();
                    results.Add(new TestResultDto
                    {
                        Success = false,
                        Status = "Memory limit exceeded."
                    });

                    await memoryCancellationTokenSource.CancelAsync();
                }
                else if (completedTask == timeoutTask)
                {
                    process.Kill();
                    results.Add(new TestResultDto
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
                            results.Add(new TestResultDto
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
                            results.Add(new TestResultDto
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
                        results.Add(new TestResultDto
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