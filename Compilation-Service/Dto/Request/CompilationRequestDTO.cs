namespace informaticsge.models;

public class CompilationRequestDTO
{
    public string Code { set; get; }

    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMS { set; get; }
    
    public List<TestCaseDTO> testcases { set; get; }
}