namespace informaticsge.models;

public class SubmissionRequestDTO
{
    public string Code { set; get; }

    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMs { set; get; }
    
    public List<TestCaseDTO> testcases { set; get; }
}