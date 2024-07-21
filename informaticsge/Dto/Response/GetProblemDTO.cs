namespace informaticsge.Dto;

public class GetProblemDto
{
    public int Id { set; get; }
    
    public string Name { set; get; }
    
    public string ProblemText { set; get; }
    
    public string Tag { set; get; }
    
    public string Timelimit { set; get; }
    
    public string MemoryLimit { set; get; }
    
    public string ExampleInput { set; get; }
    
    public string ExampleOutput { set; get; }
    
}