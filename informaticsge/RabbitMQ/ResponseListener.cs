using informaticsge.Controllers;
using informaticsge.Dto.Response;
using informaticsge.Entity;
using informaticsge.Services;

namespace informaticsge.RabbitMQ;

public class ResponseListener
{
    private readonly ILogger<ResponseListener> _logger;
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _scopeFactory;

    public ResponseListener(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<ResponseListener> logger)
    {
        _rabbitMqService = rabbitMqService;
        _scopeFactory = scopeFactory;
        _logger = logger;

        // Start listening for messages
        _rabbitMqService.ReceiveResult(ProcessResult);
    }

    // Method to process the request
    private void ProcessResult(SubmissionResponseDto request)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Resolve any services you need from the scope
            var submissionService = scope.ServiceProvider.GetRequiredService<SubmissionService>();

            // Handle the request using the service
           submissionService.HandleSubmissionResults(request);
        }
    }
}