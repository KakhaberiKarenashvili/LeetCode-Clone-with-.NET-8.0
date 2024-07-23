namespace informaticsge.Dto;

public class GetSubmissionsDTO
{
    public int Id { get; set; }
    
    public string AuthUsername { get; set; }
    
    public string ProblemName { get; set; }
    
    public string? Status { get; set; }
    
}