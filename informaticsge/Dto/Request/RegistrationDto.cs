using System.ComponentModel.DataAnnotations;

namespace informaticsge.Dto.Request;

public class RegistrationDto
{
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Password { set; get; }
    
    [Required]
    public string UserName { set; get; }
    
}