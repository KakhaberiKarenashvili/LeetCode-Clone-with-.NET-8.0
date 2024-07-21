namespace informaticsge.models;

public class CompilationRequestDto
{
    public string Code { set; get; }

    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMs { set; get; }
    
    public List<TestCaseDto>? Testcases { set; get; }
}