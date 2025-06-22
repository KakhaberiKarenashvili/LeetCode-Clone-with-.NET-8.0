using BuildingBlocks.Common.Classes;

namespace BuildingBlocks.Messaging.Events;

public record CppSubmissionRequestEvent
{
    public int SubmissionId { set; get; }
    
    public string Code { set; get; }
    
    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMs { set; get; }
    
    public required List<TestCaseDto> Testcases { set; get; }
}
