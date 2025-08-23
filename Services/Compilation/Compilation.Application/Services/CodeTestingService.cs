using System.Diagnostics;
using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Common.Enums;
using Compilation.Application.Configuration.Languages;
using Compilation.Application.Configuration.Languages.Base;
using Compilation.Application.Dto.Common;
using Compilation.Application.Models;

namespace Compilation.Application.Services;

public class CodeTestingService
{
    private readonly ILogger<CodeTestingService> _logger;
    private readonly MemoryMonitorService _memoryMonitorService;

    public CodeTestingService(ILogger<CodeTestingService> logger, MemoryMonitorService memoryMonitorService)
    {
        _logger = logger;
        _memoryMonitorService = memoryMonitorService;
    }
    
    public async Task TestCode(SubmissionRequestedEvent submissionEvent)
    {
        var languageConfig = LanguageConfigurationFactory
            .GetLanguageConfiguration(submissionEvent.Language);
        
        var results = new List<TestResultDto>();
        var fileId = Guid.NewGuid().ToString("N")[..6];
        var sourceFile = languageConfig.GetFileName(fileId);
        var executableFileName = languageConfig.GetExecutableFileName(fileId);
        
        

    }


    private async Task<CompilationResult> CompileCode(
        LanguageConfigurationBase languageConfig,
        string sourceFileName,
        string executableFileNam
    )
    {
        var processInfo = languageConfig.GetCompileProcessStartInfo(sourceFileName, executableFileNam);

        var process = new Process { StartInfo = processInfo };
        
        process.Start();
        await process.WaitForExitAsync();
        
        var error = await process.StandardError.ReadToEndAsync();
        
        return new CompilationResult
        {
            Success = string.IsNullOrEmpty(error),
            Error = error
        };
        
    }



    private async Task<List<TestResultDto>> ExecuteCode(
        LanguageConfigurationBase languageConfig,
        string executableFileName,
        SubmissionRequestedEvent submissionEvent)
    {
        var results = new List<TestResultDto>();
        
        foreach (var testCase in submissionEvent.Testcases)
        {
            // Create a cancellation token source for monitoring memory usage and timeout
            CancellationTokenSource memoryCancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource();

            var processInfo = languageConfig.GetExecuteProcessStartInfo(executableFileName);
            processInfo.Environment["MEMORY_LIMIT_MB"] = submissionEvent.MemoryLimitMb.ToString();
            processInfo.Environment["TIME_LIMIT_MS"] = submissionEvent.TimeLimitMs.ToString();
            
            using var process = new Process { StartInfo = processInfo };
            process.Start();
            
            await process.StandardInput.WriteLineAsync(testCase.Input);
            process.StandardInput.Close();
            
            // Create tasks for monitoring memory usage and timeout
            Task monitorMemoryTask = _memoryMonitorService.MonitorMemoryUsage(process, submissionEvent.MemoryLimitMb, memoryCancellationTokenSource.Token);
            Task timeoutTask = Task.Delay(submissionEvent.TimeLimitMs, timeoutCancellationTokenSource.Token);
            
            var completedTask = await Task.WhenAny(process.WaitForExitAsync(), monitorMemoryTask, timeoutTask);

            if (completedTask == monitorMemoryTask)
            {
                process.Kill();
                results.Add(new TestResultDto
                {
                    Success = false,
                    Status = Status.MemoryLimitExceeded
                });
                await memoryCancellationTokenSource.CancelAsync();
            }
            else if(completedTask == timeoutTask)
            {
                process.Kill();
                results.Add(new TestResultDto
                {
                    Success = false,
                    Status = Status.TimeLimitExceeded
                });
            }
            else
            {
                var output = await process.StandardOutput.ReadToEndAsync(timeoutCancellationTokenSource.Token);
                var error = await process.StandardError.ReadToEndAsync(timeoutCancellationTokenSource.Token);

                results.Add(CreateTestResultDto(testCase, output, error));
            }
        }

        return results;
    }


    private TestResultDto CreateTestResultDto(TestCaseDto testCase,
        string output, string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            return new TestResultDto
            {
                Success = false,
                Input = testCase.Input,
                ExpectedOutput = testCase.ExpectedOutput,
                Output = error,
                Status = Status.TestFailed
            };
        }

        var passed = output.Trim() == testCase.ExpectedOutput?.Trim();
        return new TestResultDto
        {
            Success = passed,
            Input = testCase.Input,
            ExpectedOutput = testCase.ExpectedOutput,
            Output = output,
            Status = passed ? Status.TestPassed : Status.TestFailed
        };

    }
    
    
    private void CleanupTemporaryFiles(string sourceFile, string executableFileName)
    {
        if (System.IO.File.Exists(sourceFile))
        {
            System.IO.File.Delete(sourceFile);
        }

        if (System.IO.File.Exists(executableFileName))
        {
            System.IO.File.Delete(executableFileName);
        }
    }

}