using System.Diagnostics;
using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using BuildingBlocks.Messaging.Events;
using Compilation.Application.Dto.Common;

namespace Compilation.Application.Services;

public class CppTestingService
{
    private readonly MemoryMonitorService _memoryMonitorService;
    private readonly ILogger _logger;

    public CppTestingService(MemoryMonitorService memoryMonitorService, ILogger<CppTestingService> logger)
    {
        _memoryMonitorService = memoryMonitorService;
        _logger = logger;
    }


    public async Task<List<TestResultDto>?> TestCppCode(CppSubmissionRequestedEvent submissionRequestedDto)
    {
        List<TestResultDto> results = new List<TestResultDto>();

        var fileId = Guid.NewGuid().ToString("N")[..6]; 
        var cppFileName = $"cpp-file_{fileId}.cpp";
        var exeFileName = $"cpp-file_{fileId}";
        
        try
        {
            var compile = await CompileCppCode(submissionRequestedDto.Code, cppFileName, exeFileName);

            if (compile.Success)
            {
                var execute = await ExecuteCppCode(exeFileName, submissionRequestedDto);
               
                return execute;
            }
            else
            {
                results.Add(new TestResultDto
                {
                    
                    Success = false,
                    Input = submissionRequestedDto.Testcases.First().Input ?? new TestCaseDto().Input,
                    ExpectedOutput = submissionRequestedDto.Testcases.First().ExpectedOutput ?? new TestCaseDto().ExpectedOutput,
                    Output = compile.Error,
                    Status = Status.CompilationFailed
                });
                return results;
            }
        }
        finally
        {
            CleanupTemporaryFiles(cppFileName, exeFileName);
        }
    }


    private async Task<CompilationResult> CompileCppCode(string code, string cppFileName, string? exeFileName)
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
                return new CompilationResult()
                {
                    Success = true,
                    Error = error,
                };
            }
            else
            {
                return new CompilationResult()
                {
                    Success = false,
                    Error = error,
                };
            }
        }
    }
    
      private async Task<List<TestResultDto>> ExecuteCppCode(string? exeFileName, CppSubmissionRequestedEvent submissionRequested)
    {
        List<TestResultDto> results = new List<TestResultDto>();


        foreach (var testCase in submissionRequested.Testcases)
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


                // Wait for either the process to exit, memory limit exceeded, or timeout
                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    // If the memory limit is exceeded, terminate the process
                    process.Kill();
                    results.Add(new TestResultDto
                    {
                        Success = false,
                        Status = Status.MemoryLimitExceeded
                    });

                    // Cancel the memory monitoring task
                    await memoryCancellationTokenSource.CancelAsync();
                }
                else if (completedTask == timeoutTask)
                {
                    // If the timeout is reached, terminate the process
                    process.Kill();
                    results.Add(new TestResultDto
                    {
                        Success = false,
                        Status = Status.TimeLimitExceeded
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
                            results.Add(new TestResultDto
                            {
                                Success = true,
                                Input = testCase.Input,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output,
                                Status = Status.TestPassed
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
                                Status = Status.TestFailed
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
                            Status = Status.TestFailed
                        });
                    }

                }
            }
        }

        return results;
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