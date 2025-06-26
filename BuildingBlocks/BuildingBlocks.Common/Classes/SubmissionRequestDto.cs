namespace BuildingBlocks.Common.Classes;

public class SubmissionRequestDto
{
    public int SubmissionId { set; get; }
    public string Language { set; get; }
    public string Code { set; get; }
    public int MemoryLimitMb { set; get; }
    public int TimeLimitMs { set; get; }
    public required List<TestCaseDto> Testcases { set; get; }
}