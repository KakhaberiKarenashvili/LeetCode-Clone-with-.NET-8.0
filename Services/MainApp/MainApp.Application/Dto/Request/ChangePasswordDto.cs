using System.ComponentModel.DataAnnotations;

namespace MainApp.Application.Dto.Request;

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; }
    [Required]
    public string NewPassword { get; set; }
}