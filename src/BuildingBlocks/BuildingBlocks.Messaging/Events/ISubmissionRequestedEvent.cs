using BuildingBlocks.Common.Dtos;

namespace BuildingBlocks.Messaging.Events;

public interface ISubmissionRequestedEvent
{
    int SubmissionId { get; set; }
    string Code { get; set; }
    string Language { get; }
    List<TestCaseDto> Testcases { get; set; }
    int MemoryLimitMb { get; set; }
    int TimeLimitMs { get; set; }

}