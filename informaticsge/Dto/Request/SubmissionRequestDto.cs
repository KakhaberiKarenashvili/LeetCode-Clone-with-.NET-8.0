using System.ComponentModel.DataAnnotations;

namespace informaticsge.models;

public class SubmissionRequestDto
{
    public string Code { set; get; }

    public int MemoryLimitMb { set; get; }
    
    public int TimeLimitMs { set; get; }
    
    public List<TestCaseDto>? Testcases { set; get; }
}