using BuildingBlocks.Common.Dtos;
using BuildingBlocks.Messaging.Events;

namespace Compilation.Application.Models;

public class SubmissionRequestedEvent
{
    public string Code { get; set; }
    public string Language { get; set; }
    public List<TestCaseDto> Testcases { get; set; }
    public int MemoryLimitMb { get; set; }
    public int TimeLimitMs { get; set; }
}