using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace informaticsge.Dto.Request;

public class SubmissionDto
{
    [Required]
    [FromQuery]
    public int ProblemId { get; set; }
    
    [Required]
    [FromQuery]
    [AllowedValues("C++", "Python", ErrorMessage = "Choose only allowed language"), ]
    public string Language { get; set; } 
    
}