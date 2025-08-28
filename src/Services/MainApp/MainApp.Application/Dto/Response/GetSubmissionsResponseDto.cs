using MainApp.Domain.Entity;

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


    public static GetSubmissionsResponseDto FromSubmission(Submissions submission)
    {
        return new GetSubmissionsResponseDto
        {
            Id = submission.Id,
            AuthUsername = submission.AuthUsername,
            ProblemName = submission.ProblemName,
            Language = submission.Language,
            SubmissionTime = submission.SubmissionTime,
            SuccessRate = $"{submission.SuccessRate}%",
            Status = submission.Status.ToString(),
        };
    }
    
}