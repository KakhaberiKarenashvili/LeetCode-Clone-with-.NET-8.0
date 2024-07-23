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
    public async Task<List<SubmissionResponceDTO>?> CompileAndRunCppCodeAsync(SubmissionRequestDTO submissionRequestDto)
    {
        List<SubmissionResponceDTO> results = new List<SubmissionResponceDTO>();
        
        var fileId = Guid.NewGuid();
        var cppFileName = $"temp_{fileId}.cpp";
        var exeFileName = $"temp_{fileId}";
        
        var compile = await CompileCppCode(submissionRequestDto.Code, cppFileName, exeFileName);

        if (compile.Success)
        {
            var execute = await ExecuteCppCode(exeFileName, submissionRequestDto);
            return execute;
        }
        else
        {
            results.Add(new SubmissionResponceDTO
            {
                Success = false,
                Input = submissionRequestDto.testcases.First().Input,
                ExpectedOutput = submissionRequestDto.testcases.First().ExpectedOutput,
                Output = compile.Error,
                Status = "Compilation Error"
            });
            
            return results;
        }
        
    }
    
    private async Task<List<SubmissionResponceDTO>> ExecuteCppCode(string? exefilepath, SubmissionRequestDTO submissionRequestDto)
    {
        List<SubmissionResponceDTO> results = new List<SubmissionResponceDTO>();
        
        
        foreach (var testCase in submissionRequestDto.testcases)
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
                    ["MEMORY_LIMIT_MB"] = submissionRequestDto.MemoryLimitMb.ToString(),
                    ["TIME_LIMIT_MS"] = submissionRequestDto.TimeLimitMs.ToString()
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
                Task monitorMemoryTask = MonitorMemoryUsage(process, submissionRequestDto.MemoryLimitMb, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(submissionRequestDto.TimeLimitMs, timeoutCancellationTokenSource.Token);

                
                // Wait for either the process to exit, memory limit exceeded, or timeout
                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    // If the memory limit is exceeded, terminate the process
                    process.Kill();
                    results.Add(new SubmissionResponceDTO
                    {
                        Success = false,
                        Status = "Memory limit exceeded."
                    });

                    // Cancel the memory monitoring task
                    memoryCancellationTokenSource.Cancel();
                }
                else if (completedTask == timeoutTask)
                {
                    // If the timeout is reached, terminate the process
                    process.Kill();
                    results.Add(new SubmissionResponceDTO
                    {
                        Success = false,
                        Status = "Time limit exceeded."
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
                        if (output.Trim() == testCase.ExpectedOutput?.Trim())
                        {
                            results.Add(new SubmissionResponceDTO
                            {
                                Success = true,
                                Input = testCase.Input,
                                ExpectedOutput = testCase.ExpectedOutput,
                                Output = output
                            });
                        }
                        else
                        {
                            results.Add(new SubmissionResponceDTO
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
                        results.Add(new SubmissionResponceDTO
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
    
    private async Task<CompilationResultDTO> CompileCppCode(string code, string cppFileName, string? exeFileName)
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

             await process.WaitForExitAsync();
            
            var error = process.StandardError.ReadToEndAsync().Result;

            if (string.IsNullOrEmpty(error))
            {
                return new CompilationResultDTO()
                {
                    Success = true,
                    Error = error,
                };
            }
            else
            {
                return new CompilationResultDTO()
                {
                    Success = false,
                    Error = error,
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


