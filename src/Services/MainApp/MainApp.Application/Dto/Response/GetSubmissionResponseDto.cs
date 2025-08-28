using BuildingBlocks.Common.Enums;
using MainApp.Domain.Entity;

namespace MainApp.Application.Dto.Response;

public class GetSubmissionResponseDto
{
    public int Id { get; set; }
    public string AuthUsername { get; set; }
    public string Language { set; get; }
    public string Code { get; set; }
    public int ProblemId { get; set; }
    public string ProblemName { get; set; }
    public string Status { get; set; }
    
    public string SuccessRate { get; set; }
    
    public DateTime SubmissionTime { get; set; }
    public string? Input { set; get; }
    public string? ExpectedOutput { set; get; }
    public string? Output { set; get; }

    public static GetSubmissionResponseDto FromSubmission(Submissions submission)
    {
        return new GetSubmissionResponseDto
        {
            Id = submission.Id,
            AuthUsername = submission.AuthUsername,
            ProblemId = submission.ProblemId,
            ProblemName = submission.ProblemName,
            Status = submission.Status.ToString(),
            Language = submission.Language,
            SubmissionTime = submission.SubmissionTime,
            SuccessRate = $"{submission.SuccessRate}%",
            Code = submission.Code,
            Output = submission.Output,
            Input = submission.Input,
            ExpectedOutput = submission.ExpectedOutput,
        };
    }
}