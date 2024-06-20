namespace informaticsge.models;

public class CompilationRequestDTO
{
    public string Code { set; get; }

    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMs { set; get; }
    
    public List<TestCaseDTO>? Testcases { set; get; }
}