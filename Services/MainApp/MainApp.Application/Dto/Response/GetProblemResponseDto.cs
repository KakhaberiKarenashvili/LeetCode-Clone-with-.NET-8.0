namespace MainApp.Application.Dto.Response;

public class GetProblemResponseDto
{
    public int Id { set; get; }
    
    public string? Name { set; get; }
    
    public string? ProblemText { set; get; }
    
    public string? Tag { set; get; }
    
    public string? Difficulty { set; get; }
    
    public int? TimelimitMs { set; get; }
    
    public int? MemoryLimitMb { set; get; }
    
    public string? ExampleInput { set; get; }
    
    public string? ExampleOutput { set; get; }
    
}