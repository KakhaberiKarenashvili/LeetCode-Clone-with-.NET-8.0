using BuildingBlocks.Common.Dtos;

namespace BuildingBlocks.Messaging.Events;

public record CppSubmissionRequestedEvent : ISubmissionRequestedEvent
{
    public int SubmissionId { set; get; }

    public string Language => "cpp";
    public string Code { set; get; }
    public int MemoryLimitMb { set; get; }
    public int TimeLimitMs { set; get; }
    public List<TestCaseDto>? Testcases { set; get; }
}
