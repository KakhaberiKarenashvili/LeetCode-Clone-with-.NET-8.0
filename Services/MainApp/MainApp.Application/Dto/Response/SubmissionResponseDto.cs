namespace MainApp.Application.Dto.Response;

public class SubmissionResponseDto
{
    public int SubmissionId { get; set; }
    
    public List<TestResultDto>? Results { get; set; }
}