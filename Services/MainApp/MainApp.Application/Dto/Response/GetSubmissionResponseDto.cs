using BuildingBlocks.Common.Enums;

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
}