namespace informaticsge.Dto.Response;

public class TestResultDto
{
    public bool Success { get; set; }
    public string? Input {  get; set; }
    public string? ExpectedOutput {  get; set; }
    public string? Output { get; set; } 
    public string? Status { get; set; }
    
}