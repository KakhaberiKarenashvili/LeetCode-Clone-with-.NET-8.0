using System.ComponentModel.DataAnnotations;

namespace informaticsge.Dto.Request;

public class RegistrationDto
{
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string UserName { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    [MaxLength(32)]
    public string Password { get; set; }
    
}