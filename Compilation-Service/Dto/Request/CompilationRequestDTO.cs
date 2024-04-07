namespace informaticsge.models;

public class CompilationRequestDTO
{
    public string Code { set; get; }

    public int MemoryLimitMS { set; get; }
    
    public int TimeLimitMS { set; get; }
    
    public List<TestCaseDTO> testcases { set; get; }
}