using BuildingBlocks.Messaging.Events;
using Compilation.Application.Extentions;
using Compilation.Application.Services;
using MassTransit;

namespace Compilation.Application.Consumers;

public class CppSubmissionRequestEventHandler : IConsumer<CppSubmissionRequestedEvent>
{
    
    private readonly CodeTestingService _service;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public CppSubmissionRequestEventHandler(CodeTestingService service, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<CppSubmissionRequestedEvent> context)
    {

        var genericEvent = context.Message.ToGenericEvent();
        
        var result = await _service.TestCode(genericEvent);
        
        var response = new SubmissionResponseEvent()
        {
            SubmissionId = context.Message.SubmissionId,
            Results = result
        };
        
        await _publishEndpoint.Publish(response);
    }
}