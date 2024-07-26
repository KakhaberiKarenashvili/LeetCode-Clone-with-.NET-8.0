namespace informaticsge.Dto.Response;

public class GetProblemResponseDto
{
    public int Id { set; get; }
    
    public string Name { set; get; }
    
    public string ProblemText { set; get; }
    
    public string Tag { set; get; }
    
    public int Timelimit { set; get; }
    
    public int MemoryLimit { set; get; }
    
    public string ExampleInput { set; get; }
    
    public string ExampleOutput { set; get; }
    
}