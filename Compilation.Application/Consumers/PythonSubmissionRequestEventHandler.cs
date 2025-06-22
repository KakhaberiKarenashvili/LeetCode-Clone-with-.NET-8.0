using BuildingBlocks.Messaging.Events;
using Compilation.Application.Services;
using MassTransit;

namespace Compilation.Application.Consumers;

public class PythonSubmissionRequestEventHandler : IConsumer<PythonSubmissionRequestEvent>
{
    private readonly PythonTestingService _service;
    private readonly IPublishEndpoint _publishEndpoint;

    public PythonSubmissionRequestEventHandler(PythonTestingService service, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PythonSubmissionRequestEvent> context)
    {
        var result = await _service.TestPythonCode(context.Message);

        var response = new SubmissionResponseEvent()
        {
            SubmissionId = context.Message.SubmissionId,
            Results = result
        };
        
        await _publishEndpoint.Publish(response);
    }
}