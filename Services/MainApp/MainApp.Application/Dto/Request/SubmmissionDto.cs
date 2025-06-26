using System.ComponentModel.DataAnnotations;

namespace MainApp.Application.Dto.Request;

public class SubmissionDto
{
    [Required]
    public int ProblemId { get; set; }
    
    [Required]
    [AllowedValues("C++", "Python", ErrorMessage = "Choose only allowed language"), ]
    public string Language { get; set; } 
    
    [Required]
    public string Code { get; set; }
    
}