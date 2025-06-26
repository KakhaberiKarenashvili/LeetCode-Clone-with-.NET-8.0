using BuildingBlocks.Messaging.Events;
using Compilation.Application.Services;
using MassTransit;

namespace Compilation.Application.Consumers;

public class CppSubmissionRequestEventHandler : IConsumer<CppSubmissionRequestedEvent>
{
    
    private readonly CppTestingService _service;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public CppSubmissionRequestEventHandler(CppTestingService service, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<CppSubmissionRequestedEvent> context)
    {
        var result = await _service.TestCppCode(context.Message);
        
        var response = new SubmissionResponseEvent()
        {
            SubmissionId = context.Message.SubmissionId,
            Results = result
        };
        
        await _publishEndpoint.Publish(response);
    }
}