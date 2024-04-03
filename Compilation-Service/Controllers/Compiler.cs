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

    [HttpGet("/hello")]
    public string hello()
    {
        Console.WriteLine("hello from docker");
        return "hello from docker";
    }

    [HttpPost("/compile")]
    public async Task<List<CompilationResult>> CompileAndRunCppCodeAsync(CompilationRequestDTO compilationRequestDto)
    {
        Console.WriteLine(compilationRequestDto.Code);
        List<CompilationResult> results = new List<CompilationResult>();

        var cppFileName = "temp.cpp";
        var exeFileName = "temp.exe";

        // Write the C++ code to a temporary file
        await System.IO.File.WriteAllTextAsync(cppFileName, compilationRequestDto.Code);

        var testCaseNum = 0;
        foreach (var testCase in compilationRequestDto.testcases)
        {
            // Create a cancellation token source for monitoring memory usage and timeout
            CancellationTokenSource memoryCancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            // Execute the C++ code file using the command prompt
            var processInfo = new ProcessStartInfo
            {
                FileName = "g++",
                Arguments =$"{cppFileName} -o {exeFileName} ",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Environment =
                {
                    ["MEMORY_LIMIT_MB"] = compilationRequestDto.MemoryLimitMS.ToString(),
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
                Task monitorMemoryTask =
                    MonitorMemoryUsage(process, compilationRequestDto.MemoryLimitMS, memoryCancellationTokenSource.Token);
                Task timeoutTask = Task.Delay(compilationRequestDto.TimeLimitMS, timeoutCancellationTokenSource.Token);

                // Wait for either the process to exit, memory limit exceeded, or timeout
                Task completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

                if (completedTask == monitorMemoryTask)
                {
                    // If the memory limit is exceeded, terminate the process
                    process.Kill();
                    results.Add(new CompilationResult
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
                    results.Add(new CompilationResult
                    {
                        TestCaseNum = testCaseNum,
                        Success = false,
                        Error = "Time limit exceeded."
                    });

                    // Cancel the memory monitoring task
                    memoryCancellationTokenSource.Cancel();
                }
                else
                {
                    // Check if the output matches the expected output for this test case
                    var output = await process.StandardOutput.ReadToEndAsync();
                    string errorOutput = await process.StandardError.ReadToEndAsync();
                    
                    await process.WaitForExitAsync();
                    
                    
                    Console.WriteLine("Actual Output: " + output); 
                    
                    Console.WriteLine(errorOutput);
                    
                    if (output.Trim() == testCase.ExpectedOutput.Trim())
                    {
                        results.Add(new CompilationResult
                        {
                            TestCaseNum = testCaseNum,
                            Success = true,
                            Output = output
                        });
                    }
                    else
                    {
                        results.Add(new CompilationResult
                        {
                            TestCaseNum = testCaseNum,
                            Success = false,
                            Output = output,
                            Error = "Output does not match expected output."
                        });
                    }
                }
            }

            testCaseNum++;
        }
        Console.WriteLine((results.Count));
        return results;
    }

    private async Task MonitorMemoryUsage(Process process, int memoryLimitMB, CancellationToken cancellationToken)
    {
        while (!process.HasExited && !cancellationToken.IsCancellationRequested)
        {
            // Check memory usage of the process
            if (process.WorkingSet64 > memoryLimitMB * 1024 * 1024)
            {
                // If memory limit is exceeded, terminate the process
                process.Kill();
                return;
            }

            // Delay to avoid tight loop
            await Task.Delay(100, cancellationToken);
        }
    }
}