namespace Compilation_Service.Dto.Request;

public abstract class TestCaseDto
{
    
    public string? Input { get; set; }
    public string? ExpectedOutput { get; set; }
    
}