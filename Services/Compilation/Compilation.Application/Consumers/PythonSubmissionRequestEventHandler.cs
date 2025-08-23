using BuildingBlocks.Messaging.Events;
using Compilation.Application.Extentions;
using Compilation.Application.Services;
using MassTransit;

namespace Compilation.Application.Consumers;

public class PythonSubmissionRequestEventHandler : IConsumer<PythonSubmissionRequestedEvent>
{
    private readonly CodeTestingService _service;
    private readonly IPublishEndpoint _publishEndpoint;

    public PythonSubmissionRequestEventHandler(CodeTestingService service, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PythonSubmissionRequestedEvent> context)
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