namespace MainApp.Application.Dto.Response;

public class GetSubmissionsResponseDto
{
    public int Id { get; set; }
    
    public string? AuthUsername { get; set; }
    
    public string? ProblemName { get; set; }
    
    public string? Language { get; set; }
    public string? Status { get; set; }
    
    public string? SuccessRate { get; set; }
    
    public DateTime? SubmissionTime { get; set; }
    
}