using System.Diagnostics;

namespace Compilation.Application.Services;

public class MemoryMonitorService
{
    private readonly ILogger _logger;

    public MemoryMonitorService(ILogger<MemoryMonitorService> logger)
    {
        _logger = logger;
    }

    public async Task MonitorMemoryUsage(Process process, int memoryLimitMb, CancellationToken cancellationToken)
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
}