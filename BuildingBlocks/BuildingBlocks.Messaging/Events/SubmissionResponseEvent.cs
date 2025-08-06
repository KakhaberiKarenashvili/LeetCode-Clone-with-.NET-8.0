using BuildingBlocks.Common.Dtos;

namespace BuildingBlocks.Messaging.Events;

public class SubmissionResponseEvent
{
    public int SubmissionId { get; set; }
    
    public List<TestResultDto>? Results { get; set; }
}