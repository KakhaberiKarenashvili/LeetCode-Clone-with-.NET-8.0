using Compilation_Service.Dto.Request;
using Compilation_Service.Dto.Response;
using Compilation_Service.Services;

namespace Compilation_Service.RabbitMQ;

public class RequestListener
{
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RequestListener> _logger;

    public RequestListener(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<RequestListener> logger)
    {
        _rabbitMqService = rabbitMqService;
        _scopeFactory = scopeFactory;
        _logger = logger;

        _rabbitMqService.ReceiveRequest(ProcessRequest);
    }

    private void ProcessRequest(SubmissionRequestDto submissionRequestDto)
    {
        
        using (var scope = _scopeFactory.CreateScope())
        {
            var submissionRequestHandler = scope.ServiceProvider.GetRequiredService<SubmissionRequestHandler>();

            submissionRequestHandler.ProcessSubmissionRequest(submissionRequestDto);
        }
    }
}