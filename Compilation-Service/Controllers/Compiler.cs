using System.Diagnostics;
using System.IO;
using System.Text;
using informaticsge.models;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class Compiler : ControllerBase
{
    [HttpPost("/compile")]
    public async Task<List<ExecutionResult>?> CompileAndRunCppCodeAsync(CompilationRequestDTO compilationRequestDto)
    {
        List<ExecutionResult> results = new List<ExecutionResult>();
        
        var fileid = Guid.NewGuid();
        var cppFileName = $"temp_{fileid}.cpp";
        var exeFileName = $"temp_{fileid}";
        
        var compile = await CompileCppCode(compilationRequestDto.Code, cppFileName, exeFileName);

        if (compile.Success)
        {
            var execute = await ExecuteCppCode(exeFileName, compilationRequestDto);
            return execute;
        }
        else
        {
            results.Add(new ExecutionResult
            {
                Success = false,
                Error = compile.Error
            });
            
            return results;
        }
        
    }
    
    private async Task<List<ExecutionResult>> ExecuteCppCode(string? exefilepath, CompilationRequestDTO compilationRequestDto)
    {
        List<ExecutionResult> results = new List<ExecutionResult>();
        
         var testCaseNum = 0;
        foreach (var testCase in compilationRequestDto.testcases)
        {
            // Create a cancellation token source for monitoring memory usage and timeout
            CancellationTokenSource memoryCancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            // Execute the C++ code file using the command prompt
            var processInfo = new ProcessStartInfo
            {
                FileName = $"{exefilepath}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Environment =
                {
                    ["MEMORY_LIMIT_MB"] = compilationRequestDto.MemoryLimitMb.ToString(),
                    ["TIME_LIMIT_MS"] = compilationRequestDto.TimeLimitMS.ToString()
                }
            };
           
            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();

                // Pass the input string to the running process
                process.StandardInput.WriteLine(testCase.Input);
                process.StandardInput.Close();

                // Create tasks for monitoring memory usage and timeout
                Task monitorMemoryTask = MonitorMemoryUsage(process, compilationRequestDto.MemoryLimitMb, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(compilationRequestDto.TimeLimitMS, timeoutCancellationTokenSource.Token);

                
                // Wait for either the process to exit, memory limit exceeded, or timeout
                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    // If the memory limit is exceeded, terminate the process
                    process.Kill();
                    results.Add(new ExecutionResult
                    {
                        TestCaseNum = testCaseNum,
                        Success = false,
                        Error = "Memory limit exceeded."
                    });

                    // Cancel the memory monitoring task
                    memoryCancellationTokenSource.Cancel();
                }
                else if (completedTask == timeoutTask)
                {
                    // If the timeout is reached, terminate the process
                    process.Kill();
                    results.Add(new ExecutionResult
                    {
                        TestCaseNum = testCaseNum,
                        Success = false,
                        Error = "Time limit exceeded."
                    }); 
                    // Cancel the memory monitoring task
                    memoryCancellationTokenSource.Cancel();
                }
                 
                //in case there is not problem with memory or timeout
                else
                {
                    var output = process.StandardOutput.ReadToEndAsync(timeoutCancellationTokenSource.Token).Result;
                    var error = process.StandardError.ReadToEndAsync(timeoutCancellationTokenSource.Token).Result;
                    
                    if (string.IsNullOrEmpty(error))
                    {
                        if (output.Trim() == testCase.ExpectedOutput.Trim())
                        {
                            results.Add(new ExecutionResult
                            {
                                TestCaseNum = testCaseNum,
                                Success = true,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output
                            });
                        }
                        else
                        {
                            results.Add(new ExecutionResult
                            {
                                TestCaseNum = testCaseNum,
                                Success = false,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output,
                                Error = "Output does not match expected output."
                            });
                        }
                    }
                    else
                    {
                        results.Add(new ExecutionResult
                        {
                            TestCaseNum = testCaseNum,
                            Success = false,
                            ExpectedOutput = testCase.ExpectedOutput,
                            Output = error,
                            Error = "Output does not match expected output."
                        });
                    }
                    
                }
            }
            testCaseNum++;
        }
        return results;
    }
    
    private async Task<CompilationResult> CompileCppCode(string code, string cppFileName, string? exeFileName)
    {
        
        await System.IO.File.WriteAllTextAsync(cppFileName, code);
        
        var processInfo = new ProcessStartInfo
        {
            FileName = "g++",
            Arguments =$"{cppFileName} -o {exeFileName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using (var process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();

            process.WaitForExitAsync();
            
            var error = process.StandardError.ReadToEndAsync().Result;

            if (string.IsNullOrEmpty(error))
            {
                return new CompilationResult()
                {
                    Success = true,
                    Error = error,
                    Executable = exeFileName
                };
            }
            else
            {
                return new CompilationResult()
                {
                    Success = false,
                    Error = error,
                    Executable = exeFileName
                };
            }
        }
    }
    
//santas little helper monitors memory use by cpp code 
    private async Task MonitorMemoryUsage(Process process, int memoryLimitMb, CancellationToken cancellationToken)
    {
        while (!process.HasExited && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("memory allocated " + process.PeakWorkingSet64);
                Console.WriteLine("memory limit " + (memoryLimitMb * 1024 * 1024));
                
                if (process.WorkingSet64 > memoryLimitMb * 1024 * 1024)
                {
                    process.Kill();
                    return;
                }
            }
            catch (Exception ex)
            {
                // Handle potential errors during memory usage retrieval (optional: log error)
                Console.WriteLine($"Error monitoring memory usage: {ex.Message}");
            }

            await Task.Delay(100, cancellationToken);
        }
    }
}


