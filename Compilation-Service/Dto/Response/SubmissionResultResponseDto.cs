namespace Compilation_Service.Dto.Response;

public class SubmissionResultResponseDto
{
    public int SubmissionId { get; set; }
    public List<TestResultDto>? Results { get; set; }
}