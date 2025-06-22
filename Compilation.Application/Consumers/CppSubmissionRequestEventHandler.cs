using BuildingBlocks.Messaging.Events;
using Compilation.Application.Services;
using MassTransit;

namespace Compilation.Application.Consumers;

public class CppSubmissionRequestEventHandler : IConsumer<CppSubmissionRequestEvent>
{
    
    private readonly CppTestingService _service;
    private readonly IPublishEndpoint _publishEndpoint;
    
    public CppSubmissionRequestEventHandler(CppTestingService service, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task Consume(ConsumeContext<CppSubmissionRequestEvent> context)
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