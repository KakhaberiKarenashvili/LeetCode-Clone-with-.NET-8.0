using BuildingBlocks.Messaging.Events;
using Compilation.Application.Models;

namespace Compilation.Application.Extentions;

public static class EventExtensions
{
    public static SubmissionRequestedEvent ToGenericEvent(this ISubmissionRequestedEvent specificEvent)
    {
        return new SubmissionRequestedEvent
        {
            Code = specificEvent.Code,
            Language = specificEvent.Language,
            Testcases = specificEvent.Testcases,
            MemoryLimitMb = specificEvent.MemoryLimitMb,
            TimeLimitMs = specificEvent.TimeLimitMs
        };
    }

}