using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Services;

namespace informaticsge.RabbitMQ;

public class ResponseListener : BackgroundService
{
    private readonly ILogger<ResponseListener> _logger;
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _scopeFactory;

    public ResponseListener(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<ResponseListener> logger)
    {
        _rabbitMqService = rabbitMqService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMqService.ReceiveResult(ProcessResult);
    }
    
    // Method to process the request
    private async Task ProcessResult(SubmissionResponseDto request)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var submissionService = scope.ServiceProvider.GetRequiredService<SubmissionService>();
            
            try
            {
                await submissionService.HandleSubmissionResults(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process submission result for SubmissionId {SubmissionId}", request.SubmissionId);
            }
        }
    }
    
}