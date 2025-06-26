using System.ComponentModel.DataAnnotations;

namespace MainApp.Application.Dto.Request;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    [MaxLength(32)]
    public string Password { get; set; }
}